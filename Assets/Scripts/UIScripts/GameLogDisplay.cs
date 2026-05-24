
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VRC.SDK3.Data;
using VRC.SDKBase;

[UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
public class GameLogDisplay : UdonSharpBehaviour
{
    [SerializeField] GameVariables_BoardGame gameVariables;
    [SerializeField] TextMeshProUGUI logText;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] bool autoScrollToBottom = true;

    int lastSeenIncrement = -1;

    void Update()
    {
        if (gameVariables == null || logText == null) return;
        if (gameVariables.GameLogIncrement != lastSeenIncrement)
        {
            lastSeenIncrement = gameVariables.GameLogIncrement;
            RebuildText();
        }
    }

    void RebuildText()
    {
        DataList list = gameVariables.gameLogDataList;
        if (list == null || list.Count == 0)
        {
            logText.text = "";
            return;
        }

        string built = "";
        for (int i = 0; i < list.Count; i++)
        {
            if (i > 0) built = built + "\n";
            built = built + list[i].String;
        }
        logText.text = built;

        if (autoScrollToBottom && scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }
}
