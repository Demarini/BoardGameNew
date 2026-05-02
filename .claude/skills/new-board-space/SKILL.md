---
description: Implement a new board space type for the VRChat board game. The user describes what the space should do in plain language and this skill provides all the context needed to implement it end-to-end.
arguments: [description]
---

The user wants to add a new board space type to the VRChat board game. Here is their description of what it should do:

**$ARGUMENTS**

Your job is to implement this space type end-to-end. Read the files listed below to understand current patterns, then make all necessary changes. Use the existing space types as reference for how effects are structured.

## Files to Read First
Read these to understand the current patterns before writing any code:
- `Assets/Scripts/CustomBoardScripts/SpaceSettings.cs` — All existing space properties
- `Assets/Scripts/GameController/GameController_BoardGame.cs` — How effects are processed (ProcessAudio, ProcessRollAgain, ProcessMissedTurn, ProcessSendBackToStart, ProcessLandedSpaceMovement, ProcessSwapWithPlayer)
- `Assets/Scripts/GameController/RollDiceHelper_BoardGame.cs` — The ProcessLandingEffects pipeline and priority order
- `Assets/Scripts/CustomBoardScripts/MysteryManager.cs` — How mystery results map to space types

## Files to Modify (in order)

### 1. SpaceSettings.cs
Add the new property. Use `bool` for toggle effects, `int` for effects with variable amounts.

### 2. ImageSettings.cs
Add a `public Material` field for the space's board icon. The user will assign the actual material in Unity.

### 3. TextSettings.cs
Add a `public string` field with default display text. Use `{x}` placeholder if the effect has a variable amount.

### 4. GameController_BoardGame.cs
Implement the actual game effect. Choose the right approach based on effect type:
- **Movement effects** → Add to or extend `ProcessLandedSpaceMovement()` / `ProcessSendBackToStart()`
- **Turn effects** → Add to `ProcessRollAgain()` / `ProcessMissedTurn()`
- **Drink effects** → Add to `ProcessAudio()` with a new toggle counter in GameVariables_BoardGame
- **Player targeting** → Add to `ProcessSwapWithPlayer()` or create a new Process method
- **New mechanics** → Create a new Process method and call it from RollDiceHelper.ProcessLandingEffects()

If the effect needs audio feedback, also add a synced `[UdonSynced] public int Toggle{Name}` counter to `GameVariables_BoardGame.cs`.

### 5. RollDiceHelper_BoardGame.cs
Wire the new Process method into `ProcessLandingEffects()` if it needs special handling in the movement/effect pipeline. Consider priority: SendBackToStart > Movement > Swap > other effects. Most simple effects (drink, turn) don't need changes here.

### 6. UpdateBoard.cs (Editor Script) — Three methods
- `ApplySpaceDefinition()` — Map the JSON type string to the SpaceSettings property
- `ReturnTextBasedOffSetting()` — Generate display text for the space
- `ReturnMaterialBasedOffSetting()` — Return the correct material (position by visual priority)

### 7. MysteryManager.cs — Three methods (if the space can appear as a mystery result)
- `ApplyMysteryResult()` — Map type string to SpaceSettings property + add to the reset block
- `GetTextForType()` — Return display text for the type string
- `GetMaterialForType()` — Return material for the type string

### 8. JSON Board Definitions
Add the new type to relevant board definitions in `Assets/BoardGame/BoardDefinitions/`. As a fixed space: `{ "type": "typeName" }`. As a mystery pool entry: `{ "type": "typeName", "weight": N }`. Reference weights for mystery pool: common effects ~15-20, uncommon ~3-5, rare ~1-2.

## Important Constraints
- UdonSharp: No interfaces, abstract classes, generics, or dictionaries. Use concrete types and if/else chains.
- Networking: All game state decisions happen on master. Use synced increment counters for cross-client events.
- Existing space types to reference as patterns: `SendBackToStart` (movement), `SwapWithFirst` (player targeting), `DrinkXTimes` (drink + audio), `MissTurn` (turn effect), `RollAgain` (turn effect).

After implementation, remind the user to:
1. Assign the new material in ImageSettings inspector
2. Add the type to their board JSON files
3. Regenerate the board via `BoardGame > Generate Board From JSON`
