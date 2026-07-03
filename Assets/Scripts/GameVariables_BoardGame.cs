
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class GameVariables_BoardGame : UdonSharpBehaviour
{
    [SerializeField] GameController_BoardGame gameController;
    [SerializeField] ToggleGameAudio_BoardGame toggleGameAudio;
    [SerializeField] HelperFunctions_BoardGame helperFunctions;
    [SerializeField] RunDiceTimer runDiceTimer;
    //[SerializeField] GameController_BoardGame gameController;
    [SerializeField] PlayerList_BoardGame playerLists;
    [SerializeField] UpdateSpaces updateSpaces;
    [SerializeField] CameraFollowHead cameraFollowHead;
    [SerializeField] UpdatePlayerCamerasOnSpace_BoardGame updatePlayerCamerasOnSpace;
    [SerializeField] RollDiceHelper_BoardGame rollDiceHelper;
    [SerializeField] SpacePopupHUD spacePopupHUD;
    [SerializeField] PlayerChoiceMenu playerChoiceMenu;
    public GameObject winnerGameObject;
    public bool ReceivedAllVariables;
    public bool AwaitingPicture;
    public Animator rollDiceAnim;
    public Text playersInGameText;
    bool rollAnimationStart = false;
    float rollTimer = 0;
    bool sameRollDelay = false;
    float sameRollDelayTimer = 0;
    public bool ReceivedGameStartedValues;
    bool winnerCelebrationStarted = false;
    float winnerTimer = 0;
    public Text winnerText;
    public Text masterText;
    [UdonSynced, FieldChangeCallback(nameof(WinnerName))]
    public string winnerName = "";
    public string WinnerName
    {
        set
        {
            winnerName = value;
            winnerText.text = "Winner\n" + value;
        }
        get => winnerName;
    }
    [UdonSynced, FieldChangeCallback(nameof(MasterName))]
    public string masterName = "";
    public string MasterName
    {
        set
        {
            masterName = value;
            masterText.text = "Master: " + value;
        }
        get => masterName;
    }
    public int tmpToggleChooseSomeoneToDrink = 0;
    [UdonSynced, FieldChangeCallback(nameof(ToggleChooseSomeoneToDrink))]
    public int toggleChooseSomeoneToDrink = 0;
    public int ToggleChooseSomeoneToDrink
    {
        set
        {
            toggleChooseSomeoneToDrink = value;
        }
        get => toggleChooseSomeoneToDrink;
    }

    public int tmpToggleDrink = 0;
    [UdonSynced, FieldChangeCallback(nameof(ToggleDrink))]
    public int toggleDrink = 0;
    public int ToggleDrink
    {
        set
        {
            toggleDrink = value;
        }
        get => toggleDrink;
    }

    public int tmpToggleDrinkWithHost = 0;
    [UdonSynced, FieldChangeCallback(nameof(ToggleDrinkWithHost))]
    public int toggleDrinkWithHost = 0;
    public int ToggleDrinkWithHost
    {
        set
        {
            toggleDrinkWithHost = value;
        }
        get => toggleDrinkWithHost;
    }

    public int tmpToggleEveryoneDrink = 0;
    [UdonSynced, FieldChangeCallback(nameof(ToggleEveryoneDrink))]
    public int toggleEveryoneDrink = 0;
    public int ToggleEveryoneDrink
    {
        set
        {
            toggleEveryoneDrink = value;
        }
        get => toggleEveryoneDrink;
    }

    public int tmpToggleGirlsDrink = 0;
    [UdonSynced, FieldChangeCallback(nameof(ToggleGirlsDrink))]
    public int toggleGirlsDrink = 0;
    public int ToggleGirlsDrink
    {
        set
        {
            toggleGirlsDrink = value;
        }
        get => toggleGirlsDrink;
    }

    public int tmpToggleGuysDrink = 0;
    [UdonSynced, FieldChangeCallback(nameof(ToggleGuysDrink))]
    public int toggleGuysDrink = 0;
    public int ToggleGuysDrink
    {
        set
        {
            toggleGuysDrink = value;
        }
        get => toggleGuysDrink;
    }

    public int tmpToggleSendBackToStart = 0;
    [UdonSynced, FieldChangeCallback(nameof(ToggleSendBackToStart))]
    public int toggleSendBackToStart = 0;
    public int ToggleSendBackToStart
    {
        set
        {
            toggleSendBackToStart = value;
        }
        get => toggleSendBackToStart;
    }
    public int tmpToggleSwapWithFirst = 0;
    [UdonSynced, FieldChangeCallback(nameof(ToggleSwapWithFirst))]
    public int toggleSwapWithFirst = 0;
    public int ToggleSwapWithFirst
    {
        set
        {
            toggleSwapWithFirst = value;
        }
        get => toggleSwapWithFirst;
    }
    public int tmpToggleSwapWithLast = 0;
    [UdonSynced, FieldChangeCallback(nameof(ToggleSwapWithLast))]
    public int toggleSwapWithLast = 0;
    public int ToggleSwapWithLast
    {
        set
        {
            toggleSwapWithLast = value;
        }
        get => toggleSwapWithLast;
    }
    public int tmpToggleRollAgain = 0;
    [UdonSynced, FieldChangeCallback(nameof(ToggleRollAgain))]
    public int toggleRollAgain = 0;
    public int ToggleRollAgain
    {
        set
        {
            toggleRollAgain = value;
        }
        get => toggleRollAgain;
    }
    public int tmpToggleImmune = 0;
    [UdonSynced, FieldChangeCallback(nameof(ToggleImmune))]
    public int toggleImmune = 0;
    public int ToggleImmune
    {
        set
        {
            toggleImmune = value;
        }
        get => toggleImmune;
    }
    public int tmpToggleDrinkWhatYouRoll = 0;
    [UdonSynced, FieldChangeCallback(nameof(ToggleDrinkWhatYouRoll))]
    public int toggleDrinkWhatYouRoll = 0;
    public int ToggleDrinkWhatYouRoll
    {
        set
        {
            toggleDrinkWhatYouRoll = value;
        }
        get => toggleDrinkWhatYouRoll;
    }
    public int tmpToggleMissTurn = 0;
    [UdonSynced, FieldChangeCallback(nameof(ToggleMissTurn))]
    public int toggleMissTurn = 0;
    public int ToggleMissTurn
    {
        set
        {
            toggleMissTurn = value;
        }
        get => toggleMissTurn;
    }
    public int tmpToggleLeaderDrink = 0;
    [UdonSynced, FieldChangeCallback(nameof(ToggleLeaderDrink))]
    public int toggleLeaderDrink = 0;
    public int ToggleLeaderDrink
    {
        set
        {
            toggleLeaderDrink = value;
        }
        get => toggleLeaderDrink;
    }
    [UdonSynced, FieldChangeCallback(nameof(LeaderDrinkPlayerIndex))]
    public int leaderDrinkPlayerIndex = -1;
    public int LeaderDrinkPlayerIndex
    {
        set
        {
            leaderDrinkPlayerIndex = value;
        }
        get => leaderDrinkPlayerIndex;
    }

    // ---------------- Interactive "choose a player" support ----------------
    // Index of the player who must make a choice this turn (-1 = nobody).
    [UdonSynced, FieldChangeCallback(nameof(ChoosingPlayerIndex))]
    public int choosingPlayerIndex = -1;
    public int ChoosingPlayerIndex
    {
        set
        {
            choosingPlayerIndex = value;
        }
        get => choosingPlayerIndex;
    }
    // 0 = choose someone to drink, 1 = choose someone to swap with (future reuse).
    [UdonSynced, FieldChangeCallback(nameof(ChooseMode))]
    public int chooseMode = 0;
    public int ChooseMode
    {
        set
        {
            chooseMode = value;
        }
        get => chooseMode;
    }
    // Bumped by master to show/refresh/hide the chooser menu (one-shot counter).
    public int tmpChoosePromptIncrement = 0;
    [UdonSynced, FieldChangeCallback(nameof(ChoosePromptIncrement))]
    public int choosePromptIncrement = 0;
    public int ChoosePromptIncrement
    {
        set
        {
            choosePromptIncrement = value;
        }
        get => choosePromptIncrement;
    }
    // Player who was chosen to drink + audio toggle (mirrors LeaderDrink).
    [UdonSynced, FieldChangeCallback(nameof(ChosenDrinkPlayerIndex))]
    public int chosenDrinkPlayerIndex = -1;
    public int ChosenDrinkPlayerIndex
    {
        set
        {
            chosenDrinkPlayerIndex = value;
        }
        get => chosenDrinkPlayerIndex;
    }
    public int tmpToggleChosenDrink = 0;
    [UdonSynced, FieldChangeCallback(nameof(ToggleChosenDrink))]
    public int toggleChosenDrink = 0;
    public int ToggleChosenDrink
    {
        set
        {
            toggleChosenDrink = value;
        }
        get => toggleChosenDrink;
    }
    // Bumped to refresh piece positions WITHOUT starting the dice timer (used to show a
    // player on the space they landed before an interactive choice resolves).
    public int tmpBoardVisualRefresh = 0;
    [UdonSynced, FieldChangeCallback(nameof(BoardVisualRefresh))]
    public int boardVisualRefresh = 0;
    public int BoardVisualRefresh
    {
        set
        {
            boardVisualRefresh = value;
        }
        get => boardVisualRefresh;
    }
    public bool hasLoadedForFirstTime;

    [UdonSynced, FieldChangeCallback(nameof(GameStarted))]
    public bool gameStarted;
    public bool GameStarted
    {
        set
        {
            gameStarted = value;
            //Debug.Log("Game Started Updated");
            //Debug.Log(gameStarted);
        }
        get => gameStarted;
    }
    // Master-authoritative flag for the start-of-game intro (national anthem, etc.).
    // Master sets it true when the game starts and false when the intro window ends.
    // Every client (including late joiners) mirrors the intro object + dice block off THIS,
    // so a late joiner who arrives after it's already false does NOT replay the intro.
    [UdonSynced, FieldChangeCallback(nameof(GameStartIntroPlaying))]
    public bool gameStartIntroPlaying;
    public bool GameStartIntroPlaying
    {
        set
        {
            bool wasPlaying = gameStartIntroPlaying;
            gameStartIntroPlaying = value;
            if (gameController == null) return;
            if (value && !wasPlaying) gameController.EnableGameStartIntro();
            else if (!value && wasPlaying) gameController.DisableGameStartIntro();
        }
        get => gameStartIntroPlaying;
    }
    [UdonSynced, FieldChangeCallback(nameof(GameEnded))]
    public bool gameEnded;
    public bool GameEnded
    {
        set
        {
            gameEnded = value;
            //Debug.Log("Game Ended Updated");
            //Debug.Log(gameEnded);
        }
        get => gameEnded;
    }
    int tmpWinnerDetected;
    [UdonSynced, FieldChangeCallback(nameof(WinnerDetected))]
    public int winnerDetected = 0;
    public int WinnerDetected
    {
        set
        {
            winnerDetected = value;
            //Debug.Log("Game Ended Updated");
            //Debug.Log(gameEnded);
        }
        get => winnerDetected;
    }
    int tmpTakePicture = 0;
    [UdonSynced, FieldChangeCallback(nameof(TakePicture))]
    public int takePicture = 0;
    public int TakePicture
    {
        set
        {
            takePicture = value;
            cameraFollowHead.TakePicture();
            //gameController.CheckToUpdateDiceClickerInteract();
            //Debug.Log("Take Picture Updated");
            //Debug.Log(takePicture);
        }
        get => takePicture;
    }
    [UdonSynced, FieldChangeCallback(nameof(CurrentPlayerIndex))]
    public int currentPlayerIndex = -1;
    public int CurrentPlayerIndex
    {
        set
        {
            currentPlayerIndex = value;
            if (gameStarted && ReceivedGameStartedValues && playerLists.ReceivedGameStartedValues)
            {
                if (Networking.LocalPlayer.isMaster)
                {
                    //PostRollUpdates();
                }
            }
            //gameController.CheckToUpdateDiceClickerInteract();
            //Debug.Log("Current Player Updated");
            //Debug.Log(currentPlayerIndex);
        }
        get => currentPlayerIndex;
    }
    [UdonSynced, FieldChangeCallback(nameof(PreviousPlayerIndex))]
    public int previousPlayerIndex = -1;
    public int PreviousPlayerIndex
    {
        set
        {
            previousPlayerIndex = value;
            //runDiceTimer.RunTimer = true;
            //gameController.CheckToUpdateDiceClickerInteract();
            //Debug.Log("Previous Player Updated");
            //Debug.Log(previousPlayerIndex);
        }
        get => previousPlayerIndex;
    }
    [UdonSynced, FieldChangeCallback(nameof(SamePlayer))]
    public int samePlayer = 0;
    public int SamePlayer
    {
        set
        {
            samePlayer = value;
            if (gameStarted && ReceivedGameStartedValues && playerLists.ReceivedGameStartedValues)
            {
                if (Networking.LocalPlayer.isMaster)
                {
                    //PostRollUpdates();
                }
            }
            //Debug.Log("Same Player Updated");
            //Debug.Log(samePlayer);
        }
        get => samePlayer;
    }
    int tmpPlayerUpdateBoard = 0;
    [UdonSynced, FieldChangeCallback(nameof(PlayerUpdateBoard))]
    public int playerUpdateBoard = 0;
    public int PlayerUpdateBoard
    {
        set
        {
            playerUpdateBoard = value;
            if (gameStarted && ReceivedGameStartedValues && playerLists.ReceivedGameStartedValues && Networking.LocalPlayer.isMaster)
            {
                Debug.Log("Updating Board");
                PostRollUpdates();
            }
            //Debug.Log("Player Update Board Updated");
            //Debug.Log(playerUpdateBoard);
        }
        get => playerUpdateBoard;
    }

    [UdonSynced, FieldChangeCallback(nameof(CurrentRoll))]
    public int currentRoll = 0;
    public int CurrentRoll
    {
        set
        {
            currentRoll = value;
            TurnAnimsOff();
            //Debug.Log("Current Roll Updated");
            //Debug.Log(currentRoll);
        }
        get => currentRoll;
    }

    [UdonSynced, FieldChangeCallback(nameof(SameRoll))]
    public int sameRoll = 0;
    public int SameRoll
    {
        set
        {
            sameRoll = value;
            TurnAnimsOff();
            //Debug.Log("Same Roll Updated");
            //Debug.Log(sameRoll);
        }
        get => sameRoll;
    }

    [UdonSynced, FieldChangeCallback(nameof(MissedTurnJson))]
    public string missedTurnJson;
    public string MissedTurnJson
    {
        set
        {
            missedTurnJson = value;
            //Debug.Log("Missed Turns Updated");
            //Debug.Log(missedTurnJson);
        }
        get => missedTurnJson;
    }
    public DataList missedTurnDataList = new DataList();

    [UdonSynced, FieldChangeCallback(nameof(PlayerSpaceJson))]
    public string playerSpaceJson;
    public string PlayerSpaceJson
    {
        set
        {
            playerSpaceJson = value;
            //Debug.Log("Player Space Updated");
            //Debug.Log(playerSpaceJson);
        }
        get => playerSpaceJson;
    }
    public DataList playerSpaceDataList = new DataList();

    bool tmpMusicPlayerEnabled = false;
    [UdonSynced, FieldChangeCallback(nameof(MusicPlayerEnabled))]
    public bool musicPlayerEnabled = false;
    public bool MusicPlayerEnabled
    {
        set
        {
            musicPlayerEnabled = value;
        }
        get => musicPlayerEnabled;
    }
    string tmpSwapPlayerIndex = "";
    [UdonSynced, FieldChangeCallback(nameof(SwapPlayerIndex))]
    public string swapPlayerIndex = "";
    public string SwapPlayerIndex
    {
        set
        {
            swapPlayerIndex = value;
        }
        get => swapPlayerIndex;
    }
    int tmpSwapPlayerIndexSamePlayer = 0;
    [UdonSynced, FieldChangeCallback(nameof(SwapPlayerIndexSamePlayer))]
    public int swapPlayerindexSamePlayer = 0;
    public int SwapPlayerIndexSamePlayer
    {
        set
        {
            swapPlayerindexSamePlayer = value;
        }
        get => swapPlayerindexSamePlayer;
    }
    public int tmpSwapWithLastIncrement = 0;
    [UdonSynced, FieldChangeCallback(nameof(SwapWithLastIncrement))]
    public int swapWithLastIncrement = 0;
    public int SwapWithLastIncrement
    {
        set
        {
            swapWithLastIncrement = value;
        }
        get => swapWithLastIncrement;
    }
    public int tmpSwapWithFirstIncrement = 0;
    [UdonSynced, FieldChangeCallback(nameof(SwapWithFirstIncrement))]
    public int swapWithFirstIncrement = 0;
    public int SwapWithFirstIncrement
    {
        set
        {
            swapWithFirstIncrement = value;
        }
        get => swapWithFirstIncrement;
    }
    public int tmpPopupIncrement = 0;
    [UdonSynced, FieldChangeCallback(nameof(PopupIncrement))]
    public int popupIncrement = 0;
    public int PopupIncrement
    {
        set
        {
            popupIncrement = value;
        }
        get => popupIncrement;
    }
    [UdonSynced, FieldChangeCallback(nameof(PopupMessage))]
    public string popupMessage = "";
    public string PopupMessage
    {
        set
        {
            popupMessage = value;
        }
        get => popupMessage;
    }
    [UdonSynced, FieldChangeCallback(nameof(PopupTargetPlayerIndex))]
    public int popupTargetPlayerIndex = -1;
    public int PopupTargetPlayerIndex
    {
        set
        {
            popupTargetPlayerIndex = value;
        }
        get => popupTargetPlayerIndex;
    }
    // Player index to suppress this popup for (-1 = nobody). Lets "everyone except the
    // chooser" popups exist without the chooser seeing redundant text behind their menu.
    [UdonSynced, FieldChangeCallback(nameof(PopupExcludePlayerIndex))]
    public int popupExcludePlayerIndex = -1;
    public int PopupExcludePlayerIndex
    {
        set
        {
            popupExcludePlayerIndex = value;
        }
        get => popupExcludePlayerIndex;
    }

    public DataList gameLogDataList = new DataList();
    public int tmpGameLogIncrement = 0;
    [UdonSynced, FieldChangeCallback(nameof(GameLogIncrement))]
    public int gameLogIncrement = 0;
    public int GameLogIncrement
    {
        set
        {
            gameLogIncrement = value;
        }
        get => gameLogIncrement;
    }
    [UdonSynced, FieldChangeCallback(nameof(GameLogJson))]
    public string gameLogJson = "[]";
    public string GameLogJson
    {
        set
        {
            gameLogJson = value;
        }
        get => gameLogJson;
    }
    public const int GameLogMaxEntries = 30;
    public void LogEvent(string entry)
    {
        if (!Networking.LocalPlayer.isMaster) return;
        gameLogDataList.Add(entry);
        while (gameLogDataList.Count > GameLogMaxEntries)
        {
            gameLogDataList.RemoveAt(0);
        }
        GameLogJson = helperFunctions.SerializeDataList(gameLogDataList, GameLogJson);
        GameLogIncrement++;
    }
    public void Update()
    {

        if (Networking.LocalPlayer.isMaster)
        {
            if(MasterName != Networking.LocalPlayer.displayName)
            {
                MasterName = Networking.LocalPlayer.displayName;
                RequestSerialization();
                Debug.Log("Update Master Name");
            }
        }
        if (winnerCelebrationStarted)
        {
            if(winnerTimer > 360)
            {
                Debug.Log("Winner timer off");
                winnerTimer = 0;
                winnerCelebrationStarted = false;
                winnerGameObject.SetActive(false);
                gameController.TurnOnMusicPlayerVolume();
            }
            else
            {
                winnerTimer = winnerTimer + Time.deltaTime;
            }
        }
        if (rollAnimationStart)
        {
            if (rollTimer > 2)
            {
                //Debug.Log("Roll Timer Greater Than 2");
                rollDiceHelper.CalculateRoll();
                rollAnimationStart = false;
                rollTimer = 0;
            }
            else
            {
                rollTimer = rollTimer + Time.deltaTime;
            }
        }
        if (sameRollDelay)
        {
            if(sameRollDelayTimer > .1)
            {
                RollTheDiceAnim();
                sameRollDelay = false;
                sameRollDelayTimer = 0;
            }
            else
            {
                sameRollDelayTimer = sameRollDelayTimer + Time.deltaTime;
            }
        }
    }
    public override void OnPreSerialization()
    {
        //Debug.Log("Preserialization Game Variables");
        MissedTurnJson = helperFunctions.SerializeDataList(missedTurnDataList, MissedTurnJson);
        PlayerSpaceJson = helperFunctions.SerializeDataList(playerSpaceDataList, PlayerSpaceJson);
        GameLogJson = helperFunctions.SerializeDataList(gameLogDataList, GameLogJson);
        playerLists.UpdatePlayersInGameText();
        cameraFollowHead.TakePicture();
        toggleGameAudio.ToggleAudio();
        if (spacePopupHUD != null) spacePopupHUD.CheckPopup();
        if(tmpWinnerDetected != WinnerDetected && hasLoadedForFirstTime)
        {
            tmpWinnerDetected = WinnerDetected;
            DoWinnerActivities();
        }
        else
        {
            tmpWinnerDetected = WinnerDetected;
        }
        if (gameStarted)
        {
            winnerGameObject.SetActive(false);
        }
        if(tmpMusicPlayerEnabled != MusicPlayerEnabled)
        {
            if (MusicPlayerEnabled)
            {
                gameController.TurnOnMusicPlayer();
            }
            else
            {
                gameController.TurnOffMusicPlayer();
            }
        }
    }
    public override void OnDeserialization()
    {
        //Debug.Log("Deserialization Game Variables");
        missedTurnDataList = helperFunctions.DeserializeDataList(MissedTurnJson, missedTurnDataList);
        playerSpaceDataList = helperFunctions.DeserializeDataList(PlayerSpaceJson, playerSpaceDataList);
        gameLogDataList = helperFunctions.DeserializeDataList(GameLogJson, gameLogDataList);
        //Debug.Log("Current Player Index: " + currentPlayerIndex.ToString());
        //Debug.Log("Previous Player Index: " + PreviousPlayerIndex.ToString());
        if (PlayerUpdateBoard != tmpPlayerUpdateBoard)
        {
            //Debug.Log("Update Board");
            PostRollUpdates();
            tmpPlayerUpdateBoard = PlayerUpdateBoard;
        }
        if (BoardVisualRefresh != tmpBoardVisualRefresh)
        {
            tmpBoardVisualRefresh = BoardVisualRefresh;
            RefreshBoardVisualsOnly();
        }
        CheckForGameStartedValueSync();

        playerLists.UpdatePlayersInGameText();

        if (ReceivedAllVariables)
        {
            Debug.Log("Player Joined, game variables take picture");
            cameraFollowHead.TakePicture();
        }
        else
        {
            Debug.Log("Player Joined and is awaiting picture");
            AwaitingPicture = true;
        }
        if (!gameEnded)
        {
            toggleGameAudio.ToggleAudio();
            if (spacePopupHUD != null) spacePopupHUD.CheckPopup();
            if (playerChoiceMenu != null) playerChoiceMenu.CheckPrompt();
        }
        else
        {
            //updatePlayerCamerasOnSpace.ClearAllSpacesOfPictures();
            gameController.diceObjectInteract.SetActive(false);
            updateSpaces.ClearOutlineSpaces();
        }
        if (tmpWinnerDetected != WinnerDetected && hasLoadedForFirstTime)
        {
            tmpWinnerDetected = WinnerDetected;
            DoWinnerActivities();
        }
        else
        {
            tmpWinnerDetected = WinnerDetected;
        }
        if (gameStarted)
        {
            winnerGameObject.SetActive(false);
        }
        if (tmpMusicPlayerEnabled != MusicPlayerEnabled)
        {
            if (MusicPlayerEnabled)
            {
                gameController.TurnOnMusicPlayer();
            }
            else
            {
                gameController.TurnOffMusicPlayer();
            }
        }
    }
    public void DoWinnerActivities()
    {
        winnerCelebrationStarted = true;
        winnerGameObject.SetActive(true);
        gameController.TurnOffMusicPlayerVolume();
    }
    void RollTheDiceAnim()
    {
        //Debug.Log("Starting Roll Dice Anim");
        //Debug.Log("Turning Anim On: " + CurrentRoll.ToString());
        TurnCorrectAnimOn(CurrentRoll);
        rollAnimationStart = true;
    }
    void TurnAnimsOff()
    {
        rollDiceAnim.SetBool("RollOne", false);
        rollDiceAnim.SetBool("RollTwo", false);
        rollDiceAnim.SetBool("RollThree", false);
        rollDiceAnim.SetBool("RollFour", false);
        rollDiceAnim.SetBool("RollFive", false);
        rollDiceAnim.SetBool("RollSix", false);
        sameRollDelay = true;
        if (rollDiceHelper != null) rollDiceHelper.ResetMysteryVisual();
    }
    void TurnCorrectAnimOn(int diceRoll)
    {
        switch (diceRoll)
        {
            case 1:
                rollDiceAnim.SetBool("RollOne", true);
                break;
            case 2:
                rollDiceAnim.SetBool("RollTwo", true);
                break;
            case 3:
                rollDiceAnim.SetBool("RollThree", true);
                break;
            case 4:
                rollDiceAnim.SetBool("RollFour", true);
                break;
            case 5:
                rollDiceAnim.SetBool("RollFive", true);
                break;
            case 6:
                rollDiceAnim.SetBool("RollSix", true);
                break;
        }
        if (tmpMusicPlayerEnabled != MusicPlayerEnabled)
        {
            if (MusicPlayerEnabled)
            {
                gameController.TurnOnMusicPlayer();
            }
            else
            {
                gameController.TurnOffMusicPlayer();
            }
        }
    }
    void PostRollUpdates()
    {
        Debug.Log("Post Roll Updates");
        updateSpaces.UpdateOutlineSpaces();
        updatePlayerCamerasOnSpace.UpdateDisplayPanelCameras();
        updatePlayerCamerasOnSpace.UpdatePlayerSpaces();
        if(CurrentPlayerIndex != -1)
        {
            updatePlayerCamerasOnSpace.previousSpaceToDisable = Convert.ToInt32(playerSpaceDataList[CurrentPlayerIndex].ToString());
            updatePlayerCamerasOnSpace.previousPlayerToDisable = CurrentPlayerIndex;
        }
        //Debug.Log("Previous Space to Disable: " + updatePlayerCamerasOnSpace.previousSpaceToDisable.ToString());
        //Debug.Log("Previous Player to Disable: " + updatePlayerCamerasOnSpace.previousPlayerToDisable.ToString());
        runDiceTimer.RunTimer = true;
    }
    // Visual-only board refresh (piece positions + outlines + display cameras), WITHOUT
    // arming the dice timer -- so a player can be shown on the space they landed during an
    // interactive choice without the dice clicker being enabled mid-choice.
    //
    // UpdateOutlineSpaces keys off PreviousPlayerIndex, which during a choice still points at
    // the player BEFORE the roller (the turn hasn't advanced), so it would put the red outline
    // on the wrong space. ShowOutlineOnSpace highlights the roller's actual landing space
    // (red for everyone else, blue for the roller) -- which is what "where they landed" means.
    public void RefreshBoardVisualsOnly()
    {
        int rollerSpace = (CurrentPlayerIndex >= 0 && CurrentPlayerIndex < playerSpaceDataList.Count)
            ? Convert.ToInt32(playerSpaceDataList[CurrentPlayerIndex].ToString())
            : -1;
        updateSpaces.ShowOutlineOnSpace(rollerSpace);
        updatePlayerCamerasOnSpace.UpdateDisplayPanelCameras();
        updatePlayerCamerasOnSpace.UpdatePlayerSpaces();
    }
    void CheckForGameStartedValueSync()
    {
        if (GameStarted && !ReceivedGameStartedValues)
        {
            //Debug.Log("Game Started and Received Game Variables For First Time");
            ReceivedGameStartedValues = true;
            if (ReceivedGameStartedValues && playerLists.ReceivedGameStartedValues)
            {
                //Debug.Log("Received All Variables, Ready For Dice Check");
                updatePlayerCamerasOnSpace.UpdateCameraCountOnSpaces();
                runDiceTimer.RunTimer = true;
                ReceivedAllVariables = true;
            }
            else
            {
                //Debug.Log("Still Waiting For Player Variables");
            }
        }
    }
    void CheckToSeeIfSwapped()
    {
        if(tmpSwapPlayerIndex != SwapPlayerIndex)
        {
            //check to see if we are the player
            string[] playerSwapSplit = SwapPlayerIndex.Split('|');
            if (Networking.LocalPlayer.playerId.ToString() == playerSwapSplit[0])
            {
                //we are the player
                if (playerSwapSplit[1] == "Last")
                {
                    gameController.ToggleLastAudio();
                }
                else if (playerSwapSplit[1] == "First")
                {
                    gameController.ToggleFirstAudio();
                }
            }
            tmpSwapPlayerIndex = SwapPlayerIndex;
        }
        if (tmpSwapPlayerIndexSamePlayer != SwapPlayerIndexSamePlayer)
        {
            //check to see if we are the player
            string[] playerSwapSplit = SwapPlayerIndex.Split('|');
            if(playerSwapSplit.Length == 2)
            {
                if(Networking.LocalPlayer.playerId.ToString() == playerSwapSplit[0])
                {
                    //we are the player
                    if (playerSwapSplit[1] == "Last")
                    {
                        gameController.ToggleLastAudio();
                    }
                    else if(playerSwapSplit[1] == "First")
                    {
                        gameController.ToggleFirstAudio();
                    }
                }
            }
            tmpSwapPlayerIndexSamePlayer = SwapPlayerIndexSamePlayer;
        }
    }
}