
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class GameController_BoardGame : UdonSharpBehaviour
{
    [SerializeField] PlayerList_BoardGame playerLists;
    [SerializeField] GameVariables_BoardGame gameVariables;
    [SerializeField] RollDiceHelper_BoardGame rollDiceHelper;
    [SerializeField] UpdateSpaces updateSpaces;
    [SerializeField] UpdatePlayerCamerasOnSpace_BoardGame updatePlayerCamerasOnSpace;
    public GameObject boardGameSpaceSettings;

    public GameObject diceObjectInteract;
    public void EndGame()
    {
        gameVariables.GameStarted = false;
        gameVariables.GameEnded = true;
        playerLists.playersInGameDataList.Clear();
        playerLists.playerStatusInGameDataList.Clear();
        playerLists.playerNamesInGameDataList.Clear();
        gameVariables.missedTurnDataList.Clear();
        gameVariables.playerSpaceDataList.Clear();
        gameVariables.ReceivedGameStartedValues = false;
        playerLists.ReceivedGameStartedValues = false;
        gameVariables.ReceivedAllVariables = false;
        playerLists.Shuffled = false;
        gameVariables.GameStarted = false;
        gameVariables.CurrentPlayerIndex = -1;
        diceObjectInteract.SetActive(false);
        //updatePlayerCamerasOnSpace.ClearAllSpacesOfPictures();
        updateSpaces.ClearOutlineSpaces();
        gameVariables.RequestSerialization();
        playerLists.RequestSerialization();
    }
    public void StartGame()
    {
        //Debug.Log("Start Game");
        if (Networking.LocalPlayer.isMaster)
        {
            if (!gameVariables.GameStarted)
            {
                DataList tempList = Shuffle(playerLists.playersInGameDataList);
                playerLists.playersInGameDataList = tempList;
                for (int i = 0; i < playerLists.playersInGameDataList.Count; i++)
                {
                    playerLists.playerStatusInGameDataList.Add((int)PlayerInGameStatus.Connected);
                    playerLists.playerNamesInGameDataList.Add(VRCPlayerApi.GetPlayerById(playerLists.playersInGameDataList[i].Int).displayName);
                    gameVariables.missedTurnDataList.Add(false);
                    gameVariables.playerSpaceDataList.Add(0);
                }
                gameVariables.ReceivedGameStartedValues = true;
                playerLists.ReceivedGameStartedValues = true;
                gameVariables.ReceivedAllVariables = true;
                playerLists.Shuffled = true;
                gameVariables.GameStarted = true;
                gameVariables.GameEnded = false;
                gameVariables.CurrentPlayerIndex = 0;
                playerLists.GetSelfIndex();
                gameVariables.PlayerUpdateBoard++;
                gameVariables.TakePicture++;
                updatePlayerCamerasOnSpace.UpdateCameraCountOnSpaces();
                playerLists.RequestSerialization();
                gameVariables.RequestSerialization();
            }
        }
    }
    public void DetectPlayerForIncrement()
    {
        if (gameVariables.CurrentPlayerIndex == playerLists.playersInGameDataList.Count - 1)
        {
            Debug.Log("Setting Index 0");
            gameVariables.CurrentPlayerIndex = 0;
        }
        else
        {
            gameVariables.CurrentPlayerIndex++;
            Debug.Log("Setting Index: " + gameVariables.CurrentPlayerIndex.ToString());
        }
    }
    public void SamePlayerRollAgain()
    {
        Debug.Log("Roll Again");
        gameVariables.PreviousPlayerIndex = gameVariables.CurrentPlayerIndex;
        if (gameVariables.missedTurnDataList[gameVariables.CurrentPlayerIndex].Boolean)
        {
            //Debug.Log("Current Player Missed Turn While On Roll Again, Kind of Wild: " + gameVariables.CurrentPlayerIndex.ToString());
            gameVariables.missedTurnDataList[gameVariables.CurrentPlayerIndex] = false;
            NextPlayer();
        }
        else
        {
            gameVariables.SamePlayer++;
        }
        gameVariables.PlayerUpdateBoard++;
        gameVariables.RequestSerialization();
    }
    public void NextPlayer()
    {
        //increment player
        Debug.Log("Enter Next Player Function");
        gameVariables.PreviousPlayerIndex = gameVariables.CurrentPlayerIndex;
        DetectPlayerForIncrement();
        while (playerLists.playerStatusInGameDataList[gameVariables.CurrentPlayerIndex].Int > 0)
        {
            Debug.Log("Current Player is Disconnected Or Left Game: " + gameVariables.CurrentPlayerIndex.ToString());
            DetectPlayerForIncrement();
        }
        //loop until we find a player that isn't missing a turn, this also clears out missed turns in the case that there's all missed turns + disconnected or left players.
        while (gameVariables.missedTurnDataList[gameVariables.CurrentPlayerIndex].Boolean)
        {
            Debug.Log("Current Player Missed Turn: " + gameVariables.CurrentPlayerIndex.ToString());
            gameVariables.missedTurnDataList[gameVariables.CurrentPlayerIndex] = false;
            DetectPlayerForIncrement();
        }
        while (playerLists.playerStatusInGameDataList[gameVariables.CurrentPlayerIndex].Int > 0)
        {
            Debug.Log("Current Player is Disconnected Or Left Game: " + gameVariables.CurrentPlayerIndex.ToString());
            DetectPlayerForIncrement();
        }
        if (gameVariables.CurrentPlayerIndex == gameVariables.PreviousPlayerIndex)
        {
            Debug.Log("Roll Again Somehow");
            SamePlayerRollAgain();
        }
        else
        {
            Debug.Log("Incrementing Player Update Board");
            gameVariables.PlayerUpdateBoard++;
            gameVariables.RequestSerialization();
        }
    }
    public void RollDice()
    {
        if(Networking.LocalPlayer.playerId == playerLists.playersInGameDataList[gameVariables.CurrentPlayerIndex])
        {
            //Debug.Log("Validated User - Can Roll Dice. Sending to Master");
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "RollDiceMaster");
        }
    }
    public void RollDiceMaster()
    {
        //Debug.Log("Received Roll Dice Master");
        rollDiceHelper.RollDiceMaster();
    }
    private DataList Shuffle(DataList list)
    {
        DataList tempList = list.ShallowClone();
        DataList returnList = new DataList();
        for (int i = tempList.Count - 1; i >= 0; i--)
        {
            int indexToRemove = Random.Range(0, tempList.Count);
            //Debug.Log("Index to Remove: " + indexToRemove.ToString());
            int value = tempList[indexToRemove].Int;
            //Debug.Log("Index Value: " + value.ToString());
            tempList.Remove(value);
            returnList.Add(value);
        }
        return returnList;
    }
    public void CheckToUpdateDiceClickerInteract()
    {
        if (gameVariables.gameStarted && gameVariables.ReceivedGameStartedValues && playerLists.ReceivedGameStartedValues)
        {
            //Debug.Log("Checking For Dice Clicker Update");
            //Debug.Log("Local Player: " + Networking.LocalPlayer.playerId.ToString());
            //Debug.Log("Current Player: " + playerLists.playersInGameDataList[gameVariables.CurrentPlayerIndex].ToString());
            //Debug.Log("Players in Game Length: " + playerLists.playersInGameDataList.Count.ToString());
            if (Networking.LocalPlayer.playerId == playerLists.playersInGameDataList[gameVariables.CurrentPlayerIndex])
            {
                Debug.Log("Found Current Player - Setting Dice Interact to Active");
                diceObjectInteract.SetActive(true);
            }
            else
            {
                Debug.Log("Not Current Player - Setting Dice Interact to Disabled");
                Debug.Log("Current Index: " + gameVariables.CurrentPlayerIndex.ToString());
                Debug.Log("Current Player Number: " + playerLists.playersInGameDataList[gameVariables.CurrentPlayerIndex].ToString());
                DisableDiceObjectInteract();
            }
        }
    }
    public SpaceSettings GetSpace(int space)
    {
        return boardGameSpaceSettings.transform.GetChild(space).gameObject.GetComponent<SpaceSettings>();
    }
    public void ProcessAudio(SpaceSettings spaceSetting)
    {
        if(spaceSetting.EveryoneDrinkXTimes > 0)
        {
            gameVariables.ToggleEveryoneDrink++;
        }
        else if(spaceSetting.GirlsDrink)
        {
            gameVariables.ToggleGirlsDrink++;
        }
        else if (spaceSetting.GuysDrink)
        {
            gameVariables.ToggleGuysDrink++;
        }
        else if (spaceSetting.DrinkWithHost)
        {
            gameVariables.ToggleDrinkWithHost++;
        }
        else if (spaceSetting.ChooseSomeoneToDrink)
        {
            gameVariables.ToggleChooseSomeoneToDrink++;
        }
        else if (spaceSetting.DrinkXTimes > 0)
        {
            gameVariables.ToggleDrink++;
        }
    }
    public bool ProcessRollAgain(SpaceSettings spaceSetting)
    {
        if (spaceSetting.RollAgain)
        {
            //Debug.Log("------------------------RollAgain------------------------");
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool IsEnd(int space)
    {
        return boardGameSpaceSettings.transform.childCount - 1 == space;
    }
    public int CalculateLandingSpace(int roll, int space)
    {
        int totalSpaces = boardGameSpaceSettings.transform.childCount - 1;
        if (space + roll > totalSpaces)
        {
            //Debug.Log("Over Max");
            //Debug.Log("Total Spaces: " + totalSpaces.ToString());
            //Debug.Log("Current Space: " + space.ToString());
            //Debug.Log("Roll: " + roll.ToString());
            return totalSpaces - (roll - (totalSpaces - space));
        }
        else if(space + roll < 0)
        {
            //Debug.Log("Under Min? Are you dumb?");
            return 0;
        }
        else
        {
            return roll + space;
        }
    }
    private int FindFirstPlacePlayerIndex(int currentPlayerIndex)
    {
        int maxIndex = currentPlayerIndex;
        for(int i = 0; i < gameVariables.playerSpaceDataList.Count; i++)
        {
            if(gameVariables.playerSpaceDataList[maxIndex].Int < gameVariables.playerSpaceDataList[i].Int)
            {
                maxIndex = i;
            }
        }
        return maxIndex;
    }
    private int FindLastPlacePlayerIndex(int currentPlayerIndex)
    {
        int minIndex = currentPlayerIndex;
        for (int i = 0; i < gameVariables.playerSpaceDataList.Count; i++)
        {
            if (gameVariables.playerSpaceDataList[i].Int < gameVariables.playerSpaceDataList[minIndex].Int)
            {
                minIndex = i;
            }
        }
        return minIndex;
    }
    public void ProcessMissedTurn(SpaceSettings spaceSetting)
    {
        if (spaceSetting.MissTurn)
        {
            //Debug.Log("Found Miss Turn, Updating List");
            gameVariables.missedTurnDataList[gameVariables.CurrentPlayerIndex] = true;
        }
    }
    public int ProcessSwapPlayer(SwapWithPlayer swapWithPlayer, int currentPlayerIndex)
    {
        
        if(swapWithPlayer == SwapWithPlayer.SwapWithFirst)
        {
            return FindFirstPlacePlayerIndex(currentPlayerIndex);
        }
        else if(swapWithPlayer == SwapWithPlayer.SwapWithLast)
        {
            return FindLastPlacePlayerIndex(currentPlayerIndex);
        }
        else
        {
            //Debug.Log("------------------------Can't swap with self------------------------");
            return currentPlayerIndex;
        }
    }
    public SwapWithPlayer ProcessSwapWithPlayer(SpaceSettings spaceSetting)
    {
        if (spaceSetting.SwapWithFirst)
        {
            //Debug.Log("------------------------SwapWithFirst------------------------");
            return SwapWithPlayer.SwapWithFirst;
        }
        if (spaceSetting.SwapWithLast)
        {
            //Debug.Log("------------------------SwapWithLast------------------------");
            return SwapWithPlayer.SwapWithLast;
        }
        return SwapWithPlayer.DontSwap;
    }
    public bool ProcessSendBackToStart(SpaceSettings spaceSetting)
    {
        if (spaceSetting.SendBackToStart)
        {
            //Debug.Log("------------------------SendBackToStart------------------------");
            return true;
        }
        else
        {
            return false;
        }
    }
    public int ProcessLandedSpaceMovement(SpaceSettings spaceSetting)
    {
        if (spaceSetting.MoveForwardXSpaces > 0)
        {
            //Debug.Log("------------------------MoveForwardXSpaces------------------------");
            //Debug.Log(spaceSetting.MoveForwardXSpaces.ToString() + " Spaces");
            return spaceSetting.MoveForwardXSpaces;
        }
        if (spaceSetting.MoveBackXSpaces > 0)
        {
            //Debug.Log("------------------------MoveBackXSpaces------------------------");
            //Debug.Log(spaceSetting.MoveBackXSpaces.ToString() + " Spaces");
            return spaceSetting.MoveBackXSpaces * -1;
        }
        return 0;
        
        //if (spaceSetting.DrinkWhatYouRoll)
        //{
        //    //Debug.Log("DrinkWhatYouRoll");
        //}
        //spaceSetting.ImmuneFromDrinking)
        //{
        //    //Debug.Log("ImmuneFromDrinking");
        //}
        //if (spaceSetting.MissTurn)
        //{
        //    //Debug.Log("MissTurn");
        //}
        //if (spaceSetting.DrinkWithHost)
        //{
        //    //Debug.Log("DrinkWithHost");
        //}
        //if (spaceSetting.ChooseSomeoneToDrink)
        //{
        //    //Debug.Log("ChooseSomeoneToDrink");
        //}
        //if (spaceSetting.GirlsDrink)
        //{
        //    //Debug.Log("GirlsDrink");
        //}
        //if (spaceSetting.GuysDrink)
        //{
        //    //Debug.Log("GuysDrink");
        //}
        //if (spaceSetting.Finish)
        //{
        //    //Debug.Log("Finish");
        //}
        //if (spaceSetting.Start)
        //{
        //    //Debug.Log("Start");
        //}
        //if (spaceSetting.DrinkXTimes > 0)
        //{
        //    //Debug.Log("DrinkXTimes");
        //}
        //if (spaceSetting.EveryoneDrinkXTimes > 0)
        //{
        //    //Debug.Log("EveryoneDrinkXTimes");
        //}
        
        //return space;
    }
    bool IsSameShuffledList(DataList tempList)
    {
        for(int i = 0;i < tempList.Count; i++)
        {
            if(tempList[i] != playerLists.playersInGameDataList[i])
            {
                return false;
            }
        }
        return true;
    }
    public void DisableDiceObjectInteract()
    {
        diceObjectInteract.SetActive(false);
    }
}