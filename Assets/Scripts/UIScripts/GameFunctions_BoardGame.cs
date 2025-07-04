using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class GameFunctions_BoardGame : UdonSharpBehaviour
{
    [SerializeField] GameController_BoardGame gameController;
    [SerializeField] PlayerFunctions_BoardGame playerFunctions;
    [SerializeField] GameVariables_BoardGame gameVariables;
    [SerializeField] PlayerList_BoardGame playerLists;
    [SerializeField] CameraFollowHead cameraFollowHead;
    [SerializeField] GameObject clickAudio;
    [SerializeField] GameObject musicPlayer;
    public void JoinClicked() { PlayClick(); playerFunctions.SendPlayerToMasterAdd(Networking.LocalPlayer.playerId); }

    public void LeaveClicked() { PlayClick(); playerFunctions.SendPlayerToMasterRemove(Networking.LocalPlayer.playerId); }

    public GameObject audioSources;

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

    public void ForceRemovePlayer()
    {
        if (Networking.LocalPlayer.isMaster)
        {
            PlayClick();
            playerFunctions.RemovePlayer(Convert.ToInt32(playerLists.playersInGameDataList[gameVariables.CurrentPlayerIndex].ToString()), true);
        }
    }
    public void ToggleMusic()
    {
        PlayClick();
        audioSources.SetActive(!audioSources.activeSelf);
    }
    public void ToggleMusic2()
    {
        if (Networking.LocalPlayer.isMaster)
        {
            gameVariables.MusicPlayerEnabled = true;
            gameVariables.RequestSerialization();
            PlayClick();
        }
    }
    public void TurnOnMusic()
    {
        if (Networking.LocalPlayer.isMaster)
        {
            gameVariables.MusicPlayerEnabled = false;
            gameVariables.RequestSerialization();
            PlayClick();
        }
    }
    public void TurnOffMusic()
    {
        if (Networking.LocalPlayer.isMaster)
        {
            gameVariables.MusicPlayerEnabled = false;
            gameVariables.RequestSerialization();
            PlayClick();
        }
    }
}