---
description: Implement a new board space type for the VRChat board game. The user describes what the space should do in plain language and this skill provides all the context needed to implement it end-to-end.
arguments: [description]
---

The user wants to add a new board space type to the VRChat board game. Here is their description:

**$ARGUMENTS**

Your job is to implement this end-to-end. Read the files listed below first, then make all changes. Use existing space types as patterns.

## Before you start

If you haven't already, the rule files in `.claude/rules/` should be loaded automatically when you read the C# files below. Confirm you've seen:

- `udonsharp.md` — sync patterns, master-authoritative rule, the **mystery-space gotcha** (mystery resolution mutates `SpaceSettings` at runtime, including its reset block).
- `editor-scripts.md` — the three methods every new type touches in `UpdateBoard.cs`, and the reset-block rule.
- `board-json.md` — known type strings, weight conventions.

## Files to read first

Read these to understand current patterns before writing code:

- `Assets/Scripts/CustomBoardScripts/SpaceSettings.cs` — all existing space properties
- `Assets/Scripts/GameController/GameController_BoardGame.cs` — how effects are processed (`ProcessAudio`, `ProcessRollAgain`, `ProcessMissedTurn`, `ProcessSendBackToStart`, `ProcessLandedSpaceMovement`, `ProcessSwapWithPlayer`, `ProcessPopup`)
- `Assets/Scripts/GameController/RollDiceHelper_BoardGame.cs` — the `ProcessLandingEffects` pipeline and priority order (sendBackToStart > movement > swap > other; loops up to 10 iterations)
- `Assets/Scripts/CustomBoardScripts/MysteryManager.cs` — how mystery results map to space types

## Files to modify (in order)

### 1. `SpaceSettings.cs`
Add the new property. Use `bool` for toggles, `int` for variable amounts.

### 2. `ImageSettings.cs`
Add a `public Material` field for the board icon. The user will assign the actual material in the inspector.

### 3. `TextSettings.cs`
Add a `public string` field with default display text. Use `{x}` placeholder if the effect has a variable amount.

### 4. `GameController_BoardGame.cs`
Implement the actual effect. Pick the right `Process*` method based on type:

- **Movement** → extend `ProcessLandedSpaceMovement()` or `ProcessSendBackToStart()`
- **Turn effects** → `ProcessRollAgain()` or `ProcessMissedTurn()`
- **Drink + audio** → `ProcessAudio()` + add a synced `int Toggle{Name}` counter to `GameVariables_BoardGame.cs`
- **Player targeting** → `ProcessSwapWithPlayer()` / `ProcessSwapPlayer()`, or a new `Process*` method
- **New mechanic** → new `Process*` method, wired into `RollDiceHelper.ProcessLandingEffects()`

Also extend **`ProcessPopup()`** to add a popup message for the new effect. Use `target = -1` for global messages, or `target = gameVariables.CurrentPlayerIndex` to show only to the affected player.

### 5. `RollDiceHelper_BoardGame.cs`
Only modify `ProcessLandingEffects()` if the new effect interacts with the movement-loop priority (sendBackToStart > moveForward/Back > swap). Most simple effects (drink, turn-skip) don't need changes here.

### 6. `UpdateBoard.cs` (editor)
Three methods (see `.claude/rules/editor-scripts.md` for detail):

- `ApplyEffect()` — JSON `type` string → SpaceSettings property
- `ReturnTextBasedOffSetting()` — generate display text (uses `" and "` concatenation in priority order)
- `ReturnMaterialBasedOffSetting()` — return material (**order matters** — earlier branches win on multi-flag spaces)

Also add the new property to the **reset block** at the top of `ApplySpaceDefinition()`.

### 7. `MysteryManager.cs` — only if the type can appear in a mystery pool
Three methods:

- `ApplyMysteryResult()` — type string → SpaceSettings property mapping, AND add the property to the reset block above the if/else chain
- `GetTextForType()` — return display text for the type string
- `GetMaterialForType()` — return material for the type string

A type listed in a JSON `mysteryPool` but missing from these three is a **silent no-op** when rolled. Confirm before declaring done.

### 8. JSON board definitions
Add the new type to relevant files in `Assets/BoardGame/BoardDefinitions/`:

- As a fixed space: `{ "type": "newType" }` (or with `amount` / `spaces` / `text` / `extras[]` as needed)
- As a mystery pool entry: `{ "type": "newType", "weight": N }` (weight 7-13 common, 3-5 uncommon, 1-2 rare)

## Post-implementation checklist (tell the user before declaring done)

1. **Assign the new material** in the `ImageSettings` inspector on the scene's `CreateCustomBoard/Scripts/ImageSettings` object.
2. **Update at least one JSON board definition** (the one you tested with).
3. **Regenerate the board**: select the root board GameObject in the scene, then `BoardGame → Generate Board From JSON`. This is required for both fixed-space additions AND mystery pool changes — the pool is *baked* into `MysteryManager` at generation time.
4. **If the type appears in a mystery pool**: re-verify it actually fires by rolling onto a mystery in ClientSim. A type missing from `MysteryManager.ApplyMysteryResult()` will silently no-op.
5. **Test the popup message** — `ProcessPopup` is easy to forget, and the absence is silent.

## Patterns to reference

- `SendBackToStart` — movement reset effect
- `SwapWithFirst` / `SwapWithLast` — player targeting + sync counter for audio
- `DrinkXTimes` — drink + audio toggle counter
- `MissTurn` — turn effect via `missedTurnDataList`
- `RollAgain` — turn effect via `ProcessRollAgain` return value
- `MoveForwardXSpaces` / `MoveBackXSpaces` — recalculated landing space in the effect loop
