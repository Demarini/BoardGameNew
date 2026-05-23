---
paths:
  - "Assets/**/*.cs"
---

# UdonSharp + networking rules

These rules load any time Claude touches a `.cs` file under `Assets/`. They govern how gameplay code must be written and reasoned about.

## UdonSharp compile constraints

UdonSharp compiles C# to the Udon VM. Standard C# features that **don't work**:

- Interfaces, abstract classes, generics
- `Dictionary<,>` — use parallel arrays, `DataList`, or `DataDictionary` from `VRC.SDK3.Data`
- LINQ in hot paths — use explicit loops
- Polymorphic dispatch — use `if/else` chains with concrete types
- `GetComponent<Interface>()` — instead, get a reference via `[SerializeField]` and call directly

Editor-only code under `Assets/Editor/` is standard C# (no Udon constraints) — see `.claude/rules/editor-scripts.md`.

The `*.csproj`/`*.sln` files at the repo root are Unity-generated and gitignored. Never hand-edit them.

## Master-authoritative game state

The master client makes **all** game-state decisions; clients react to deserialized state. The contract is:

1. Master reads input, decides outcome.
2. Master mutates `[UdonSynced]` fields and calls `RequestSerialization()`.
3. Clients receive the update; `OnDeserialization()` fires; side effects execute on every client (including master).

If you write logic that runs on a client and mutates synced state, that's a bug. Wrap it in `if (Networking.LocalPlayer.isMaster)`, or send a network event to the owner with `SendCustomNetworkEvent(NetworkEventTarget.Owner, "MethodName")`.

## Two sync patterns coexist — pick the right one

**Pattern A — `FieldChangeCallback` (side effect inline on set):**

```csharp
[UdonSynced, FieldChangeCallback(nameof(WinnerName))]
public string winnerName = "";
public string WinnerName {
    set { winnerName = value; winnerText.text = "Winner\n" + value; }
    get => winnerName;
}
```

The setter fires on every deserialize. Use for small, idempotent side effects (UI text, simple flags).

**Pattern B — `tmp*` field + compare in `OnDeserialization` (manual edge detect):**

```csharp
int tmpPlayerUpdateBoard = 0;
[UdonSynced] public int playerUpdateBoard;

public override void OnDeserialization() {
    if (playerUpdateBoard != tmpPlayerUpdateBoard) {
        tmpPlayerUpdateBoard = playerUpdateBoard;
        PostRollUpdates();   // expensive — only on actual change
    }
}
```

Use for expensive, order-dependent, or "once per event" effects. The `tmp*` field name convention is used throughout `GameVariables_BoardGame.cs`.

Both patterns coexist intentionally. Don't unify them — `FieldChangeCallback` for cheap callbacks, manual edge detect for everything else.

## Incrementing-counter convention for one-shot events

Cross-client one-shot events (audio cues, popups, mystery results, swap notifications) use an `int` synced counter that master increments. Clients detect the change via either sync pattern and fire the effect.

Examples: `ToggleDrink++`, `WinnerDetected++`, `PopupIncrement++`, `mysteryResolveIncrement++`, `SwapWithFirstIncrement++`.

**Never** use a `bool` for one-shot events — you can't detect a re-fire if the value didn't change. Always increment.

## DataLists must be serialized to JSON

`DataList` from `VRC.SDK3.Data` is not directly sync-able. The pattern, used by `playerSpaceDataList` and `missedTurnDataList`:

```csharp
public DataList playerSpaceDataList = new DataList();
[UdonSynced, FieldChangeCallback(nameof(PlayerSpaceJson))]
public string playerSpaceJson;

public override void OnPreSerialization() {
    PlayerSpaceJson = helperFunctions.SerializeDataList(playerSpaceDataList, PlayerSpaceJson);
}
public override void OnDeserialization() {
    playerSpaceDataList = helperFunctions.DeserializeDataList(PlayerSpaceJson, playerSpaceDataList);
}
```

`HelperFunctions_BoardGame.SerializeDataList` / `DeserializeDataList` are the canonical wrappers.

## Runtime invariants (do not break)

- `GameController.GetSpace(i)` reads `boardGameSpaceSettings.transform.GetChild(i).GetComponent<SpaceSettings>()`. Space index 0 is always Start, last index is always Finish. `IsEnd(i)` is `childCount - 1 == i`.
- The scene hierarchy under `<Root Board>/` is hard-coded via `Find()` chains in `UpdateBoard.cs` and elsewhere. Do not rename `Board/`, `Spaces/`, `CreateCustomBoard/`, `Prefabs/`, `Scripts/`, or any direct children.
- Each visual space GameObject has **children 8/9/10/11** that are outline objects (red/blue/overlap). `UpdateSpaces.cs` and `MysteryManager.cs` both toggle these by index — don't reorder children.

## The mystery space gotcha

`MysteryManager.ApplyMysteryResult()` **mutates the landed `SpaceSettings` at runtime**. It resets every flag, then sets the resolved type's flag, and the space then behaves as a normal space of that type. The runtime visual (material + text) is also overwritten by the spin animation and stays until the next mystery lands there.

Consequences when modifying gameplay:

- A new space type added to `SpaceSettings.cs` but **not** to `ApplyMysteryResult()`'s reset block will leak old flags into mystery-resolved spaces.
- A new mystery pool type added to JSON but **not** to `ApplyMysteryResult()`'s if/else chain (and `GetTextForType` / `GetMaterialForType`) will silently no-op when rolled.
- Mystery spaces that have already resolved this game cannot be re-randomized — the original `IsMystery=true` flag is wiped during resolution.

## Hardcoded rubber-banding magic numbers

`RollDiceHelper_BoardGame.cs` contains board-specific magic numbers from when the game was always a 7x7 / 49-space board. They break on other boards. If you touch dice logic, surface this:

- `RerollIfInLoop`: between spaces 12–16, caps roll at 4 (was a loop trap on the Classic board).
- `RerollForHugeLeader`: a 10-space lead while in first triggers a reroll-low; if near space 24, forces them onto space 24 (the SendBackToStart space on the original board).
- `RerollForFinalSpaceConditions`: player named "El Linguino" or `timer < 1800` (30 minutes) triggers a reroll off the finish.
- The reroll-off-finish minimum was changed from 50 to 30 minutes in `b6877ab5`.

## Asset coupling

Each `UdonSharpBehaviour` has both a `.cs` script and a paired `.asset` (the UdonSharp program serialization). Commit them together — runtime references the asset, not the script. Unity regenerates the asset on save; if you rename a serialized field, the asset must be touched/saved for the change to apply.

`Assets/SerializedUdonPrograms/` is gitignored; it regenerates on compile.
