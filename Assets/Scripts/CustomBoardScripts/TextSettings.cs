
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TextSettings : UdonSharpBehaviour
{
    public string RollAgainText = "Roll Again";
    public string DrinkXTimesText = "Drink {x}";
    public string SendBackToStartText = "Go Back to Start";
    public string EveryoneDrinkXTimesText = "Everyone Drinks {x}";
    public string MoveBackXSpacesText = "Go Back {x} Spaces";
    public string MoveForwardXSpacesText = "Go to Space {x}";
    public string DrinkWhatYouRollText = "Drink What You Roll";
    public string SwapWithLastText = "Swap With Last Place";
    public string SwapWithFirstText = "Swap With First Place";
    public string ImmuneFromDrinkingText = "Immune From Drinking";
    public string MissTurnText = "Miss a Turn";
    public string DrinkWithHostText = "Drink With the Host";
    public string ChooseSomeoneToDrinkText = "Choose Someone to Drink";
    public string GirlsDrinkText = "Girls Drink";
    public string GuysDrinkText = "Guys Drink";
    public string FinishText = "Finish";
    void Start()
    {
        
    }
}