using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UdonSharpEditor;

// Script A of the choice-menu generators.
//
// Select a GameObject that lives UNDER an existing world-space Canvas, then run
// BoardGame > Create Choice Menu. This builds the panel / title / grid / ONE plain
// starter button with stretch anchors (so it scales with the canvas), and adds +
// wires the PlayerChoiceMenu U# component.
//
// You then turn that single "ChoiceButton" into the real prototype (add
// PlayerChoiceButton, set its label + menu refs, wire OnClick -> SendCustomEvent
// "OnPressed"). Finally run BoardGame > Populate Choice Menu Buttons (Script B) to
// clone it up to 64.
public static class GenerateChoiceMenu
{
    [MenuItem("BoardGame/Create Choice Menu")]
    static void Create()
    {
        GameObject parent = Selection.activeGameObject;
        if (parent == null)
        {
            Debug.LogError("[ChoiceMenu] Select the GameObject (under a Canvas) to place the menu under, then run again.");
            return;
        }
        if (parent.GetComponentInParent<Canvas>() == null)
        {
            Debug.LogError("[ChoiceMenu] Selected object is not under a Canvas, so UI graphics can't render (this is the red-X / nothing-shows symptom). Select an object that lives under one of your existing world-space Canvases and run again.");
            return;
        }
        if (parent.transform.Find("ChoiceMenu") != null)
        {
            Debug.LogError("[ChoiceMenu] A 'ChoiceMenu' already exists under the selected object. Delete it first, then re-run.");
            return;
        }

        // Borrow a font from any existing Text in the scene; the builtin lookup can
        // return null in some Unity builds, leaving text invisible.
        Font font = null;
        Text anyText = Object.FindObjectOfType<Text>();
        if (anyText != null && anyText.font != null) font = anyText.font;
        if (font == null) font = (Font)Resources.GetBuiltinResource(typeof(Font), "LegacyRuntime.ttf");
        if (font == null) Debug.LogWarning("[ChoiceMenu] Could not find a font - assign one on the Text components manually.");

        // ---- Panel root (stretches to fill the Canvas, so you size the menu by sizing the Canvas) ----
        GameObject panel = NewUI("ChoiceMenu", parent.transform);
        Stretch(panel.GetComponent<RectTransform>(), 0f);
        Image panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0f, 0f, 0f, 0.85f);

        // ---- Timer bar (thin strip across the very top; drains during the choice) ----
        GameObject bar = NewUI("TimerBar", panel.transform);
        RectTransform barRt = bar.GetComponent<RectTransform>();
        barRt.anchorMin = new Vector2(0f, 1f);
        barRt.anchorMax = new Vector2(1f, 1f);
        barRt.pivot = new Vector2(0.5f, 1f);
        barRt.offsetMin = new Vector2(0f, -22f);
        barRt.offsetMax = new Vector2(0f, 0f);
        Image barImg = bar.AddComponent<Image>();
        barImg.sprite = (Sprite)AssetDatabase.GetBuiltinExtraResource(typeof(Sprite), "UI/Skin/UISprite.psd");
        barImg.type = Image.Type.Filled;
        barImg.fillMethod = Image.FillMethod.Horizontal;
        barImg.fillOrigin = (int)Image.OriginHorizontal.Left;
        barImg.fillAmount = 1f;
        barImg.color = new Color(0.85f, 0.3f, 0.3f, 1f);

        // ---- Title (anchored across the top, just below the timer bar) ----
        GameObject title = NewUI("Title", panel.transform);
        RectTransform titleRt = title.GetComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0f, 1f);
        titleRt.anchorMax = new Vector2(1f, 1f);
        titleRt.pivot = new Vector2(0.5f, 1f);
        titleRt.offsetMin = new Vector2(10f, -85f);
        titleRt.offsetMax = new Vector2(-10f, -28f);
        Text titleText = title.AddComponent<Text>();
        titleText.text = "Choose someone";
        titleText.font = font;
        titleText.fontSize = 36;
        titleText.color = Color.white;
        titleText.alignment = TextAnchor.MiddleCenter;

        // ---- Grid container (stretches to fill the panel below the title) ----
        GameObject gridGo = NewUI("Grid", panel.transform);
        RectTransform gridRt = gridGo.GetComponent<RectTransform>();
        gridRt.anchorMin = new Vector2(0f, 0f);
        gridRt.anchorMax = new Vector2(1f, 1f);
        gridRt.offsetMin = new Vector2(10f, 10f);
        gridRt.offsetMax = new Vector2(-10f, -90f); // leave headroom for the timer bar + title
        GridLayoutGroup grid = gridGo.AddComponent<GridLayoutGroup>();
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 8;
        grid.spacing = new Vector2(8f, 8f);
        grid.padding = new RectOffset(8, 8, 8, 8);
        grid.cellSize = new Vector2(120f, 60f); // runtime LayoutGrid overrides this

        // ---- One plain starter button (you make it the real prototype) ----
        GameObject btn = NewUI("ChoiceButton", gridGo.transform);
        Image btnImg = btn.AddComponent<Image>();
        btnImg.color = new Color(0.2f, 0.4f, 0.8f, 1f);
        Button button = btn.AddComponent<Button>();
        button.targetGraphic = btnImg;
        GameObject lbl = NewUI("Label", btn.transform);
        Stretch(lbl.GetComponent<RectTransform>(), 0f);
        Text lblText = lbl.AddComponent<Text>();
        lblText.text = "Player";
        lblText.font = font;
        lblText.color = Color.white;
        lblText.alignment = TextAnchor.MiddleCenter;

        // ---- Add + wire the PlayerChoiceMenu U# component ----
        PlayerChoiceMenu menu = UdonSharpUndo.AddComponent<PlayerChoiceMenu>(panel);
        menu.gameVariables = Object.FindObjectOfType<GameVariables_BoardGame>();
        menu.playerLists = Object.FindObjectOfType<PlayerList_BoardGame>();
        menu.playerChoiceRelay = Object.FindObjectOfType<PlayerChoiceRelay>();
        menu.menuRoot = panel;
        menu.gridRect = gridRt;
        menu.grid = grid;
        menu.titleText = titleText;
        menu.timerBar = barImg;
        menu.choiceTimeout = 30f;
        menu.cellSpacing = 8f;
        menu.gridPadding = 8f;
        UdonSharpEditorUtility.CopyProxyToUdon(menu);

        if (menu.gameVariables == null) Debug.LogWarning("[ChoiceMenu] No GameVariables_BoardGame in scene - assign it on the menu later.");
        if (menu.playerLists == null) Debug.LogWarning("[ChoiceMenu] No PlayerList_BoardGame in scene - assign it on the menu later.");
        if (menu.playerChoiceRelay == null) Debug.LogWarning("[ChoiceMenu] No PlayerChoiceRelay in scene yet - create that object and assign it on the menu before testing.");

        EditorUtility.SetDirty(panel);
        Selection.activeGameObject = btn;
        Debug.Log("[ChoiceMenu] Created. Next: on 'ChoiceButton' add PlayerChoiceButton (set its label + menu), wire the Button OnClick -> its UdonBehaviour SendCustomEvent \"OnPressed\", then run BoardGame > Populate Choice Menu Buttons.");
    }

    static GameObject NewUI(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    static void Stretch(RectTransform rt, float inset)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(inset, inset);
        rt.offsetMax = new Vector2(-inset, -inset);
    }
}
