﻿
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

    [UdonSynced, FieldChangeCallback(nameof(CurrentPlayerIndex))]
    public int currentPlayerIndex = -1;
    public int CurrentPlayerIndex
    {
        set
        {
            currentPlayerIndex = value;
            if(gameStarted && ReceivedGameStartedValues && playerLists.ReceivedGameStartedValues)
            {
                runDiceTimer.RunTimer = true;
            }
           //gameController.CheckToUpdateDiceClickerInteract();
            Debug.Log("Current Player Updated");
            Debug.Log(currentPlayerIndex);
        }
        get => currentPlayerIndex;
    }
    int tmpPreviousPlayerIndex = -1;
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
        if(tmpPreviousPlayerIndex != PreviousPlayerIndex)
        {
            updateSpaces.UpdateOutlineSpaces();
            tmpPreviousPlayerIndex = PreviousPlayerIndex;
        }
        CheckForGameStartedValueSync();
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
                runDiceTimer.RunTimer = true;
            }
            else
            {
                Debug.Log("Still Waiting For Player Variables");
            }
        }
    }
}