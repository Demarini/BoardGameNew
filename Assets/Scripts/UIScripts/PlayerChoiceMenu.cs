
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

// Local on-screen menu shown to the current player when they land on an interactive
// "choose a player" space. It never touches game state directly -- it only reads the
// broadcast (ChoosingPlayerIndex / ChoosePromptIncrement) to decide whether to show,
// and routes the pick through PlayerChoiceRelay (which the chooser already owns).
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class PlayerChoiceMenu : UdonSharpBehaviour
{
    // Public so the editor generators (GenerateChoiceMenu / PopulateChoiceMenuButtons) can wire them.
    public GameVariables_BoardGame gameVariables;
    public PlayerList_BoardGame playerLists;
    public PlayerChoiceRelay playerChoiceRelay;

    public GameObject menuRoot;            // panel to show/hide
    public RectTransform gridRect;         // RectTransform the grid lives in
    public GridLayoutGroup grid;           // Constraint = Fixed Column Count
    public Text titleText;
    public PlayerChoiceButton[] buttons;   // pool, one per possible slot (size 64)

    // Mirror the GridLayoutGroup's spacing/padding here (avoids reading RectOffset in Udon).
    public float cellSpacing = 8f;
    public float gridPadding = 8f;

    // Optional head placement: the world-space Canvas root to drop in front of the chooser
    // when the menu opens. Left null = no repositioning (menu stays wherever it's placed).
    // Placed ONCE on open (not per-frame) so the buttons don't dodge your pointer.
    public Transform canvasToPosition;
    public float distanceFromHead = 1.2f;
    public float verticalOffset = -0.2f;
    public float minHeight = 1.0f;        // floor: menu center never drops below this (small avatars)

    // While the menu is hidden, the canvas (and its VRCUiShape collider) is parked here --
    // far off-map -- so players don't run into an invisible wall left where the menu last
    // opened. ShowMenu moves it to the head; HideMenu/Start move it back here.
    public Vector3 parkedPosition = new Vector3(0f, -1000f, 0f);

    // Optional countdown bar: a Filled Image that drains 1 -> 0 over choiceTimeout. Purely
    // local/visual -- master's RollDiceHelper.choiceTimeout is what actually picks. Keep
    // choiceTimeout here matching that value.
    public Image timerBar;
    public float choiceTimeout = 30f;

    bool promptLoaded = false;
    int activeButtonCount = 0;
    float choiceBarTimer = 0f;

    void Start()
    {
        if (menuRoot != null) menuRoot.SetActive(false);
        ParkCanvas();
    }

    // While the menu is open, keep it live: rebuild the eligible-player buttons (so
    // joins/leaves/disconnects mid-choice are reflected) and re-fit the grid (so panel/
    // canvas resizing is reflected). The UI setters self-guard on no-change, so doing
    // this every frame is cheap.
    void Update()
    {
        if (menuRoot != null && menuRoot.activeSelf)
        {
            PopulateButtons();
            if (activeButtonCount > 0) LayoutGrid(activeButtonCount);
            if (timerBar != null)
            {
                choiceBarTimer += Time.deltaTime;
                float remaining = 1f - choiceBarTimer / choiceTimeout;
                timerBar.fillAmount = remaining < 0f ? 0f : remaining;
            }
        }
    }

    // Master-local entry: master never deserializes its own write, so StartChoosePrompt
    // calls this directly. Bypasses the late-join stale-guard since this is always fresh.
    public void ShowForLocalChooser()
    {
        promptLoaded = true;
        gameVariables.tmpChoosePromptIncrement = gameVariables.ChoosePromptIncrement;
        if (gameVariables.ChoosingPlayerIndex >= 0
            && gameVariables.ChoosingPlayerIndex == playerLists.selfIndex)
        {
            ShowMenu();
        }
        else
        {
            HideMenu();
        }
    }

    // Called from GameVariables.OnDeserialization on remote clients.
    public void CheckPrompt()
    {
        // Don't act on a stale increment the first time we see it (e.g. on late join).
        if (!promptLoaded)
        {
            promptLoaded = true;
            gameVariables.tmpChoosePromptIncrement = gameVariables.ChoosePromptIncrement;
            Debug.Log($"[ChoiceMenu] CheckPrompt first-load swallow (tmp synced to {gameVariables.ChoosePromptIncrement})");
            return;
        }
        if (gameVariables.tmpChoosePromptIncrement == gameVariables.ChoosePromptIncrement) return;
        gameVariables.tmpChoosePromptIncrement = gameVariables.ChoosePromptIncrement;

        Debug.Log($"[ChoiceMenu] CheckPrompt fired: choosingIdx={gameVariables.ChoosingPlayerIndex}, selfIndex={playerLists.selfIndex}");
        if (gameVariables.ChoosingPlayerIndex >= 0
            && gameVariables.ChoosingPlayerIndex == playerLists.selfIndex)
        {
            ShowMenu();
        }
        else
        {
            HideMenu();
        }
    }

    void ShowMenu()
    {
        Debug.Log("[ChoiceMenu] ShowMenu entered");
        // Activate FIRST so a later throw in PopulateButtons/LayoutGrid can't leave the
        // menu silently hidden.
        if (menuRoot != null) menuRoot.SetActive(true);

        choiceBarTimer = 0f;
        if (timerBar != null) timerBar.fillAmount = 1f;

        if (titleText != null)
        {
            titleText.text = gameVariables.ChooseMode == 1
                ? "Choose someone to swap with"
                : "Choose someone to drink";
        }

        PopulateButtons();
        if (activeButtonCount > 0) LayoutGrid(activeButtonCount);
        PositionInFrontOfHead();
        Debug.Log($"[ChoiceMenu] ShowMenu done, activeButtons={activeButtonCount}");
    }

    // Drop the menu's canvas in front of the local head, facing the player. Called once
    // when the menu opens; move this into Update() if you want continuous head-follow.
    void PositionInFrontOfHead()
    {
        if (canvasToPosition == null) return;
        VRCPlayerApi lp = Networking.LocalPlayer;
        if (lp == null) return;
        VRCPlayerApi.TrackingData head = lp.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);

        // Horizontal forward only, so head pitch doesn't shove the menu up/down or onto the player.
        Vector3 forward = head.rotation * Vector3.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.001f) forward = Vector3.forward; // looking straight up/down
        forward = forward.normalized;

        Vector3 pos = head.position + forward * distanceFromHead;
        pos.y = head.position.y + verticalOffset;   // height tracks avatar...
        if (pos.y < minHeight) pos.y = minHeight;    // ...but never below the floor
        canvasToPosition.position = pos;

        // Face the player, upright (don't inherit head pitch/roll). Canvas forward points
        // the same way the player looks (away from them), which renders the UI right-way-round.
        canvasToPosition.rotation = Quaternion.LookRotation(forward, Vector3.up);
    }

    // Show one button per eligible target: in the game (status Connected == 0) and not
    // self. Disconnected (2) and LeftGame (1) players are filtered out. Re-runnable every
    // frame -- the SetActive/text setters no-op when nothing changed.
    void PopulateButtons()
    {
        int playerCount = playerLists.playerNamesInGameDataList.Count;
        int activeCount = 0;
        for (int i = 0; i < buttons.Length; i++)
        {
            PlayerChoiceButton btn = buttons[i];
            if (btn == null) continue;
            bool eligible = i < playerCount
                && i != playerLists.selfIndex
                && Convert.ToInt32(playerLists.playerStatusInGameDataList[i].ToString()) == 0;
            if (eligible)
            {
                btn.slotIndex = i;
                if (btn.label != null) btn.label.text = playerLists.playerNamesInGameDataList[i].String;
                if (!btn.gameObject.activeSelf) btn.gameObject.SetActive(true);
                activeCount++;
            }
            else
            {
                if (btn.gameObject.activeSelf) btn.gameObject.SetActive(false);
            }
        }
        activeButtonCount = activeCount;
    }

    public void HideMenu()
    {
        if (menuRoot != null) menuRoot.SetActive(false);
        ParkCanvas();
    }

    // Stash the canvas (and its collider) far off-map so nothing runs into it while hidden.
    void ParkCanvas()
    {
        if (canvasToPosition != null) canvasToPosition.position = parkedPosition;
    }

    // Called by a PlayerChoiceButton's OnClick.
    public void OnSlotChosen(int playerIndex)
    {
        if (gameVariables.ChoosingPlayerIndex != playerLists.selfIndex) return; // only the chooser
        if (playerIndex < 0) return;
        playerChoiceRelay.SubmitChoice(playerIndex);
        HideMenu();
    }

    // Size cells so all `count` buttons fill the panel with no scrolling. Chooses a
    // column count whose grid best matches the panel's aspect ratio.
    void LayoutGrid(int count)
    {
        if (grid == null || gridRect == null || count <= 0) return;

        float availW = gridRect.rect.width - gridPadding * 2f;
        float availH = gridRect.rect.height - gridPadding * 2f;
        if (availH < 1f) availH = 1f;

        int cols = Mathf.Clamp(Mathf.RoundToInt(Mathf.Sqrt(count * (availW / availH))), 1, count);
        int rows = Mathf.CeilToInt((float)count / cols);

        float cellW = (availW - cellSpacing * (cols - 1)) / cols;
        float cellH = (availH - cellSpacing * (rows - 1)) / rows;
        if (cellW < 1f) cellW = 1f;
        if (cellH < 1f) cellH = 1f;

        grid.constraintCount = cols;
        grid.cellSize = new Vector2(cellW, cellH);
    }
}
