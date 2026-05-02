
using UdonSharp;
using UnityEngine;
using TMPro;
using VRC.SDKBase;

[UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
public class SpacePopupHUD : UdonSharpBehaviour
{
    [SerializeField] GameVariables_BoardGame gameVariables;
    [SerializeField] PlayerList_BoardGame playerLists;

    public CanvasGroup canvasGroup;
    public TextMeshProUGUI popupText;

    public float fadeInDuration = 0.3f;
    public float displayDuration = 2.5f;
    public float fadeOutDuration = 0.5f;
    public float distanceFromHead = 1.5f;
    public float verticalOffset = -0.1f;

    bool isShowing = false;
    bool isPersistent = false;
    float showTimer = 0f;
    bool popupHasLoaded = false;

    void Start()
    {
        if (canvasGroup != null) canvasGroup.alpha = 0f;
    }

    void LateUpdate()
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;
        if (localPlayer == null || canvasGroup == null) return;

        VRCPlayerApi.TrackingData headData = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
        Vector3 forward = headData.rotation * Vector3.forward;
        Vector3 up = headData.rotation * Vector3.up;

        transform.position = headData.position + forward * distanceFromHead + up * verticalOffset;
        transform.rotation = headData.rotation * Quaternion.Euler(0, 180, 0);

        if (isShowing)
        {
            showTimer += Time.deltaTime;
            float totalDuration = fadeInDuration + displayDuration + fadeOutDuration;

            if (showTimer < fadeInDuration)
            {
                canvasGroup.alpha = showTimer / fadeInDuration;
            }
            else if (isPersistent)
            {
                canvasGroup.alpha = 1f;
            }
            else if (showTimer < fadeInDuration + displayDuration)
            {
                canvasGroup.alpha = 1f;
            }
            else if (showTimer < totalDuration)
            {
                canvasGroup.alpha = 1f - (showTimer - fadeInDuration - displayDuration) / fadeOutDuration;
            }
            else
            {
                canvasGroup.alpha = 0f;
                isShowing = false;
            }
        }
    }

    public void CheckPopup()
    {
        Debug.Log($"[PopupHUD] CheckPopup called. popupHasLoaded={popupHasLoaded}, tmp={gameVariables.tmpPopupIncrement}, current={gameVariables.PopupIncrement}");
        if (!popupHasLoaded)
        {
            popupHasLoaded = true;
            gameVariables.tmpPopupIncrement = gameVariables.PopupIncrement;
            return;
        }

        if (gameVariables.tmpPopupIncrement != gameVariables.PopupIncrement)
        {
            gameVariables.tmpPopupIncrement = gameVariables.PopupIncrement;

            int target = gameVariables.PopupTargetPlayerIndex;
            Debug.Log($"[PopupHUD] New popup! msg='{gameVariables.PopupMessage}', target={target}, selfIndex={playerLists.selfIndex}");
            if (target == -1 || target == playerLists.selfIndex)
            {
                ShowPopup(gameVariables.PopupMessage);
            }
            else
            {
                Debug.Log($"[PopupHUD] Skipped — not relevant to local player");
            }
        }
    }

    public void ShowPopup(string message)
    {
        Debug.Log($"[PopupHUD] ShowPopup: '{message}', canvasGroup={(canvasGroup != null ? "OK" : "NULL")}, popupText={(popupText != null ? "OK" : "NULL")}");
        if (popupText != null) popupText.text = message;
        isPersistent = false;
        showTimer = 0f;
        isShowing = true;
    }

    public void ShowPersistentPopup(string message)
    {
        if (popupText != null) popupText.text = message;
        isPersistent = true;
        showTimer = 0f;
        isShowing = true;
    }

    public void DismissPopup()
    {
        isPersistent = false;
        showTimer = fadeInDuration + displayDuration;
    }
}
