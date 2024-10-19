
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
    
    void Update()
    {
        
    }
    public void RollDiceMaster()
    {
        if (Networking.LocalPlayer.isMaster)
        {
            int randomRoll = GetRandomRoll();
            //Debug.Log("Player Rolled: " + randomRoll.ToString());
            if(randomRoll == gameVariables.CurrentRoll)
            {
                //Debug.Log("Same Roll: " + gameVariables.CurrentRoll);
                gameVariables.SameRoll++;
            }
            else
            {
                //Debug.Log("Random Roll: " + randomRoll.ToString());
                gameVariables.CurrentRoll = randomRoll;
            }
            gameVariables.RequestSerialization();
        }
    }
    public void CalculateRoll()
    {
        if (!Networking.LocalPlayer.isMaster)
        {
            return;
        }
        int finalLandingSpace = gameController.CalculateLandingSpace(gameVariables.CurrentRoll, gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].Int);
        //Debug.Log("Final Landing Space Initial: " + finalLandingSpace.ToString());
        bool movementHasEnded = false;
        SpaceSettings spaceSetting = gameController.GetSpace(finalLandingSpace);
        int numberOfMovements = 0;
        if (finalLandingSpace != 0 && !gameController.IsEnd(finalLandingSpace))
        {


            while (!movementHasEnded)
            {
                bool sendBackToStart = gameController.ProcessSendBackToStart(spaceSetting);
                int moveForwardBackwards = gameController.ProcessLandedSpaceMovement(spaceSetting);
                int swapPlayer = (int)gameController.ProcessSwapWithPlayer(spaceSetting);
                if (sendBackToStart)
                {
                    //send back to start takes full movement priority.
                    finalLandingSpace = 0;
                }
                else if (moveForwardBackwards != 0)
                {
                    //moving forwards/backwards takes next priority, moving forwards is before backwards.
                    //Debug.Log("Move Forward Backwards: " + moveForwardBackwards.ToString());
                    finalLandingSpace = gameController.CalculateLandingSpace(moveForwardBackwards, finalLandingSpace);
                    //Debug.Log("New Final Landing Space: " + finalLandingSpace.ToString());
                }
                else if (swapPlayer != 0)
                {
                    //last priority is swapping player, if they aren't going back to start, and if they aren't moving forwards or backwards, and they land on swap, they will then swap.
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
                    //if none of these things are true, we know that their possible movement manipulations have completed.
                    movementHasEnded = true;
                }
                spaceSetting = gameController.GetSpace(finalLandingSpace);
                numberOfMovements++;
                if (numberOfMovements > 10)
                {
                    //Debug.Log("What the fuck are you doing with this much movement manipulation off one space? Cancelled dumbass.");
                    break;
                }
            }
            numberOfMovements = 0;
            gameController.ProcessMissedTurn(spaceSetting);
            gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex] = finalLandingSpace;
            gameController.ProcessAudio(spaceSetting);
            if (!gameController.ProcessRollAgain(spaceSetting))
            {
                gameController.NextPlayer();
            }
            else
            {
                gameController.SamePlayerRollAgain();
            }
        }
        else if(finalLandingSpace == 0)
        {
            //Debug.Log("Player is at start, no manipulation.");
            numberOfMovements = 0;
            gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex] = finalLandingSpace;
            gameController.NextPlayer();
        }
        else if (gameController.IsEnd(finalLandingSpace))
        {
            //Debug.Log("GAME OVER!!!");
        }
        updateSpaces.UpdateOutlineSpaces();
        //possibly need to request serialization on game variables, but the roll should do it
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