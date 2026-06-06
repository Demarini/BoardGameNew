using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UdonSharpEditor;

// Script B of the choice-menu generators.
//
// Select your finished prototype "ChoiceButton" (the one with PlayerChoiceButton +
// OnClick wired), then run BoardGame > Populate Choice Menu Buttons. It clones the
// prototype up to TargetCount using the editor clipboard (the same pattern as
// UpdateBoard.cs, which preserves UdonBehaviour references that Instantiate breaks),
// then assigns the full pool into the PlayerChoiceMenu.buttons array.
//
// Re-running is safe: existing clones under the grid are removed first, the prototype
// is kept.
public static class PopulateChoiceMenuButtons
{
    const int TargetCount = 64;

    [MenuItem("BoardGame/Populate Choice Menu Buttons")]
    static void Populate()
    {
        GameObject prototype = Selection.activeGameObject;
        if (prototype == null)
        {
            Debug.LogError("[ChoiceMenu] Select the prototype ChoiceButton first.");
            return;
        }
        PlayerChoiceButton protoPcb = prototype.GetComponent<PlayerChoiceButton>();
        if (protoPcb == null)
        {
            Debug.LogError("[ChoiceMenu] Selected object has no PlayerChoiceButton component - finish wiring the prototype first.");
            return;
        }
        Transform gridT = prototype.transform.parent;
        if (gridT == null)
        {
            Debug.LogError("[ChoiceMenu] Prototype must be parented under the Grid.");
            return;
        }
        PlayerChoiceMenu menu = gridT.GetComponentInParent<PlayerChoiceMenu>();
        if (menu == null)
        {
            Debug.LogError("[ChoiceMenu] Could not find a PlayerChoiceMenu above the grid.");
            return;
        }

        Undo.RegisterFullObjectHierarchyUndo(gridT.gameObject, "Populate Choice Menu Buttons");

        // Remove any previously generated clones; keep the prototype.
        List<GameObject> stale = new List<GameObject>();
        foreach (Transform child in gridT)
        {
            if (child.gameObject != prototype) stale.Add(child.gameObject);
        }
        foreach (GameObject go in stale) Object.DestroyImmediate(go);

        PlayerChoiceButton[] buttons = new PlayerChoiceButton[TargetCount];
        buttons[0] = protoPcb;

        for (int i = 1; i < TargetCount; i++)
        {
            Selection.activeGameObject = prototype;
            Unsupported.CopyGameObjectsToPasteboard();
            Unsupported.PasteGameObjectsFromPasteboard();
            GameObject clone = Selection.activeGameObject;
            clone.transform.SetParent(gridT, false);
            clone.transform.SetAsLastSibling(); // paste inserts right after the prototype; force append order
            clone.name = "ChoiceButton (" + i + ")";
            buttons[i] = clone.GetComponent<PlayerChoiceButton>();
        }

        menu.buttons = buttons;
        UdonSharpEditorUtility.CopyProxyToUdon(menu);

        EditorUtility.SetDirty(menu.gameObject);
        EditorUtility.SetDirty(gridT.gameObject);
        Selection.activeGameObject = prototype;
        Debug.Log("[ChoiceMenu] Populated " + TargetCount + " buttons under '" + gridT.name + "' and assigned PlayerChoiceMenu.buttons.");
    }
}
