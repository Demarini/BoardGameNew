using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class GameFunctions_BoardGame : UdonSharpBehaviour
{
    [SerializeField] GameController_BoardGame gameController;
    [SerializeField] PlayerFunctions_BoardGame playerFunctions;
    
    public void JoinClicked() => playerFunctions.SendPlayerToMasterAdd(Networking.LocalPlayer.playerId);

    public void LeaveClicked() => playerFunctions.SendPlayerToMasterRemove(Networking.LocalPlayer.playerId);

    public void StartGameClicked() => gameController.StartGame();

    public void RollDiceClicked() => gameController.RollDice();
    public void NextPlayer()
    {
        gameController.NextPlayer();
    }
    public void PreviousPlayer()
    {

    }
}