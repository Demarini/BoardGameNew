using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class GameFunctions_BoardGame : UdonSharpBehaviour
{
    [SerializeField] GameController_BoardGame gameController;
    [SerializeField] PlayerFunctions_BoardGame playerFunctions;
    [SerializeField] CameraFollowHead cameraFollowHead;
    [SerializeField] GameObject clickAudio;
    public void JoinClicked() { PlayClick(); playerFunctions.SendPlayerToMasterAdd(Networking.LocalPlayer.playerId); }

    public void LeaveClicked() { PlayClick(); playerFunctions.SendPlayerToMasterRemove(Networking.LocalPlayer.playerId); }

    public void StartGameClicked()
    {
        if (!Networking.LocalPlayer.isMaster)
        {
            return;
        }
        PlayClick();
        gameController.StartGame();
    }
    public void EndGameClicked()
    {
        if (!Networking.LocalPlayer.isMaster)
        {
            return;
        }
        PlayClick();
        gameController.EndGame();
    }

    public void RollDiceClicked() { PlayClick(); gameController.RollDice(); }

    public void NextPlayerClicked()
    {
        if (!Networking.LocalPlayer.isMaster)
        {
            return;
        }
        PlayClick();
        gameController.NextPlayer();
    }

    public void UpdatePicturesClicked() { PlayClick(); cameraFollowHead.TakePicture(); }

    void PlayClick()
    {   
        clickAudio.SetActive(false);
        clickAudio.SetActive(true);
    }

    public void PreviousPlayer()
    {

    }
}