---
paths:
  - "Assets/Editor/**/*.cs"
---

# Editor scripts rules

Code under `Assets/Editor/` runs in the Unity editor process, not in the Udon VM. **Standard C# rules apply** — LINQ, generics, `JsonUtility`, `EditorUtility`, `UnityEditor.*` all work. UdonSharp constraints (see `.claude/rules/udonsharp.md`) do **not** apply here.

These scripts shape the gameplay scene at edit time. Their output is committed (the scene file), so changes here have permanent effect on the project, not just runtime behavior.

## The board generation pipeline

`UpdateBoard.cs` is the canonical editor script. Entry point: menu item **`BoardGame → Generate Board From JSON`**.

Legacy menu items kept for back-compat (do not extend — modify the JSON path instead):

- `GameObject/3D Object/CreateBoard` — generates square boards from `BoardSettings.boardSize`.
- `GameObject/3D Object/UpdateBoardWithSettings` — re-applies materials/text to an existing board.

To run, the user must **select the root board GameObject** in the scene first. The script reads many things relative to that selection via `Find()` chains. Never break these paths:

```
<selected>/Board/Spaces                              <- spaces parent
<selected>/CreateCustomBoard/Prefabs/Space           <- visual prefab (copied)
<selected>/CreateCustomBoard/Prefabs/SpaceSetting    <- data prefab (copied)
<selected>/CreateCustomBoard/Scripts/SpaceSettings   <- data parent
<selected>/CreateCustomBoard/Scripts/TextSettings
<selected>/CreateCustomBoard/Scripts/ImageSettings
```

`MysteryManager` is found by `Object.FindObjectOfType<MysteryManager>()` (not by path) — there's a warning log if it's missing.

## The clipboard-copy pattern

Instead of `Instantiate`, `UpdateBoard` uses Unity's editor clipboard to duplicate the prefab objects:

```csharp
Selection.activeGameObject = spacePrefab;
Unsupported.CopyGameObjectsToPasteboard();
Unsupported.PasteGameObjectsFromPasteboard();
GameObject temp = Selection.activeGameObject;
```

This preserves UdonSharp serialization correctly across the copy. **Don't refactor this to `Instantiate`** — UdonBehaviour references break.

Before mutating the parent objects, register undo and dirty after:

```csharp
Undo.RegisterCompleteObjectUndo(spaceHeader, "Generate Board From JSON");
// ... mutate ...
EditorUtility.SetDirty(spaceHeader);
EditorUtility.SetDirty(ss);   // per-SpaceSettings after ApplyEffect
```

## Three methods every new space type must touch

When adding a space type, edits in `UpdateBoard.cs` go in these three methods:

1. **`ApplyEffect(SpaceSettings ss, string type, int amount, int spaces)`** — maps JSON type string to `SpaceSettings` flag. Called both from `ApplySpaceDefinition` and for every entry in `extras[]`, so adding here covers stacked usage too.
2. **`ReturnTextBasedOffSetting(SpaceSettings, TextSettings, int spaceNumber)`** — generates display text. Each flag adds to `normalText` with `" and "` concatenation, in priority order.
3. **`ReturnMaterialBasedOffSetting(SpaceSettings, ImageSettings)`** — returns the material. **Order matters**: earlier branches win when multiple flags are set on one space. Current visual priority: `Start → Mystery → Finish → SendBackToStart → RollAgain → EveryoneDrink → Drink → MoveBack → MoveForward → ...`. Place a new branch at the priority you want.

If the type can appear in `mysteryPool`, three more methods need updating — but those are in the runtime script `MysteryManager.cs`, not the editor. See `.claude/skills/new-board-space/SKILL.md` for the full checklist.

## The reset block

`ApplySpaceDefinition` resets **all** `SpaceSettings` flags at the top before applying. When you add a new property to `SpaceSettings`, add it to this reset block AND to the matching reset in `MysteryManager.ApplyMysteryResult()`. Forgetting causes spaces to inherit stale flags across regenerations or mystery resolutions.

## Other editor scripts

- `ChangeTextureSizes.cs` — bulk texture import settings tweaks. Standalone tool.
- `CreateMaterials.cs` — bulk material generation. Standalone tool.

These don't need to follow the same patterns as `UpdateBoard.cs` because they don't touch UdonBehaviours.
