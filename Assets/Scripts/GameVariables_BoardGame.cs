
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class GameVariables_BoardGame : UdonSharpBehaviour
{
    [SerializeField] HelperFunctions_BoardGame helperFunctions;
    [SerializeField] RunDiceTimer runDiceTimer;
    //[SerializeField] GameController_BoardGame gameController;
    [SerializeField] PlayerList_BoardGame playerLists;
    [SerializeField] UpdateSpaces updateSpaces;
    [SerializeField] CameraFollowHead cameraFollowHead;
    [SerializeField] UpdatePlayerCamerasOnSpace_BoardGame updatePlayerCamerasOnSpace;
    public bool ReceivedGameStartedValues;

    [UdonSynced, FieldChangeCallback(nameof(GameStarted))]
    public bool gameStarted;
    public bool GameStarted
    {
        set
        {
            gameStarted = value;
            Debug.Log("Game Started Updated");
            Debug.Log(gameStarted);
        }
        get => gameStarted;
    }
    [UdonSynced, FieldChangeCallback(nameof(TakePicture))]
    public int takePicture = 0;
    public int TakePicture
    {
        set
        {
            takePicture = value;
            cameraFollowHead.TakePicture();
            //gameController.CheckToUpdateDiceClickerInteract();
            Debug.Log("Take Picture Updated");
            Debug.Log(takePicture);
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
            Debug.Log("Current Player Updated");
            Debug.Log(currentPlayerIndex);
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
            Debug.Log("Previous Player Updated");
            Debug.Log(previousPlayerIndex);
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
            Debug.Log("Same Player Updated");
            Debug.Log(samePlayer);
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
            Debug.Log("Player Update Board Updated");
            Debug.Log(playerUpdateBoard);
        }
        get => playerUpdateBoard;
    }

    [UdonSynced, FieldChangeCallback(nameof(CurrentRoll))]
    public int currentRoll;
    public int CurrentRoll
    {
        set
        {
            currentRoll = value;
            Debug.Log("Current Roll Updated");
            Debug.Log(currentRoll);
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
            Debug.Log("Same Roll Updated");
            Debug.Log(sameRoll);
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
            Debug.Log("Missed Turns Updated");
            Debug.Log(missedTurnJson);
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
            Debug.Log("Player Space Updated");
            Debug.Log(playerSpaceJson);
        }
        get => playerSpaceJson;
    }
    public DataList playerSpaceDataList = new DataList();

    public override void OnPreSerialization()
    {
        Debug.Log("Preserialization Game Variables");
        MissedTurnJson = helperFunctions.SerializeDataList(missedTurnDataList, MissedTurnJson);
        PlayerSpaceJson = helperFunctions.SerializeDataList(playerSpaceDataList, PlayerSpaceJson);
    }
    public override void OnDeserialization()
    {
        Debug.Log("Deserialization Game Variables");
        missedTurnDataList = helperFunctions.DeserializeDataList(MissedTurnJson, missedTurnDataList);
        playerSpaceDataList = helperFunctions.DeserializeDataList(PlayerSpaceJson, playerSpaceDataList);
        Debug.Log("Current Player Index: " + currentPlayerIndex.ToString());
        Debug.Log("Previous Player Index: " + PreviousPlayerIndex.ToString());
        if (PlayerUpdateBoard != tmpPlayerUpdateBoard)
        {
            Debug.Log("Update Board");
            PostRollUpdates();
            tmpPlayerUpdateBoard = PlayerUpdateBoard;
        }
        CheckForGameStartedValueSync();
    }
    void PostRollUpdates()
    {
        updateSpaces.UpdateOutlineSpaces();
        updatePlayerCamerasOnSpace.UpdateDisplayPanelCameras();
        updatePlayerCamerasOnSpace.UpdatePlayerSpaces();
        updatePlayerCamerasOnSpace.previousSpaceToDisable = Convert.ToInt32(playerSpaceDataList[CurrentPlayerIndex].Double);
        updatePlayerCamerasOnSpace.previousPlayerToDisable = CurrentPlayerIndex;
        Debug.Log("Previous Space to Disable: " + updatePlayerCamerasOnSpace.previousSpaceToDisable.ToString());
        Debug.Log("Previous Player to Disable: " + updatePlayerCamerasOnSpace.previousPlayerToDisable.ToString());
        runDiceTimer.RunTimer = true;
    }
    void CheckForGameStartedValueSync()
    {
        if (GameStarted && !ReceivedGameStartedValues)
        {
            Debug.Log("Game Started and Received Game Variables For First Time");
            ReceivedGameStartedValues = true;
            if (ReceivedGameStartedValues && playerLists.ReceivedGameStartedValues)
            {
                Debug.Log("Received All Variables, Ready For Dice Check");
                updatePlayerCamerasOnSpace.UpdateCameraCountOnSpaces();
                runDiceTimer.RunTimer = true;
            }
            else
            {
                Debug.Log("Still Waiting For Player Variables");
            }
        }
    }
}