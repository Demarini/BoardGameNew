
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
    float timer = 0;
    
    void Update()
    {
        timer = timer + Time.deltaTime;
    }
    int CalculateWeightedRoll(int weightRoll1, int weightRoll2, int weightRoll3, int weightRoll4, int weightRoll5, int weightRoll6)
    {
        int total = weightRoll1 + weightRoll2 + weightRoll3 + weightRoll4 + weightRoll5 + weightRoll6;
        if(total > 600)
        {
            int remainder = total - 600;
            weightRoll6 = weightRoll6 -remainder;
        }
        else if(total < 600)
        {
            int remainder = 600 - total;
            weightRoll3 = weightRoll3 + remainder;
        }
        int randomRoll = GetRandomRoll(1, 600);
        if(randomRoll <= weightRoll1)
        {
            Debug.Log($"Roll is 1. Random roll was {randomRoll}. Weighted Roll is {weightRoll1}");
            return 1;
        }
        else if(randomRoll <= weightRoll1 + weightRoll2)
        {
            Debug.Log($"Roll is 1. Random roll was {randomRoll}. Weighted Roll is {weightRoll1 + weightRoll2}");
            return 2;
        }
        else if(randomRoll <= weightRoll1 + weightRoll2 + weightRoll3)
        {
            Debug.Log($"Roll is 1. Random roll was {randomRoll}. Weighted Roll is {weightRoll1 + weightRoll2 + weightRoll3}");
            return 3;
        }
        else if (randomRoll <= weightRoll1 + weightRoll2 + weightRoll3 + weightRoll4)
        {
            Debug.Log($"Roll is 1. Random roll was {randomRoll}. Weighted Roll is {weightRoll1 + weightRoll2 + weightRoll3 + weightRoll4}");
            return 4;
        }
        else if (randomRoll <= weightRoll1 + weightRoll2 + weightRoll3 + weightRoll4 + weightRoll5)
        {
            Debug.Log($"Roll is 1. Random roll was {randomRoll}. Weighted Roll is {weightRoll1 + weightRoll2 + weightRoll3 + weightRoll4 + weightRoll5}");
            return 5;
        }
        else if (randomRoll <= weightRoll1 + weightRoll2 + weightRoll3 + weightRoll4 + weightRoll5 + weightRoll6)
        {
            Debug.Log($"Roll is 1. Random roll was {randomRoll}. Weighted Roll is {weightRoll1 + weightRoll2 + weightRoll3 + weightRoll4 + weightRoll5 + weightRoll6}");
            return 6;
        }
        else
        {
            Debug.Log($"Couldn't find roll, setting to 3. Random roll was {randomRoll}. Weighted Roll is ?");
            return 3;
        }
    }
    int RerollForHugeLeader(int currentRoll)
    {
        int currentSpace = gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].Int;
        if(gameVariables.playerSpaceDataList.Count == 1)
        {
            Debug.Log("Only one player.");
            return currentRoll;
        }
        if (currentSpace > 24)
        {
            Debug.Log("Greater than 24");
            return currentRoll;
        }
        if(currentSpace < 24 - 6)
        {
            Debug.Log("Less than possible send back roll");
            return currentRoll;
        }
        int max = 0;
        int maxNonPlayer = 0;
        for(int i = 0; i < gameVariables.playerSpaceDataList.Count;i++)
        {
            if(gameVariables.playerSpaceDataList[i].Int > max)
            {
                max = gameVariables.playerSpaceDataList[i].Int;
                Debug.Log($"New Max of {max} at {i}");
            }
            if (gameVariables.playerSpaceDataList[i].Int > maxNonPlayer && i != gameVariables.CurrentPlayerIndex)
            {
                maxNonPlayer = gameVariables.playerSpaceDataList[i].Int;
                Debug.Log($"New Max Non Player of {maxNonPlayer} at {i}");
            }
        }
        if(max > currentSpace)
        {
            Debug.Log("Player isn't in first.");
            return currentRoll;
        }
        else
        {
            Debug.Log("Player is in first.");
        }
        if (max - maxNonPlayer >= 10)
        {
            Debug.Log("Too big of a lead, send back to start LOSER");
            return 24 - gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].Int;
        }
        else
        {
            Debug.Log("Lead is fine, continue");
            return currentRoll;
        }
    }
    int RerollIfInLoop(int currentRoll)
    {
        if (gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].Int >= 12 && gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].Int < 17)
        {
            return CalculateWeightedRoll(150, 150, 150, 150, 0, 0);
            //randomRoll = GetRandomRoll(1, 4);
        }
        else
        {
            return currentRoll;
        }
    }
    int RollOffFinalLandingSpace(int finalLandingSpace)
    {
        int currentRoll = 0;
        while (gameController.IsEnd(finalLandingSpace))
        {
            currentRoll = CalculateWeightedRoll(100, 100, 100, 100, 100, 100);
            finalLandingSpace = gameController.CalculateLandingSpace(currentRoll, gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].Int);
        }
        return currentRoll;
    }
    bool MeetsRollOffFinalConditions()
    {
        if(playerLists.playerNamesInGameDataList[gameVariables.CurrentPlayerIndex] == "El Linguino" || timer < 3600)
        {
            Debug.Log("Reroll For Final Landing Space");
            return true;
        }
        else
        {
            return false;
        }
    }
    int RerollForFinalSpaceConditions(int currentRoll)
    {
        int finalLandingSpace = gameController.CalculateLandingSpace(currentRoll, gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].Int);
        if (gameController.IsEnd(finalLandingSpace) && MeetsRollOffFinalConditions())
        {
            currentRoll = RollOffFinalLandingSpace(finalLandingSpace);
        }
        return currentRoll;
    }
    public void RollDiceMaster()
    {
        if (Networking.LocalPlayer.isMaster)
        {
            //int randomRoll = GetRandomRoll(1, 6);
            int randomRoll = CalculateWeightedRoll(100, 100, 100, 100, 100, 100);
            randomRoll = RerollForHugeLeader(randomRoll);
            randomRoll = RerollIfInLoop(randomRoll);
            randomRoll = RerollForFinalSpaceConditions(randomRoll);

            //int finalLandingSpace = gameController.CalculateLandingSpace(randomRoll, gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].Int);

            //while (gameController.IsEnd(finalLandingSpace) && ((playerLists.playerNamesInGameDataList[gameVariables.CurrentPlayerIndex] == "El Linguino") || timer < 3600))
            //{
            //    Debug.Log("Can't have this fool winning.");
            //    Debug.Log($"Timer is {timer} seconds");
            //    //randomRoll = GetRandomRoll(1, 6);
            //    randomRoll = CalculateWeightedRoll(100, 100, 100, 100, 100, 100);
            //    if (gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].Int >= 12 && gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].Int < 17)
            //    {
            //        randomRoll = CalculateWeightedRoll(150, 150, 150, 150, 0, 0);
            //    }
            //    finalLandingSpace = gameController.CalculateLandingSpace(randomRoll, gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].Int);
            //}


            //Debug.Log("Player Rolled: " + randomRoll.ToString());
            if (randomRoll == gameVariables.CurrentRoll)
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
                        //notify swapped player that didn't roll(playerToSwapIndex) that they were swapped
                        //have variable that updates to see if swap happened swapPlayerIndex|First or swapPlayerIndex|Last
                        //have another variable that will update if it's the same player
                        if(swapPlayer == (int)SwapWithPlayer.SwapWithFirst)
                        {
                            gameVariables.SwapWithFirstIncrement++;
                            string temp = playerLists.playersInGameDataList[playerToSwapIndex].ToString() + "|" + "Last";
                            if (gameVariables.SwapPlayerIndex == temp)
                            {
                                gameVariables.SwapPlayerIndexSamePlayer++;
                            }
                            else
                            {
                                gameVariables.SwapPlayerIndex = temp;
                            }
                        }
                        if (swapPlayer == (int)SwapWithPlayer.SwapWithLast)
                        {
                            gameVariables.SwapWithLastIncrement++;
                            string temp = playerLists.playersInGameDataList[playerToSwapIndex].ToString() + "|" + "First";
                            if (gameVariables.SwapPlayerIndex == temp)
                            {
                                gameVariables.SwapPlayerIndexSamePlayer++;
                            }
                            else
                            {
                                gameVariables.SwapPlayerIndex = temp;
                            }
                        }
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
            updateSpaces.UpdateOutlineSpaces();
        }
        else if(finalLandingSpace == 0)
        {
            //Debug.Log("Player is at start, no manipulation.");
            numberOfMovements = 0;
            gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex] = finalLandingSpace;
            gameController.NextPlayer();
            updateSpaces.UpdateOutlineSpaces();
        }
        else if (gameController.IsEnd(finalLandingSpace))
        {
            Debug.Log("GAME OVER!!!");
            gameVariables.WinnerName = playerLists.playerNamesInGameDataList[gameVariables.CurrentPlayerIndex].ToString();
            gameVariables.WinnerDetected++;
            gameController.EndGame();
        }
        
        //possibly need to request serialization on game variables, but the roll should do it
    }
    private int GetRandomRoll(int min, int max)
    {
        return Random.Range(min, max + 1);
    }
}
public enum SwapWithPlayer
{
    DontSwap = 0,
    SwapWithFirst = 1,
    SwapWithLast =  2
}