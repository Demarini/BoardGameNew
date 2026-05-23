---
description: Display the VRChat board game project architecture, key systems, and file locations. Use when you need to understand how the project is structured or find where something lives.
---

Read the project architecture reference below and use it to answer questions or inform your work.

# VRChat Board Game — Project Architecture

## Overview

A multiplayer VRChat drinking board game built with UdonSharp. Players take turns rolling dice, land on spaces with various effects (drink, move, swap positions, etc.), and race to the finish. Features network-synced state, rubber-banding catch-up mechanics, mystery spaces with slot-machine animations, and a HUD popup system.

**Engine:** Unity 2022.3.22f1 + VRChat SDK
**Scripting:** UdonSharp (compiled to Udon VM — see `.claude/rules/udonsharp.md` for constraints)
**Networking:** VRChat manual sync via `[UdonSynced]` + `RequestSerialization()` + `OnDeserialization()`

## Core file map

### Game flow (`Assets/Scripts/GameController/`)

- **`GameController_BoardGame.cs`** — central orchestrator. Game start/end, turn progression, dice validation, the `Process*` space-effect methods.
- **`RollDiceHelper_BoardGame.cs`** — dice rolling (master-only), weighted rolls, rubber-banding (`RerollForHugeLeader`, `RerollIfInLoop`, `RerollForFinalSpaceConditions`), mystery detection, `ProcessLandingEffects()` pipeline.
- **`RollDiceInteract_BoardGame.cs`** — player-facing dice click handler.
- **`RunDiceTimer.cs`** — idle-prompt timer for inactive players.
- **`SyncedHelper.cs`** — `IsReady()` guard for code that may run before sync completes.
- **`UpdateSpaces.cs`** — board outline highlight management.

### Space configuration (`Assets/Scripts/CustomBoardScripts/`)

- **`SpaceSettings.cs`** — per-space data component. Boolean/int flags per effect type.
- **`MysteryManager.cs`** — mystery-space resolution, slot animation, runtime `SpaceSettings` mutation.
- **`ImageSettings.cs`** / **`TextSettings.cs`** — material references and display-text templates.
- **`BoardSettings.cs`** — legacy square-only board enum (superseded by JSON).

### Board generation (`Assets/Editor/`)

- **`UpdateBoard.cs`** — primary entry: `BoardGame → Generate Board From JSON`. Reads JSON, clears existing spaces, snake-lays out the grid, bakes mystery pool. See `.claude/rules/editor-scripts.md`.

### Board definitions (`Assets/BoardGame/BoardDefinitions/`)

- `ClassicBoard.json` — 7x7 traditional layout, fixed space types.
- `MysteryMadness.json` — 7x7, ~90% mystery, weighted pool sums to 100.
- `BattleRoyale.json` — newer variant.
- Schema and conventions: `.claude/rules/board-json.md`.

### Player management (`Assets/Scripts/PlayerScripts/`)

- **`PlayerList_BoardGame.cs`** — player IDs, status (Connected / Left / Disconnected), name sync, join/leave/rejoin.
- **`PlayerFunctions_BoardGame.cs`** — add/remove player operations.

### Camera system (`Assets/Scripts/CameraScripts/`)

- **`CameraFollowHead.cs`** — positions cameras at each player's head tracking data. Guarded by `SyncedHelper.IsReady()`.
- **`UpdatePlayerCamerasOnSpace_BoardGame.cs`** — display panel camera tiers by player count.

### Audio (`Assets/Scripts/AudioScripts/`)

- **`ToggleGameAudio_BoardGame.cs`** — each effect type has a synced increment counter; `ToggleAudio()` runs in `OnPreSerialization`/`OnDeserialization` and detects deltas to play the right cue.

### UI (`Assets/Scripts/UIScripts/`)

- **`GameFunctions_BoardGame.cs`** — start/stop UI hooks.
- **`SpacePopupHUD.cs`** — head-tracking TMP popup. Synced via `PopupIncrement` + `PopupMessage` + `PopupTargetPlayerIndex`.
- **`RotateHourglass.cs`** — idle visual.

### Special features

- **`AnyomBoard.cs`** (`GameController/`) — alternate board for player named "Anyom".
- **`RunMikeyPlayer.cs`** (`Halloween/`) — jump-scare creature.
- **`RunSanta.cs`** (`MiscScripts/`) — decorative cycling Santa.

## Scene hierarchy contract

Several scripts hardcode this layout via `Find()`. **Do not rename these nodes:**

```
<Root Board>/
  Board/
    Spaces/                   <- visual space GameObjects (Space - 0, Space - 1, ...)
      Space - N/
        Canvas/
          Text                <- space number
          Text (1)            <- description text (mutated during mystery spin)
        SpaceImage            <- renderer (mutated during mystery spin)
        [children 8/9/10/11]  <- outline objects (red/blue/overlap)
  CreateCustomBoard/
    Prefabs/
      Space, SpaceSetting     <- copied (not instantiated) by UpdateBoard
    Scripts/
      SpaceSettings/          <- parent of all SpaceSetting instances
      BoardSettings           <- legacy board size config
      ImageSettings           <- material references
      TextSettings            <- text templates
      MysteryManager          <- mystery pool + animation logic
```

## Runtime space lookup

- `GameController.GetSpace(int i)` → `boardGameSpaceSettings.transform.GetChild(i).GetComponent<SpaceSettings>()`
- `GameController.IsEnd(int i)` → `boardGameSpaceSettings.transform.childCount - 1 == i`
- Space 0 is always Start, last space is always Finish.

# Game flow — end-to-end

This is the full path from "player clicks dice" to "next player's turn".

## 1. The click (player-local)

`RollDiceInteract_BoardGame.Interact` → `GameController.RollDice()`:

```csharp
if (Networking.LocalPlayer.playerId == playerLists.playersInGameDataList[gameVariables.CurrentPlayerIndex]) {
    spacePopupHUD.DismissPopup();          // hide the persistent "Roll the Dice!" popup
    SendCustomNetworkEvent(NetworkEventTarget.Owner, "RollDiceMaster");
}
```

Only the validated current player can roll. The event goes to the **owner** (master).

## 2. Master rolls the dice

`GameController.RollDiceMaster()` → `RollDiceHelper.RollDiceMaster()` (master-only):

```csharp
int roll = CalculateWeightedRoll(100, 100, 100, 100, 100, 100);   // uniform 1-6
roll = RerollForHugeLeader(roll);                                  // rubber-band leader
roll = RerollIfInLoop(roll);                                       // space-12-16 trap
roll = RerollForFinalSpaceConditions(roll);                        // El Linguino / time gate

if (roll == gameVariables.CurrentRoll) gameVariables.SameRoll++;   // same-roll counter ensures sync fires
else gameVariables.CurrentRoll = roll;
gameVariables.RequestSerialization();
```

`CurrentRoll` and `SameRoll` are both `[UdonSynced, FieldChangeCallback]`. The setter calls `TurnAnimsOff()` which sets `sameRollDelay = true`, and `Update()` then schedules the dice animation.

### Rubber-banding details

- `RerollForHugeLeader`: only fires when caller is in first AND ≥10 spaces ahead of second. Near space 24 (the SendBackToStart on Classic), forces them onto 24. Otherwise low-rolls.
- `RerollIfInLoop`: spaces 12-16 cap to a max roll of 4. (Classic-board loop trap; doesn't help on other boards.)
- `RerollForFinalSpaceConditions`: re-rolls off finish if player name is "El Linguino" OR `timer < 1800` (30 min).

These are board-specific magic numbers — see `.claude/rules/udonsharp.md`.

## 3. Animation → delayed `CalculateRoll`

After `CurrentRoll` syncs, every client (incl. master) runs:

```csharp
// GameVariables_BoardGame.cs
void TurnAnimsOff() { ... rollDiceAnim.SetBool("Roll*", false); sameRollDelay = true; }

void Update() {
    if (sameRollDelay && sameRollDelayTimer > .1) RollTheDiceAnim();   // starts the visual roll
    if (rollAnimationStart && rollTimer > 2) rollDiceHelper.CalculateRoll();   // ← key: 2-sec delay
}
```

`CalculateRoll` runs on **every client** but exits early on non-master:

```csharp
public void CalculateRoll() {
    if (!Networking.LocalPlayer.isMaster) return;
    // ...
}
```

So the visible delay between roll and movement is **2 seconds**, and only master computes the landing.

## 4. Landing + space lookup

`RollDiceHelper.CalculateRoll()`:

```csharp
int finalLandingSpace = gameController.CalculateLandingSpace(CurrentRoll, currentPlayerSpace);
SpaceSettings spaceSetting = gameController.GetSpace(finalLandingSpace);

if (finalLandingSpace == 0) {
    // landed on Start exactly — just update space, NextPlayer.
} else if (gameController.IsEnd(finalLandingSpace)) {
    // landed on Finish — WinnerName, WinnerDetected++, EndGame().
} else if (spaceSetting.IsMystery) {
    // sync position immediately so the outline shows on the landing space
    gameVariables.playerSpaceDataList[CurrentPlayerIndex] = finalLandingSpace;
    gameVariables.RequestSerialization();
    updateSpaces.UpdateOutlineSpaces();
    waitingForMystery = true;
    mysteryManager.StartMysteryAndWait(spaceSetting, finalLandingSpace);
    return;   // ← effect pipeline resumes from OnMysteryResolved()
} else {
    ProcessLandingEffects(finalLandingSpace, spaceSetting);
}
```

`CalculateLandingSpace` handles wrap-around: if the roll would exceed the last space, it bounces back (`totalSpaces - (roll - (totalSpaces - space))`).

## 5. The effect pipeline

`ProcessLandingEffects` is a **loop, up to 10 iterations**, because effects can chain (e.g. moveForward → land on swap → swap puts you on missTurn):

```csharp
while (!movementHasEnded) {
    bool sendBackToStart = gameController.ProcessSendBackToStart(spaceSetting);
    int moveForwardBackwards = gameController.ProcessLandedSpaceMovement(spaceSetting);
    int swapPlayer = (int)gameController.ProcessSwapWithPlayer(spaceSetting);

    if (sendBackToStart)        { finalLandingSpace = 0; ... }
    else if (moveForwardBackwards != 0) { finalLandingSpace = CalculateLandingSpace(...); }
    else if (swapPlayer != 0)   { swap positions; finalLandingSpace = new position; }
    else                        { movementHasEnded = true; }

    spaceSetting = gameController.GetSpace(finalLandingSpace);
    if (++numberOfMovements > 10) break;
}
gameVariables.playerSpaceDataList[CurrentPlayerIndex] = finalLandingSpace;

if (IsEnd(finalLandingSpace)) { /* WinnerDetected++; EndGame(); */ return; }

gameController.ProcessPopup(spaceSetting, wasSentBack, lastSwapType);
gameController.ProcessMissedTurn(spaceSetting);
gameController.ProcessAudio(spaceSetting);
if (!gameController.ProcessRollAgain(spaceSetting)) gameController.NextPlayer();
else                                                gameController.SamePlayerRollAgain();
updateSpaces.UpdateOutlineSpaces();
```

**Priority order is fixed** (`sendBackToStart > movement > swap > other`), enforced by the if/else chain. Only one movement-class effect fires per iteration; non-movement effects (drink/etc.) all evaluate at the end.

## 6. Mystery resolution

Master's `StartMysteryAndWait(spaceSetting, landingSpace)`:

```csharp
pendingSpaceSettings = spaceSetting;
pendingLandingSpace = landingSpace;
waitingForSpinComplete = true;
ResolveMystery(0);   // picks weighted index, syncs, starts animation
```

`ResolveMystery` (master-only):

```csharp
targetIndex = PickWeightedRandom();
mysteryResultIndex = targetIndex;            // [UdonSynced]
mysteryLandingSpace = pendingLandingSpace;   // [UdonSynced]
mysteryResolveIncrement++;                   // [UdonSynced] one-shot counter
localMysteryResolveIncrement = mysteryResolveIncrement;
RequestSerialization();
StartSpinAnimation();
```

**Both master and clients** run `StartSpinAnimation`:
- Master starts it directly above.
- Clients run it via `OnDeserialization` when `mysteryResolveIncrement != localMysteryResolveIncrement`.

The spin runs purely from `Update()` — cubic-eased tick interval (`minInterval` → `maxInterval`), random material/text from the pool until `progress >= 0.85`, then locks to `targetIndex`. After `progress >= 1`:

- All clients show the final visual.
- **Master only** calls `ApplyMysteryResult()` (mutates the landed `SpaceSettings`) and `rollDiceHelper.OnMysteryResolved()`.

`OnMysteryResolved` re-enters the effect pipeline with the now-mutated SpaceSettings:

```csharp
public void OnMysteryResolved() {
    if (!waitingForMystery || !Networking.LocalPlayer.isMaster) return;
    waitingForMystery = false;
    int finalLandingSpace = gameVariables.playerSpaceDataList[CurrentPlayerIndex].Int;
    SpaceSettings spaceSetting = gameController.GetSpace(finalLandingSpace);   // now reflects resolved type
    ProcessLandingEffects(finalLandingSpace, spaceSetting);
}
```

See `.claude/rules/udonsharp.md` ("mystery space gotcha") for the runtime-mutation implications.

## 7. Turn handoff

`NextPlayer()` increments through the player list, skipping disconnected/left players and clearing missed-turn flags as it goes. It triggers `PlayerUpdateBoard++` which (via `FieldChangeCallback` on master + `OnDeserialization` for clients) fires `PostRollUpdates()` — outline refresh, camera updates, idle timer reset.

`SamePlayerRollAgain()` is the variant for `RollAgain` spaces; same `PlayerUpdateBoard++` but without incrementing the player index.

# Sync subsystems

## Outline sync (`UpdateSpaces.cs`)

Every visual space GameObject has **children 8, 9, 10, 11** that are outline meshes (red front/back, blue front/back). Outline state is purely local — each client computes it from synced `PreviousPlayerIndex` + `playerSpaceDataList`.

`UpdateOutlineSpaces()` rules:

- Previous-player's space gets red (children 8 + 9 active).
- Self-player's space gets blue (children 10 + 11 active).
- If previous and self are the same player → only blue.
- If previous and self are on the *same space* (different players) → red + blue overlap (children 9 + 10).

`ShowOutlineOnSpace(int)` is the simpler variant used during mystery spin — highlights a specific space immediately so all clients can see where the mystery is resolving, even before `OnMysteryResolved` finishes.

## Popup HUD (`SpacePopupHUD.cs`)

Head-tracking TMP that follows the local player's view. Synced via three vars on `GameVariables_BoardGame`:

- `PopupIncrement` (int, increments on every popup event)
- `PopupMessage` (string)
- `PopupTargetPlayerIndex` (int; **-1 means everyone**, otherwise only that player sees it)

`CheckPopup()` runs in both `OnPreSerialization` and `OnDeserialization` (so master sees its own popups). Uses `tmpPopupIncrement` for edge detection.

`ShowPersistentPopup("Roll the Dice!")` is called from `CheckToUpdateDiceClickerInteract` when it's the local player's turn — stays open until `DismissPopup()` (which happens on `RollDice()`).

## Audio toggle (`ToggleGameAudio_BoardGame.cs`)

Each effect type has a synced `Toggle*` counter on `GameVariables_BoardGame`. `ToggleAudio()` runs in `OnPreSerialization`/`OnDeserialization` and detects which counter changed.

Two targeting modes:

- **Global** (everyone hears): `EveryoneDrink`, `GirlsDrink`, `GuysDrink`, `SendBackToStart`, `SwapWith*`.
- **Self-only** (only the affected player): `Drink`, `ChooseSomeoneToDrink`, `DrinkWithHost`, `DrinkWhatYouRoll`, `Immune`, `MissTurn`, `RollAgain` — gated by `if (PreviousPlayerIndex == selfIndex)`.

Players not currently in the game skip audio (they still update `tmp*` to avoid catch-up firing).

## Game state sync (`GameVariables_BoardGame.cs`)

`OnPreSerialization` serializes `DataList`s to JSON, then fans out to subsystems:

```csharp
public override void OnPreSerialization() {
    MissedTurnJson = helperFunctions.SerializeDataList(missedTurnDataList, MissedTurnJson);
    PlayerSpaceJson = helperFunctions.SerializeDataList(playerSpaceDataList, PlayerSpaceJson);
    playerLists.UpdatePlayersInGameText();
    cameraFollowHead.TakePicture();
    toggleGameAudio.ToggleAudio();
    spacePopupHUD.CheckPopup();
    // ... winner / music checks
}
```

`OnDeserialization` mirrors this — deserialize the JSON strings, run all the change-detection hooks, fire `PostRollUpdates` if `PlayerUpdateBoard` changed.

Most synced state is in `GameVariables_BoardGame.cs`; player roster + status is in `PlayerList_BoardGame.cs`.

# Network sync recap

Master makes all game-state decisions. Synced state uses two patterns (full detail in `.claude/rules/udonsharp.md`):

1. `[UdonSynced, FieldChangeCallback]` — side effect inline on every deserialize.
2. `[UdonSynced]` int + `tmp*` field, manual edge-detect in `OnDeserialization`.

One-shot events use an **incrementing int counter**, never a bool. `DataList`s are serialized to JSON strings via `HelperFunctions_BoardGame.SerializeDataList` / `DeserializeDataList`.
