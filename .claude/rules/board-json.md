---
paths:
  - "Assets/BoardGame/BoardDefinitions/**/*.json"
---

# Board JSON rules

This file loads when Claude is editing a board definition JSON. **After editing any of these files, the user must re-run `BoardGame → Generate Board From JSON` in the Unity editor for changes to take effect.** There is no auto-reload.

If you (Claude) just edited a board JSON, remind the user to regenerate before declaring the task done.

## Schema

```jsonc
{
  "name": "Display name only — not load-bearing",
  "columns": 7,                              // grid width (default 7 if 0/missing)
  "rows": 7,                                 // grid height (default: ceil(spaces.length / columns))
  "spaces": [
    { "type": "drink", "amount": 2 },                       // simple effect
    { "type": "moveForward", "spaces": 3 },                 // movement uses `spaces`, not `amount`
    { "type": "drink", "amount": 1, "text": "Take a sip" }, // optional `text` overrides default display
    { "type": "drink", "amount": 1, "extras": [             // stack additional effects on one space
      { "type": "sendBackToStart" }
    ]}
  ],
  "mysteryPool": [
    { "type": "drink",         "amount": 1, "weight": 7 },
    { "type": "everyoneDrink", "amount": 1, "weight": 13 }
  ]
}
```

## Field rules

- The space at **index 0 is forced to `Start`** and the **last index is forced to `Finish`** regardless of the `type` field there. Writing `{"type":"start"}` at index 0 is convention but not required.
- `amount` vs `spaces`: drink-type effects use `amount`; movement (`moveForward`, `moveBack`) uses `spaces`. In `mysteryPool`, `BakeMysteryPool` falls back to `spaces` when `amount` is 0, so for movement pool entries you can use either field.
- `weight` defaults to 1 if 0 or missing. Weights are summed live by `MysteryManager.PickWeightedRandom()` — no normalization, so use whatever total reads cleanly (the existing `MysteryMadness.json` sums to 100 for percentage readability).
- `extras[]` stacks additional effects on the same space. Visual: only the *primary* effect's material is shown; text is concatenated with " and ".
- `text` on a space overrides the auto-generated display text for that space only.

## Known type strings

`start`, `finish`, `mystery`, `drink`, `everyoneDrink`, `rollAgain`, `moveForward`, `moveBack`, `swapWithFirst`, `swapWithLast`, `missTurn`, `sendBackToStart`, `drinkWhatYouRoll`, `drinkWithHost`, `chooseSomeoneToDrink`, `girlsDrink`, `guysDrink`, `immuneFromDrinking`.

**Unknown type strings are silently ignored** by `ApplyEffect()` — no error, the space just has no effect. Double-check spelling before declaring success.

A type in `mysteryPool` must also be handled by `MysteryManager.ApplyMysteryResult()`, `GetTextForType()`, and `GetMaterialForType()`. Adding a type only to the JSON without those code changes is a silent no-op when rolled.

## Mystery pool weight conventions

Rough guidance from the current `MysteryMadness.json`:

- Common (drink 1, everyoneDrink 1): 7-13
- Uncommon (moveBack 3, swap, drinkWithHost): 3-5
- Rare (drink 5, sendBackToStart): 1-2
- Disabled: remove the entry (don't set `weight: 0` — it's bumped to 1 internally)

The total weight in `MysteryMadness.json` is 100 by convention so each entry's weight reads as a percent. Preserve this when rebalancing if you can.

## Visual layout

Spaces are laid out in a snake pattern by `UpdateBoard.cs`:
- Even rows (0, 2, 4...) flow left-to-right.
- Odd rows flow right-to-left.
- Spacing is `2.55f` units per step in both axes.

So `spaces[]` indexing follows the snake — `spaces[columns]` lands directly above `spaces[columns - 1]`, not above `spaces[0]`. Test new layouts visually after regenerating.
