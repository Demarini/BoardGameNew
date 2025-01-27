﻿
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RollDiceInteract_BoardGame : UdonSharpBehaviour
{
    [SerializeField] GameFunctions_BoardGame gameFunctions;
    public GameObject diceInteract;
    public override void Interact()
    {
        //Debug.Log("Dice Interact Clicked");
        diceInteract.SetActive(false);
        gameFunctions.RollDiceClicked();
    }
}