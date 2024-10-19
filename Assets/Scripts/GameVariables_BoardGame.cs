
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class GameVariables_BoardGame : UdonSharpBehaviour
{
    [SerializeField] ToggleGameAudio_BoardGame toggleGameAudio;
    [SerializeField] HelperFunctions_BoardGame helperFunctions;
    [SerializeField] RunDiceTimer runDiceTimer;
    //[SerializeField] GameController_BoardGame gameController;
    [SerializeField] PlayerList_BoardGame playerLists;
    [SerializeField] UpdateSpaces updateSpaces;
    [SerializeField] CameraFollowHead cameraFollowHead;
    [SerializeField] UpdatePlayerCamerasOnSpace_BoardGame updatePlayerCamerasOnSpace;
    [SerializeField] RollDiceHelper_BoardGame rollDiceHelper;
    public bool ReceivedAllVariables;
    public bool AwaitingPicture;
    public Animator rollDiceAnim;
    public Text playersInGameText;
    bool rollAnimationStart = false;
    float rollTimer = 0;
    bool sameRollDelay = false;
    float sameRollDelayTimer = 0;
    public bool ReceivedGameStartedValues;

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

    public void Update()
    {
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
        playerLists.UpdatePlayersInGameText();
        cameraFollowHead.TakePicture();
        toggleGameAudio.ToggleAudio();
    }
    public override void OnDeserialization()
    {
        //Debug.Log("Deserialization Game Variables");
        missedTurnDataList = helperFunctions.DeserializeDataList(MissedTurnJson, missedTurnDataList);
        playerSpaceDataList = helperFunctions.DeserializeDataList(PlayerSpaceJson, playerSpaceDataList);
        //Debug.Log("Current Player Index: " + currentPlayerIndex.ToString());
        //Debug.Log("Previous Player Index: " + PreviousPlayerIndex.ToString());
        if (PlayerUpdateBoard != tmpPlayerUpdateBoard)
        {
            //Debug.Log("Update Board");
            PostRollUpdates();
            tmpPlayerUpdateBoard = PlayerUpdateBoard;
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
        }
        else
        {
            updatePlayerCamerasOnSpace.ClearAllSpacesOfPictures();
            updateSpaces.ClearOutlineSpaces();
        }
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
    }
    void PostRollUpdates()
    {
        Debug.Log("Post Roll Updates");
        updateSpaces.UpdateOutlineSpaces();
        updatePlayerCamerasOnSpace.UpdateDisplayPanelCameras();
        updatePlayerCamerasOnSpace.UpdatePlayerSpaces();
        updatePlayerCamerasOnSpace.previousSpaceToDisable = Convert.ToInt32(playerSpaceDataList[CurrentPlayerIndex].Double);
        updatePlayerCamerasOnSpace.previousPlayerToDisable = CurrentPlayerIndex;
        //Debug.Log("Previous Space to Disable: " + updatePlayerCamerasOnSpace.previousSpaceToDisable.ToString());
        //Debug.Log("Previous Player to Disable: " + updatePlayerCamerasOnSpace.previousPlayerToDisable.ToString());
        runDiceTimer.RunTimer = true;
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
}