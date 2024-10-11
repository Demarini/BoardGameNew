
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SpaceSettings : UdonSharpBehaviour
{
    public bool RollAgain;
    public int DrinkXTimes;
    public bool SendBackToStart;
    public int EveryoneDrinkXTimes;
    public int MoveBackXSpaces;
    public int MoveForwardXSpaces;
    public bool DrinkWhatYouRoll;
    public bool SwapWithLast;
    public bool SwapWithFirst;
    public bool ImmuneFromDrinking;
    public bool MissTurn;
    public bool DrinkWithHost;
    public bool ChooseSomeoneToDrink;
    public bool GirlsDrink;
    public bool GuysDrink;
    public bool Finish;
    public bool Start;
}