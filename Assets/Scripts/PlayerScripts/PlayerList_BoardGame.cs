
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class PlayerList_BoardGame : UdonSharpBehaviour
{
    [SerializeField] HelperFunctions_BoardGame helperFunctions;
    [SerializeField] GameVariables_BoardGame gameVariables;
    [SerializeField] GameController_BoardGame gameController;
    [SerializeField] RunDiceTimer runDiceTimer;
    [SerializeField] UpdatePlayerCamerasOnSpace_BoardGame updatePlayerCamerasOnSpace;
    [SerializeField] CameraFollowHead cameraFollowHead;
    public bool ReceivedGameStartedValues;
    public int selfIndex = -1;
    int previousPlayerCount = 0;
    public Text playersInGameText;
    [UdonSynced, FieldChangeCallback(nameof(Shuffled))]
    public bool shuffled;
    public bool Shuffled
    {
        set
        {
            shuffled = value;
            //Debug.Log("Shuffled Updated");
            //Debug.Log(shuffled);
        }
        get => shuffled;
    }

    [UdonSynced, FieldChangeCallback(nameof(PlayersInGameJson))]
    private string playersInGameJson;
    public string PlayersInGameJson
    {
        set
        {
            playersInGameJson = value;
            //Debug.Log("Players Updated");
            //Debug.Log(playersInGameJson);
        }
        get => playersInGameJson;
    }
    public DataList playersInGameDataList = new DataList();

    [UdonSynced, FieldChangeCallback(nameof(PlayerStatusInGameJson))]
    private string playerStatusInGameJson;
    public string PlayerStatusInGameJson
    {
        set
        {
            playerStatusInGameJson = value;
            //Debug.Log("Player Status Updated");
            //Debug.Log(playerStatusInGameJson);
        }
        get => playerStatusInGameJson;
    }
    public DataList playerStatusInGameDataList = new DataList();

    [UdonSynced, FieldChangeCallback(nameof(PlayerNamesInGameJson))]
    private string playerNamesInGameJson;
    public string PlayerNamesInGameJson
    {
        set
        {
            playerNamesInGameJson = value;
            //Debug.Log("Player Names Updated");
            //Debug.Log(playerNamesInGameJson);
        }
        get => playerNamesInGameJson;
    }
    public DataList playerNamesInGameDataList = new DataList();


    public override void OnPreSerialization()
    {
        //Debug.Log("Preserialization Player Lists");
        PlayersInGameJson = helperFunctions.SerializeDataList(playersInGameDataList, PlayersInGameJson);

        PlayerStatusInGameJson = helperFunctions.SerializeDataList(playerStatusInGameDataList, PlayerStatusInGameJson);

        PlayerNamesInGameJson = helperFunctions.SerializeDataList(playerNamesInGameDataList, PlayerNamesInGameJson);
        
        UpdatePlayersInGameText();
    }
    public override void OnDeserialization()
    {
        //Debug.Log("Deserialization Player Lists");
        playersInGameDataList = helperFunctions.DeserializeDataList(PlayersInGameJson, playersInGameDataList);
        if(previousPlayerCount != playersInGameDataList.Count)
        {
            updatePlayerCamerasOnSpace.UpdateCameraCountOnSpaces();
        }
        previousPlayerCount = playersInGameDataList.Count;

        playerStatusInGameDataList = helperFunctions.DeserializeDataList(PlayerStatusInGameJson, playerStatusInGameDataList);

        playerNamesInGameDataList = helperFunctions.DeserializeDataList(PlayerNamesInGameJson, playerNamesInGameDataList);
        //if (gameVariables.GameStarted)
        //{
        //    int indexToUpdate = updatePlayerCamerasOnSpace.GetIndexToUpdate();
        //}
        
        CheckForGameStartedValueSync();

        UpdatePlayersInGameText();
        
        if (gameVariables.AwaitingPicture)
        {
            Debug.Log("Was awaiting picture, taking pictures now");
            cameraFollowHead.TakePicture();
            gameVariables.AwaitingPicture = false;
        }
        GetSelfIndex();
    }
    public void GetSelfIndex()
    {
        for (int i = 0; i < playersInGameDataList.Count; i++)
        {
            if (Networking.LocalPlayer.playerId == playersInGameDataList[i])
            {
                Debug.Log("Found Self Index: " + i.ToString());
                selfIndex = i;
            }
        }
    }
     void CheckForGameStartedValueSync()
    {
        if (Shuffled && !ReceivedGameStartedValues)
        {
            //Debug.Log("Game Started and Received Player Variables For First Time");
            ReceivedGameStartedValues = true;
            if(selfIndex != -1)
            {
                GetSelfIndex();
            }
            if (ReceivedGameStartedValues && gameVariables.ReceivedGameStartedValues)
            {
                //Debug.Log("Received All Variables, Ready For Dice Check");
                updatePlayerCamerasOnSpace.UpdateCameraCountOnSpaces();
                runDiceTimer.RunTimer = true;
                gameVariables.ReceivedAllVariables = true;
            }
            else
            {
                //Debug.Log("Still Waiting For Game Variables");
            }
        }
    }
    public void UpdatePlayersInGameText()
    {
        playersInGameText.text = "";
        for (int i = 0; i < playersInGameDataList.Count; i++)
        {
            string playerStatus = "";
            if (gameVariables.gameStarted && playerStatusInGameDataList.Count > 0)
            {
                switch (Convert.ToInt32(playerStatusInGameDataList[i].ToString()))
                {
                    case (int)PlayerInGameStatus.Connected:
                        playerStatus = "Connected";
                        break;
                    case (int)PlayerInGameStatus.LeftGame:
                        playerStatus = "Left Game";
                        break;
                    case (int)PlayerInGameStatus.Disconnected:
                        playerStatus = "Disconnected";
                        break;
                }
            }
            VRCPlayerApi player = VRCPlayerApi.GetPlayerById(Convert.ToInt32(playersInGameDataList[i].ToString()));
            string displayName = player == null ? "" : player.displayName;
            string textToAdd = (gameVariables.gameStarted && playerStatusInGameDataList.Count > 0) ? playerNamesInGameDataList[i].ToString() : displayName;
            if (gameVariables.playerSpaceDataList.Count > 0)
            {
                
                textToAdd = textToAdd + ((gameVariables.gameStarted && playerStatusInGameDataList.Count > 0) ? " [" + gameVariables.playerSpaceDataList[i].ToString() + "] [" + playerStatus + "]" : "") + "\n";
            }
            else
            {
                textToAdd = textToAdd + "\n";
            }
            playersInGameText.text = playersInGameText.text + textToAdd;
            playerStatus = "";
        }
    }
    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (Networking.LocalPlayer.isMaster)
        {
            if (gameVariables.GameStarted)
            {
                //check to see if player exists in the game already and update their place in the game to be in the game
                for(int i = 0;i < playerNamesInGameDataList.Count; i++)
                {
                    if(player.displayName == playerNamesInGameDataList[i] && playerStatusInGameDataList[i] == (int)PlayerInGameStatus.Disconnected)
                    {
                        //Debug.Log("Found Disconnected Player At Index: " + i.ToString());
                        playerStatusInGameDataList[i] = (int)PlayerInGameStatus.Connected;
                        playersInGameDataList[i] = playersInGameDataList[i] == -1 ? player.playerId : playersInGameDataList[i];
                        RequestSerialization();
                    }
                }
            }
        }
    }
    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        if (Networking.LocalPlayer.isMaster)
        {
            //Debug.Log("Is Master");
            if (gameVariables.GameStarted)
            {
                //Debug.Log("Game Started");
                //check to see if player exists in the game already and update their place in the game to be a left status
                for (int i = 0; i < playersInGameDataList.Count; i++)
                {
                    if (player.playerId == playersInGameDataList[i])
                    {
                        playersInGameDataList[i] = -1;
                        playerStatusInGameDataList[i] = (int)PlayerInGameStatus.Disconnected;
                        RequestSerialization();
                        if (i == gameVariables.CurrentPlayerIndex)
                        {
                            gameController.NextPlayer();
                        }
                    }
                }
            }
            else
            {
                if (playersInGameDataList.Contains(player.playerId))
                {
                    playersInGameDataList.Remove(player.playerId);
                    RequestSerialization();
                }
            }
        }
    }
}
public enum PlayerInGameStatus
{
    Connected = 0,
    LeftGame = 1,
    Disconnected = 2
}
