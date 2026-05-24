
using System;
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
    [SerializeField] MysteryManager mysteryManager;
    float timer = 0;
    bool isDebugging = false;
    bool waitingForMystery = false;
    
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
        int currentSpace = Convert.ToInt32(gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].ToString());
        if(gameVariables.playerSpaceDataList.Count == 1)
        {
            Debug.Log("Only one player.");
            return currentRoll;
        }
        
        
        int max = 0;
        int maxNonPlayer = 0;
        for(int i = 0; i < gameVariables.playerSpaceDataList.Count;i++)
        {
            int spaceAtI = Convert.ToInt32(gameVariables.playerSpaceDataList[i].ToString());
            if(spaceAtI > max)
            {
                max = spaceAtI;
                Debug.Log($"New Max of {max} at {i}");
            }
            if (spaceAtI > maxNonPlayer && i != gameVariables.CurrentPlayerIndex)
            {
                maxNonPlayer = spaceAtI;
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
            if (currentSpace < 24 - 6 || currentSpace > 24)
            {
                Debug.Log("Less than possible send back roll");
                return CalculateWeightedRoll(350, 250, 0, 0, 0, 0);
            }
            else
            {
                Debug.Log("Too big of a lead, send back to start LOSER");
                return 24 - Convert.ToInt32(gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].ToString());
            }
        }
        else
        {
            Debug.Log("Lead is fine, continue");
            return currentRoll;
        }
        
        
    }
    int RerollIfInLoop(int currentRoll)
    {
        if (Convert.ToInt32(gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].ToString()) >= 12 && Convert.ToInt32(gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].ToString()) < 17)
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
            finalLandingSpace = gameController.CalculateLandingSpace(currentRoll, Convert.ToInt32(gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].ToString()));
        }
        return currentRoll;
    }
    bool MeetsRollOffFinalConditions()
    {
        if (playerLists.playerNamesInGameDataList[gameVariables.CurrentPlayerIndex] == "El Linguino" || timer < 1800)
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
        int finalLandingSpace = gameController.CalculateLandingSpace(currentRoll, Convert.ToInt32(gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].ToString()));
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

            //int finalLandingSpace = gameController.CalculateLandingSpace(randomRoll, Convert.ToInt32(gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].ToString()));

            //while (gameController.IsEnd(finalLandingSpace) && ((playerLists.playerNamesInGameDataList[gameVariables.CurrentPlayerIndex] == "El Linguino") || timer < 3600))
            //{
            //    Debug.Log("Can't have this fool winning.");
            //    Debug.Log($"Timer is {timer} seconds");
            //    //randomRoll = GetRandomRoll(1, 6);
            //    randomRoll = CalculateWeightedRoll(100, 100, 100, 100, 100, 100);
            //    if (Convert.ToInt32(gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].ToString()) >= 12 && Convert.ToInt32(gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].ToString()) < 17)
            //    {
            //        randomRoll = CalculateWeightedRoll(150, 150, 150, 150, 0, 0);
            //    }
            //    finalLandingSpace = gameController.CalculateLandingSpace(randomRoll, Convert.ToInt32(gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].ToString()));
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
            string rollerName = playerLists.playerNamesInGameDataList[gameVariables.CurrentPlayerIndex].String;
            gameVariables.LogEvent(rollerName + " rolled " + randomRoll);
            gameVariables.RequestSerialization();
        }
    }
    public void CalculateRoll()
    {
        if (!Networking.LocalPlayer.isMaster)
        {
            return;
        }
        int finalLandingSpace = gameController.CalculateLandingSpace(gameVariables.CurrentRoll, Convert.ToInt32(gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].ToString()));

        SpaceSettings spaceSetting = gameController.GetSpace(finalLandingSpace);

        if (isDebugging)
        {
            Debug.Log("GAME OVER!!!");
            gameVariables.WinnerName = playerLists.playerNamesInGameDataList[gameVariables.CurrentPlayerIndex].ToString();
            gameVariables.LogEvent(playerLists.playerNamesInGameDataList[gameVariables.CurrentPlayerIndex].String + " WINS!");
            gameVariables.WinnerDetected++;
            gameController.EndGame();
            return;
        }

        if (finalLandingSpace != 0 && !gameController.IsEnd(finalLandingSpace))
        {
            Debug.Log($"[CalculateRoll] Landed on space {finalLandingSpace}, IsMystery={spaceSetting.IsMystery}, mysteryManager={(mysteryManager != null ? "assigned" : "NULL")}");
            if (spaceSetting.IsMystery && mysteryManager != null)
            {
                Debug.Log($"[CalculateRoll] Starting mystery resolution for space {finalLandingSpace}");
                gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex] = finalLandingSpace;
                gameVariables.RequestSerialization();
                updateSpaces.UpdateOutlineSpaces();
                waitingForMystery = true;
                mysteryManager.StartMysteryAndWait(spaceSetting, finalLandingSpace);
                return;
            }
            else if (spaceSetting.IsMystery && mysteryManager == null)
            {
                Debug.LogWarning("[CalculateRoll] Space is mystery but MysteryManager reference is NULL on RollDiceHelper! Wire it up in the inspector.");
            }
            ProcessLandingEffects(finalLandingSpace, spaceSetting);
        }
        else if (finalLandingSpace == 0)
        {
            gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex] = finalLandingSpace;
            gameController.NextPlayer();
            updateSpaces.UpdateOutlineSpaces();
        }
        else if (gameController.IsEnd(finalLandingSpace))
        {
            Debug.Log("GAME OVER!!!");
            gameVariables.WinnerName = playerLists.playerNamesInGameDataList[gameVariables.CurrentPlayerIndex].ToString();
            gameVariables.LogEvent(playerLists.playerNamesInGameDataList[gameVariables.CurrentPlayerIndex].String + " WINS!");
            gameVariables.WinnerDetected++;
            gameController.EndGame();
        }
    }

    public void ResetMysteryVisual()
    {
        if (mysteryManager != null) mysteryManager.ResetPreviousMysterySpace();
    }
    public void OnMysteryResolved()
    {
        if (!waitingForMystery || !Networking.LocalPlayer.isMaster) return;
        waitingForMystery = false;

        int finalLandingSpace = Convert.ToInt32(gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].ToString());
        SpaceSettings spaceSetting = gameController.GetSpace(finalLandingSpace);
        Debug.Log($"[OnMysteryResolved] Processing effects for space {finalLandingSpace}");
        ProcessLandingEffects(finalLandingSpace, spaceSetting);
    }

    void ProcessLandingEffects(int finalLandingSpace, SpaceSettings spaceSetting)
    {
        bool movementHasEnded = false;
        int numberOfMovements = 0;
        bool wasSentBack = false;
        int lastSwapType = 0;

        while (!movementHasEnded)
        {
            bool sendBackToStart = gameController.ProcessSendBackToStart(spaceSetting);
            int moveForwardBackwards = gameController.ProcessLandedSpaceMovement(spaceSetting);
            int swapPlayer = (int)gameController.ProcessSwapWithPlayer(spaceSetting);
            if (sendBackToStart)
            {
                wasSentBack = true;
                finalLandingSpace = 0;
            }
            else if (moveForwardBackwards != 0)
            {
                finalLandingSpace = gameController.CalculateLandingSpace(moveForwardBackwards, finalLandingSpace);
            }
            else if (swapPlayer != 0)
            {
                lastSwapType = swapPlayer;
                gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex] = finalLandingSpace;
                int playerToSwapIndex = gameController.ProcessSwapPlayer((SwapWithPlayer)swapPlayer, gameVariables.CurrentPlayerIndex);
                if (playerToSwapIndex == gameVariables.CurrentPlayerIndex)
                {
                    movementHasEnded = true;
                }
                else
                {
                    int tempCurrentIndexSpace = Convert.ToInt32(gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].ToString());
                    gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex] = gameVariables.playerSpaceDataList[playerToSwapIndex];
                    gameVariables.playerSpaceDataList[playerToSwapIndex] = tempCurrentIndexSpace;
                    finalLandingSpace = Convert.ToInt32(gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].ToString());
                    if (swapPlayer == (int)SwapWithPlayer.SwapWithFirst)
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
                movementHasEnded = true;
            }
            spaceSetting = gameController.GetSpace(finalLandingSpace);
            numberOfMovements++;
            if (numberOfMovements > 10)
            {
                break;
            }
        }
        gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex] = finalLandingSpace;

        if (gameController.IsEnd(finalLandingSpace))
        {
            Debug.Log("GAME OVER!!! (from ProcessLandingEffects)");
            gameVariables.WinnerName = playerLists.playerNamesInGameDataList[gameVariables.CurrentPlayerIndex].ToString();
            gameVariables.LogEvent(playerLists.playerNamesInGameDataList[gameVariables.CurrentPlayerIndex].String + " WINS!");
            gameVariables.WinnerDetected++;
            gameController.EndGame();
            return;
        }

        int leaderMoveBackTarget = gameController.ProcessLeaderMoveBack(spaceSetting);
        gameController.ProcessLandingLog(spaceSetting, wasSentBack, lastSwapType);
        gameController.ProcessPopup(spaceSetting, wasSentBack, lastSwapType, leaderMoveBackTarget);
        gameController.ProcessMissedTurn(spaceSetting);
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
    private int GetRandomRoll(int min, int max)
    {
        return UnityEngine.Random.Range(min, max + 1);
    }
}
public enum SwapWithPlayer
{
    DontSwap = 0,
    SwapWithFirst = 1,
    SwapWithLast =  2
}