
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
    public GameObject boardGameSpaces;
    public GameObject boardGameSpaceSettings;

    public GameObject diceObjectInteract;

    public void StartGame()
    {
        Debug.Log("Start Game");
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
                playerLists.Shuffled = true;
                gameVariables.GameStarted = true;
                gameVariables.CurrentPlayerIndex = 0;
                playerLists.GetSelfIndex();
                playerLists.RequestSerialization();
                gameVariables.RequestSerialization();
            }
        }
    }
    public void RollDice()
    {
        if(Networking.LocalPlayer.playerId == playerLists.playersInGameDataList[gameVariables.CurrentPlayerIndex])
        {
            Debug.Log("Validated User - Can Roll Dice. Sending to Master");
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "RollDiceMaster");
        }
    }
    public void RollDiceMaster()
    {
        Debug.Log("Received Roll Dice Master");
        rollDiceHelper.RollDiceMaster();
    }
    private DataList Shuffle(DataList list)
    {
        DataList tempList = list.ShallowClone();
        DataList returnList = new DataList();
        for (int i = tempList.Count - 1; i >= 0; i--)
        {
            int indexToRemove = Random.Range(0, tempList.Count);
            Debug.Log("Index to Remove: " + indexToRemove.ToString());
            int value = tempList[indexToRemove].Int;
            Debug.Log("Index Value: " + value.ToString());
            tempList.Remove(value);
            returnList.Add(value);
        }
        return returnList;
    }
    public void CheckToUpdateDiceClickerInteract()
    {
        if (gameVariables.gameStarted && gameVariables.ReceivedGameStartedValues && playerLists.ReceivedGameStartedValues)
        {
            Debug.Log("Checking For Dice Clicker Update");
            Debug.Log("Local Player: " + Networking.LocalPlayer.playerId.ToString());
            Debug.Log("Current Player: " + playerLists.playersInGameDataList[gameVariables.CurrentPlayerIndex].ToString());
            Debug.Log("Players in Game Length: " + playerLists.playersInGameDataList.Count.ToString());
            if (Networking.LocalPlayer.playerId == playerLists.playersInGameDataList[gameVariables.CurrentPlayerIndex])
            {
                Debug.Log("Found Current Player - Setting Dice Interact to Active");
                diceObjectInteract.SetActive(true);
            }
            else
            {
                Debug.Log("Not Current Player - Setting Dice Interact to Disabled");
                DisableDiceObjectInteract();
            }
        }
    }
    public SpaceSettings GetSpace(int space)
    {
        return boardGameSpaceSettings.transform.GetChild(space).gameObject.GetComponent<SpaceSettings>();
    }
    public bool ProcessRollAgain(SpaceSettings spaceSetting)
    {
        if (spaceSetting.RollAgain)
        {
            Debug.Log("RollAgain");
            return true;
        }
        else
        {
            return false;
        }
    }
    public int CalculateLandingSpace(int roll)
    {
        int totalSpaces = boardGameSpaceSettings.transform.childCount - 1;
        int currentSpace = gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].Int;
        if (gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].Int + roll > totalSpaces)
        {
            Debug.Log("Over Max");
            Debug.Log("Total Spaces: " + totalSpaces.ToString());
            Debug.Log("Current Space: " + currentSpace.ToString());
            Debug.Log("Roll: " + roll.ToString());
            return totalSpaces - (roll - (totalSpaces - currentSpace));
        }
        else
        {
            return roll + gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].Int;
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
            return currentPlayerIndex;
        }
    }
    public SwapWithPlayer ProcessSwapWithPlayer(SpaceSettings spaceSetting)
    {
        if (spaceSetting.SwapWithFirst)
        {
            Debug.Log("SwapWithFirst");
            return SwapWithPlayer.SwapWithFirst;
        }
        if (spaceSetting.SwapWithLast)
        {
            Debug.Log("SwapWithLast");
            return SwapWithPlayer.SwapWithLast;
        }
        return SwapWithPlayer.DontSwap;
    }
    public bool ProcessSendBackToStart(SpaceSettings spaceSetting)
    {
        if (spaceSetting.SendBackToStart)
        {
            Debug.Log("SendBackToStart");
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
            Debug.Log("MoveForwardXSpaces");
            return spaceSetting.MoveForwardXSpaces;
        }
        if (spaceSetting.MoveBackXSpaces > 0)
        {
            Debug.Log("MoveBackXSpaces");
            return spaceSetting.MoveBackXSpaces * -1;
        }
        return 0;
        
        //if (spaceSetting.DrinkWhatYouRoll)
        //{
        //    Debug.Log("DrinkWhatYouRoll");
        //}
        //spaceSetting.ImmuneFromDrinking)
        //{
        //    Debug.Log("ImmuneFromDrinking");
        //}
        //if (spaceSetting.MissTurn)
        //{
        //    Debug.Log("MissTurn");
        //}
        //if (spaceSetting.DrinkWithHost)
        //{
        //    Debug.Log("DrinkWithHost");
        //}
        //if (spaceSetting.ChooseSomeoneToDrink)
        //{
        //    Debug.Log("ChooseSomeoneToDrink");
        //}
        //if (spaceSetting.GirlsDrink)
        //{
        //    Debug.Log("GirlsDrink");
        //}
        //if (spaceSetting.GuysDrink)
        //{
        //    Debug.Log("GuysDrink");
        //}
        //if (spaceSetting.Finish)
        //{
        //    Debug.Log("Finish");
        //}
        //if (spaceSetting.Start)
        //{
        //    Debug.Log("Start");
        //}
        //if (spaceSetting.DrinkXTimes > 0)
        //{
        //    Debug.Log("DrinkXTimes");
        //}
        //if (spaceSetting.EveryoneDrinkXTimes > 0)
        //{
        //    Debug.Log("EveryoneDrinkXTimes");
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