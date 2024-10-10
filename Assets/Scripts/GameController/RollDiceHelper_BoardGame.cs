
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RollDiceHelper_BoardGame : UdonSharpBehaviour
{
    [SerializeField] GameVariables_BoardGame gameVariables;
    [SerializeField] PlayerList_BoardGame playerLists;
    [SerializeField] GameController_BoardGame gameController;
    [SerializeField] UpdateSpaces updateSpaces;
    void Start()
    {
        
    }
    public void RollDiceMaster()
    {
        if (Networking.LocalPlayer.isMaster)
        {
            int randomRoll = GetRandomRoll();
            Debug.Log("Player Rolled: " + randomRoll.ToString());
            if(randomRoll == gameVariables.CurrentRoll)
            {
                gameVariables.SameRoll++;
            }
            else
            {
                gameVariables.CurrentRoll = randomRoll;
            }
            int finalLandingSpace = gameController.CalculateLandingSpace(randomRoll);
            bool movementHasEnded = false;
            while (!movementHasEnded)
            {
                SpaceSettings spaceSetting = gameController.GetSpace(finalLandingSpace);
                bool sendBackToStart = gameController.ProcessSendBackToStart(spaceSetting);
                int moveForwardBackwards = gameController.ProcessLandedSpaceMovement(spaceSetting);
                int swapPlayer = (int)gameController.ProcessSwapWithPlayer(spaceSetting);
                if (sendBackToStart)
                {
                    finalLandingSpace = 0;
                }
                else if(moveForwardBackwards != 0)
                {
                    finalLandingSpace = finalLandingSpace + moveForwardBackwards;
                }
                else if(swapPlayer != 0)
                {
                    gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex] = finalLandingSpace;
                    int playerToSwapIndex = gameController.ProcessSwapPlayer((SwapWithPlayer)swapPlayer, gameVariables.CurrentPlayerIndex);
                    if (playerToSwapIndex == gameVariables.CurrentPlayerIndex)
                    {
                        movementHasEnded = true;
                    }
                    else
                    {
                        int tempCurrentIndexSpace = gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].Int;
                        gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex] = gameVariables.playerSpaceDataList[playerToSwapIndex];
                        gameVariables.playerSpaceDataList[playerToSwapIndex] = tempCurrentIndexSpace;
                        finalLandingSpace = gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].Int;
                    }
                }
                else
                {
                    movementHasEnded = true;
                }
            }
            gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex] = finalLandingSpace;
            gameVariables.PreviousPlayerIndex = gameVariables.CurrentPlayerIndex;
            if (gameVariables.CurrentPlayerIndex == playerLists.playersInGameDataList.Count - 1)
            {
                gameVariables.CurrentPlayerIndex = 0;
            }
            else
            {
                gameVariables.CurrentPlayerIndex++;
            }
            updateSpaces.UpdateOutlineSpaces();
            gameVariables.RequestSerialization();
        }
    }
    private int GetRandomRoll()
    {
        return Random.Range(1, 6);
    }
}
public enum SwapWithPlayer
{
    DontSwap = 0,
    SwapWithFirst = 1,
    SwapWithLast =  2
}