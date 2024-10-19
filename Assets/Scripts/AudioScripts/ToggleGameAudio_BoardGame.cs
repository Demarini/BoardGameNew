
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
        }
        else
        {
            if (gameVariables.tmpToggleChooseSomeoneToDrink != gameVariables.ToggleChooseSomeoneToDrink)
            {
                if(gameVariables.PreviousPlayerIndex == playerLists.selfIndex)
                {
                    ToggleGameObject(ChooseSomeoneToDrink);
                }
            }
            if (gameVariables.tmpToggleDrink != gameVariables.ToggleDrink)
            {
                if (gameVariables.PreviousPlayerIndex == playerLists.selfIndex)
                {
                    ToggleGameObject(Drink);
                }
            }
            if (gameVariables.tmpToggleDrinkWithHost != gameVariables.ToggleDrinkWithHost)
            {
                if (gameVariables.PreviousPlayerIndex == playerLists.selfIndex)
                {
                    ToggleGameObject(DrinkWithHost);
                }
            }
            if (gameVariables.tmpToggleEveryoneDrink != gameVariables.ToggleEveryoneDrink)
            {
                ToggleGameObject(EveryoneDrink);
            }
            if (gameVariables.tmpToggleGirlsDrink != gameVariables.ToggleGirlsDrink)
            {
                ToggleGameObject(GirlsDrink);
            }
            if (gameVariables.tmpToggleGuysDrink != gameVariables.ToggleGuysDrink)
            {
                ToggleGameObject(GuysDrink);
            }
            gameVariables.tmpToggleChooseSomeoneToDrink = gameVariables.ToggleChooseSomeoneToDrink;
            gameVariables.tmpToggleDrink = gameVariables.ToggleDrink;
            gameVariables.tmpToggleDrinkWithHost = gameVariables.ToggleDrinkWithHost;
            gameVariables.tmpToggleEveryoneDrink = gameVariables.ToggleEveryoneDrink;
            gameVariables.tmpToggleGirlsDrink = gameVariables.ToggleGirlsDrink;
            gameVariables.tmpToggleGuysDrink = gameVariables.ToggleGuysDrink;
        }
    }
    void ToggleGameObject(GameObject objectToToggle)
    {
        objectToToggle.SetActive(false);
        objectToToggle.SetActive(true);
    }
}
