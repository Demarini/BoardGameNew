
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ToggleGameAudio_BoardGame : UdonSharpBehaviour
{
    [SerializeField] GameVariables_BoardGame gameVariables;
    [SerializeField] PlayerList_BoardGame playerLists;

    public GameObject ChooseSomeoneToDrink;
    public GameObject Drink;
    public GameObject DrinkWithHost;
    public GameObject EveryoneDrink;
    public GameObject GirlsDrink;
    public GameObject GuysDrink;
    public GameObject SendBackToStart;
    public GameObject SwapWithFirst;
    public GameObject SwapWithLast;
    public GameObject RollAgain;
    public GameObject DrinkWhatYouRoll;
    public GameObject Immune;
    public GameObject MissTurn;

    public GameObject idleAudio;

    public void ToggleAudio()
    {
        if (!gameVariables.hasLoadedForFirstTime)
        {
            gameVariables.hasLoadedForFirstTime = true;
            gameVariables.tmpToggleChooseSomeoneToDrink = gameVariables.ToggleChooseSomeoneToDrink;
            gameVariables.tmpToggleDrink = gameVariables.ToggleDrink;
            gameVariables.tmpToggleDrinkWithHost = gameVariables.ToggleDrinkWithHost;
            gameVariables.tmpToggleEveryoneDrink = gameVariables.ToggleEveryoneDrink;
            gameVariables.tmpToggleGirlsDrink = gameVariables.ToggleGirlsDrink;
            gameVariables.tmpToggleGuysDrink = gameVariables.ToggleGuysDrink;
            gameVariables.tmpToggleSendBackToStart = gameVariables.ToggleSendBackToStart;
            gameVariables.tmpToggleSwapWithFirst = gameVariables.ToggleSwapWithFirst;
            gameVariables.tmpToggleSwapWithLast = gameVariables.ToggleSwapWithLast;
            gameVariables.tmpToggleRollAgain = gameVariables.ToggleRollAgain;
            gameVariables.tmpToggleImmune = gameVariables.ToggleImmune;
            gameVariables.tmpToggleDrinkWhatYouRoll = gameVariables.ToggleDrinkWhatYouRoll;
            gameVariables.tmpToggleMissTurn = gameVariables.ToggleMissTurn;
        }
        else
        {
            bool isPlayerInGame = false;
            for (int i = 0; i < playerLists.playersInGameDataList.Count; i++)
            {
                if(Networking.LocalPlayer.playerId == Convert.ToInt32(playerLists.playersInGameDataList[i].ToString()))
                {
                    if(playerLists.playerStatusInGameDataList[i] == 0)
                    {
                        isPlayerInGame = true;
                    }
                }
            }
            if (!isPlayerInGame)
            {
                gameVariables.tmpToggleChooseSomeoneToDrink = gameVariables.ToggleChooseSomeoneToDrink;
                gameVariables.tmpToggleDrink = gameVariables.ToggleDrink;
                gameVariables.tmpToggleDrinkWithHost = gameVariables.ToggleDrinkWithHost;
                gameVariables.tmpToggleEveryoneDrink = gameVariables.ToggleEveryoneDrink;
                gameVariables.tmpToggleGirlsDrink = gameVariables.ToggleGirlsDrink;
                gameVariables.tmpToggleGuysDrink = gameVariables.ToggleGuysDrink;
                gameVariables.tmpToggleSwapWithFirst = gameVariables.ToggleSwapWithFirst;
                gameVariables.tmpToggleSwapWithLast = gameVariables.ToggleSwapWithLast;
                gameVariables.tmpToggleRollAgain = gameVariables.ToggleRollAgain;
                gameVariables.tmpToggleImmune = gameVariables.ToggleImmune;
                gameVariables.tmpToggleDrinkWhatYouRoll = gameVariables.ToggleDrinkWhatYouRoll;
                gameVariables.tmpToggleMissTurn = gameVariables.ToggleMissTurn;
                return;
            }
            else if (gameVariables.tmpSwapWithFirstIncrement != gameVariables.SwapWithFirstIncrement)
            {
                ToggleGameObject(SwapWithFirst);
            }
            else if(gameVariables.tmpSwapWithLastIncrement != gameVariables.SwapWithLastIncrement)
            {
                ToggleGameObject(SwapWithLast);
            }
            else if (gameVariables.tmpToggleChooseSomeoneToDrink != gameVariables.ToggleChooseSomeoneToDrink)
            {
                if (gameVariables.PreviousPlayerIndex == playerLists.selfIndex)
                {
                    ToggleGameObject(ChooseSomeoneToDrink);
                }
            }
            else if (gameVariables.tmpToggleDrink != gameVariables.ToggleDrink)
            {
                if (gameVariables.PreviousPlayerIndex == playerLists.selfIndex)
                {
                    ToggleGameObject(Drink);
                }
            }
            else if (gameVariables.tmpToggleDrinkWithHost != gameVariables.ToggleDrinkWithHost)
            {
                if (gameVariables.PreviousPlayerIndex == playerLists.selfIndex)
                {
                    ToggleGameObject(DrinkWithHost);
                }
            }
            else if (gameVariables.tmpToggleEveryoneDrink != gameVariables.ToggleEveryoneDrink)
            {
                ToggleGameObject(EveryoneDrink);
            }
            else if (gameVariables.tmpToggleGirlsDrink != gameVariables.ToggleGirlsDrink)
            {
                ToggleGameObject(GirlsDrink);
            }
            else if (gameVariables.tmpToggleGuysDrink != gameVariables.ToggleGuysDrink)
            {
                ToggleGameObject(GuysDrink);
            }
            else if (gameVariables.tmpToggleSendBackToStart != gameVariables.ToggleSendBackToStart)
            {
                ToggleGameObject(SendBackToStart);
            }
            //if(gameVariables.tmpToggleSwapWithFirst != gameVariables.ToggleSwapWithFirst)
            //{
            //    ToggleGameObject(SwapWithFirst);
            //}
            //if (gameVariables.tmpToggleSwapWithLast != gameVariables.ToggleSwapWithLast)
            //{
            //    ToggleGameObject(SwapWithLast);
            //}
            else if (gameVariables.tmpToggleRollAgain != gameVariables.ToggleRollAgain)
            {
                if (gameVariables.PreviousPlayerIndex == playerLists.selfIndex)
                {
                    ToggleGameObject(RollAgain);
                }
            }
            else if (gameVariables.tmpToggleImmune != gameVariables.ToggleImmune)
            {
                if (gameVariables.PreviousPlayerIndex == playerLists.selfIndex)
                {
                    ToggleGameObject(Immune);
                }
            }
            else if (gameVariables.tmpToggleDrinkWhatYouRoll != gameVariables.ToggleDrinkWhatYouRoll)
            {
                if (gameVariables.PreviousPlayerIndex == playerLists.selfIndex)
                {
                    ToggleGameObject(DrinkWhatYouRoll);
                }
            }
            else if (gameVariables.tmpToggleMissTurn != gameVariables.ToggleMissTurn)
            {
                if (gameVariables.PreviousPlayerIndex == playerLists.selfIndex)
                {
                    ToggleGameObject(MissTurn);
                }
            }
            gameVariables.tmpToggleChooseSomeoneToDrink = gameVariables.ToggleChooseSomeoneToDrink;
            gameVariables.tmpToggleDrink = gameVariables.ToggleDrink;
            gameVariables.tmpToggleDrinkWithHost = gameVariables.ToggleDrinkWithHost;
            gameVariables.tmpToggleEveryoneDrink = gameVariables.ToggleEveryoneDrink;
            gameVariables.tmpToggleGirlsDrink = gameVariables.ToggleGirlsDrink;
            gameVariables.tmpToggleGuysDrink = gameVariables.ToggleGuysDrink;
            gameVariables.tmpToggleSendBackToStart = gameVariables.ToggleSendBackToStart;
            gameVariables.tmpToggleSwapWithFirst = gameVariables.ToggleSwapWithFirst;
            gameVariables.tmpToggleSwapWithLast = gameVariables.ToggleSwapWithLast;
            gameVariables.tmpToggleRollAgain = gameVariables.ToggleRollAgain;
            gameVariables.tmpToggleImmune = gameVariables.ToggleImmune;
            gameVariables.tmpToggleDrinkWhatYouRoll = gameVariables.ToggleDrinkWhatYouRoll;
            gameVariables.tmpToggleMissTurn = gameVariables.ToggleMissTurn;
            gameVariables.tmpSwapWithFirstIncrement = gameVariables.SwapWithFirstIncrement;
            gameVariables.tmpSwapWithLastIncrement = gameVariables.SwapWithLastIncrement;
        }
    }
    void ToggleGameObject(GameObject objectToToggle)
    {
        objectToToggle.SetActive(false);
        objectToToggle.SetActive(true);
    }
    public void ToggleIdleAudioOn()
    {
        idleAudio.SetActive(true);
    }
    public void ToggleIdleAudioOff()
    {
        idleAudio.SetActive(false);
    }
    public void ToggleSwapLastAudio()
    {
        SwapWithLast.SetActive(false);
        SwapWithLast.SetActive(true);
    }
    public void ToggleSwapFirstAudio()
    {
        SwapWithLast.SetActive(false);
        SwapWithLast.SetActive(true);
    }
}
