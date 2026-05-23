# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

A multiplayer VRChat drinking board game built with Unity 2022.3.22f1 + VRChat SDK + UdonSharp. Players take turns rolling dice, land on spaces with effects (drink, move, swap, miss turn, mystery, etc.), and race to the finish. Master client owns all game-state decisions; clients see results via VRChat manual sync.

Gameplay scripts under `Assets/` are **UdonSharp** (compiled to the Udon VM, with significant constraints). Editor scripts under `Assets/Editor/` are standard C#. Detailed rules for both auto-load via `.claude/rules/` — see "Project resources" below.

## How to build / test

There is **no command-line build or test runner**. Everything happens inside the Unity Editor:

- Open the project in Unity 2022.3.22f1 (the VRChat-supported version).
- Scene: `Assets/Scenes/VRCDefaultWorldScene.unity`.
- Local multiplayer testing: VRChat **ClientSim** (under `ClientSimStorage/`) via the VRChat SDK control panel.
- World upload: VRChat SDK control panel → Build & Publish.
- Compile errors surface in the Unity Console; UdonSharp recompiles on save.
- The `*.csproj` / `*.sln` files at the repo root are Unity-generated and gitignored — do not edit by hand.

No unit tests. Verification is done by running the scene in ClientSim and exercising the board.

## High-level architecture

Master-authoritative state machine driven by `GameController_BoardGame`. Each `UdonSharpBehaviour` has both a `.cs` script and a paired `.asset` (UdonSharp program serialization) — commit them together.

Top-level systems:

- **Game flow** — `Assets/Scripts/GameController/` (GameController, RollDiceHelper, UpdateSpaces, SyncedHelper)
- **Space configuration** — `Assets/Scripts/CustomBoardScripts/` (SpaceSettings, MysteryManager, ImageSettings, TextSettings)
- **Board generation (editor-time)** — `Assets/Editor/UpdateBoard.cs`, driven by JSON in `Assets/BoardGame/BoardDefinitions/`
- **Players / cameras / audio / UI** — `Assets/Scripts/{PlayerScripts, CameraScripts, AudioScripts, UIScripts}/`
- **Game state** — `Assets/Scripts/GameVariables_BoardGame.cs` (most `[UdonSynced]` state lives here)

Full game-flow walkthrough (turn loop, dice roll pipeline, mystery resolution, outline + popup + audio sync subsystems) is in the `board-game-project-architecture` skill.

### Scene hierarchy contract

Scripts hardcode this layout via `Find()` chains — do not rename:

```
<Root Board>/
  Board/Spaces/Space - N/...
  CreateCustomBoard/
    Prefabs/{Space, SpaceSetting}        <- copied (not instantiated) by UpdateBoard
    Scripts/{SpaceSettings, BoardSettings, ImageSettings, TextSettings, MysteryManager}
```

### Runtime lookup contract

- `GameController.GetSpace(i)` → `boardGameSpaceSettings.transform.GetChild(i).GetComponent<SpaceSettings>()`
- `GameController.IsEnd(i)` → `i == boardGameSpaceSettings.transform.childCount - 1`
- Space 0 = Start, last index = Finish (forced by `UpdateBoard`).

## Board generation

Boards are JSON files under `Assets/BoardGame/BoardDefinitions/`. To apply changes: select the root board GameObject in the scene, then **`BoardGame → Generate Board From JSON`**. This script clears existing spaces, snake-lays out the grid, and bakes the mystery pool into `MysteryManager`.

Editing a board JSON without regenerating has **no runtime effect**. Schema and field rules are in `.claude/rules/board-json.md` (auto-loads when editing those files).

## Repo conventions

- Commit `.cs` + matching `.asset` together.
- `Assets/SerializedUdonPrograms/` is gitignored (regenerates on compile).
- `Library/`, `Temp/`, `obj/`, `Logs/`, `UserSettings/` are gitignored Unity caches.
- Branches use the `tm/<feature>` prefix; PRs target `develop`.

## Project resources

**Rules** (`.claude/rules/`, auto-load by file path):

- `udonsharp.md` — UdonSharp compile constraints, sync patterns, master-authoritative rule, mystery-space runtime-mutation gotcha, hardcoded rubber-banding magic numbers. Loads on `Assets/**/*.cs`.
- `editor-scripts.md` — board generation pipeline, clipboard-copy pattern, the three methods every new type touches. Loads on `Assets/Editor/**/*.cs`.
- `board-json.md` — JSON schema, known type strings, weight conventions, regenerate reminder. Loads on `Assets/BoardGame/BoardDefinitions/**/*.json`.

**Skills** (`.claude/skills/`, on-demand):

- `board-game-project-architecture` — full game flow walkthrough (turn loop, dice pipeline, mystery resolution, outline/popup/audio sync) plus file inventory and scene contracts.
- `new-board-space` — guided implementation of a new space type from plain-language description, including the full post-implementation checklist.
