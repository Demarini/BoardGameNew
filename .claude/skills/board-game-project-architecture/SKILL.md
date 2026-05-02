---
description: Display the VRChat board game project architecture, key systems, and file locations. Use when you need to understand how the project is structured or find where something lives.
---

Read the project architecture reference below and use it to answer questions or inform your work on this codebase.

# VRChat Board Game - Project Architecture

## Overview
A multiplayer VRChat drinking board game built with UdonSharp. Players take turns rolling dice, land on spaces with various effects (drink, move, swap positions, etc.), and race to the finish. Features network-synced state, rubber-banding catch-up mechanics, and a mystery space system with slot-machine animations.

## Runtime Platform
- **Engine:** Unity (VRChat SDK)
- **Scripting:** UdonSharp (C# compiled to Udon VM — no interfaces, no abstract classes, no generics, no dictionaries)
- **Networking:** VRChat manual sync via `[UdonSynced]` variables + `RequestSerialization()` / `OnDeserialization()`

## Core Systems & File Locations

### Game Flow (Assets/Scripts/GameController/)
- **GameController_BoardGame.cs** — Central orchestrator. Handles game start/end, turn progression, dice validation, space effect processing (audio, movement, swap, miss turn, roll again). References `boardGameSpaceSettings` for space lookups via child index.
- **RollDiceHelper_BoardGame.cs** — Dice rolling logic (master only). Weighted rolls, rubber-banding (RerollForHugeLeader, RerollIfInLoop), mystery space detection, and the `ProcessLandingEffects()` pipeline. Contains `CalculateRoll()` entry point and `OnMysteryResolved()` callback.
- **RollDiceInteract_BoardGame.cs** — Player-facing dice click handler.

### Space Configuration (Assets/Scripts/CustomBoardScripts/)
- **SpaceSettings.cs** — UdonSharpBehaviour on each space. Boolean/int flags: `RollAgain`, `DrinkXTimes`, `SendBackToStart`, `EveryoneDrinkXTimes`, `MoveBackXSpaces`, `MoveForwardXSpaces`, `DrinkWhatYouRoll`, `SwapWithLast`, `SwapWithFirst`, `ImmuneFromDrinking`, `MissTurn`, `DrinkWithHost`, `ChooseSomeoneToDrink`, `GirlsDrink`, `GuysDrink`, `Finish`, `Start`, `IsMystery`.
- **MysteryManager.cs** — Handles mystery space resolution. Weighted random pick from pool, synced result, slot-machine animation (material + text cycling with cubic deceleration on the physical board space), then applies result to SpaceSettings and calls `RollDiceHelper.OnMysteryResolved()`.
- **ImageSettings.cs** — Material references for each space type (one per type + MysteryMat).
- **TextSettings.cs** — Display text templates with `{x}` placeholders for each space type.
- **BoardSettings.cs** — Legacy square-only board size enum (superseded by JSON system).

### Board Generation (Assets/Editor/)
- **UpdateBoard.cs** — Editor script. Primary entry point: `BoardGame > Generate Board From JSON` menu item. Reads a JSON board definition, clears existing spaces, generates Space + SpaceSetting objects in snake layout, applies materials/text, and bakes mystery pool data into MysteryManager. Supports non-square grids (columns x rows). Legacy `CreateBoard` and `UpdateBoardWithSettings` menu items preserved.

### Board Definitions (Assets/BoardGame/BoardDefinitions/)
- **ClassicBoard.json** — 7x7 traditional layout with fixed space types.
- **MysteryMadness.json** — 8x6 board, ~90% mystery spaces with weighted pool. Fixed landmarks: Start, Finish, one Roll Again, one Everyone Drinks, one Send Back to Start.
- JSON schema: `{ name, columns, rows, spaces[], mysteryPool[] }` where each space has `{ type, amount?, spaces? }` and pool entries add `weight`.

### Player Management (Assets/Scripts/PlayerScripts/)
- **PlayerList_BoardGame.cs** — Player ID list, status tracking (Connected/Left/Disconnected), name sync. Handles join/leave/rejoin.
- **PlayerFunctions_BoardGame.cs** — Add/remove player operations.

### Camera System (Assets/Scripts/CameraScripts/)
- **CameraFollowHead.cs** — Positions cameras at each player's head tracking data. Uses `SyncedHelper.IsReady()` guards.
- **UpdatePlayerCamerasOnSpace_BoardGame.cs** — Display panel camera management. Scales camera count tiers by player count (1-4, 5-9, 10-16, etc.).

### Audio (Assets/Scripts/AudioScripts/)
- **ToggleGameAudio_BoardGame.cs** — Audio toggle system. Each effect type has a synced increment counter that triggers audio playback on state change.

### UI (Assets/Scripts/UIScripts/)
- **UpdateSpaces.cs** — Board outline system. Red outline on previous player's space, blue on current. Uses children 8-11 of each space object.

### Special Features
- **AnyomBoard.cs** (GameController/) — Alternate board for player named "Anyom".
- **RunMikeyPlayer.cs** (Halloween/) — Jump scare creature that appears behind players.
- **RunSanta.cs** (MiscScripts/) — Decorative cycling Santa animation.

## Scene Hierarchy (Key Paths)
```
Root Board Object/
  Board/
    Spaces/           <- Visual space GameObjects (Space - 0, Space - 1, ...)
      Space - N/
        Canvas/
          Text        <- Space number
          Text (1)    <- Space description text
        SpaceImage    <- Renderer for space material/color
        [children 8-11] <- Outline objects
  CreateCustomBoard/
    Prefabs/
      Space           <- Space visual prefab (copied, not instantiated)
      SpaceSetting    <- SpaceSetting prefab (copied, not instantiated)
    Scripts/
      SpaceSettings/  <- Parent of all SpaceSetting instances (runtime lookup)
      BoardSettings   <- Legacy board size config
      ImageSettings   <- Material references
      TextSettings    <- Text templates
      MysteryManager  <- Mystery pool + animation logic
```

## Runtime Space Lookup
- `GameController.GetSpace(int index)` → `boardGameSpaceSettings.transform.GetChild(index).GetComponent<SpaceSettings>()`
- `GameController.IsEnd(int index)` → `boardGameSpaceSettings.transform.childCount - 1 == index`
- Space 0 is always Start, last space is always Finish.

## Network Sync Pattern
Master makes all game state decisions. Synced variables use increment counters (e.g., `ToggleDrink++`, `WinnerDetected++`) detected via `OnDeserialization()` or field change callbacks. DataLists serialized to JSON strings for array sync.
