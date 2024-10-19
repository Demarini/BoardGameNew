
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CameraFollowHead : UdonSharpBehaviour
{
    [SerializeField] GameVariables_BoardGame gameVariables;
    [SerializeField] PlayerList_BoardGame playerLists;
    [SerializeField] UpdatePlayerCamerasOnSpace_BoardGame updatePlayerCamerasOnSpace;
    public GameObject[] cameras;
    bool takePicture;
    bool cameraSet;
    private void Update()
    {
        if (takePicture)
        {
            SetCameraToPlayerHead();
            if (cameraSet)
            {
                TurnOffCamera();
                cameraSet = false;
                takePicture = false;
            }
            else
            {
                cameraSet = true;
            }
            
        }
        
    }
    public void TakePicture()
    {
        updatePlayerCamerasOnSpace.UpdateCameraCountOnSpaces();
        takePicture = true;
    }
    void TurnOffCamera()
    {
        if (gameVariables.ReceivedAllVariables)
        {
            for (int i = 0; i < playerLists.playersInGameDataList.Count; i++)
            {
                cameras[i].SetActive(false);
            }
        }
    }
    void SetCameraToPlayerHead()
    {
        if (gameVariables.ReceivedAllVariables)
        {
            for (int i = 0; i < playerLists.playersInGameDataList.Count; i++)
            {
                if(Convert.ToInt32(playerLists.playerStatusInGameDataList[i].ToString()) == 0)
                {
                    Debug.Log("Player In Game: " + playerLists.playersInGameDataList[i].ToString());
                    VRCPlayerApi player = VRCPlayerApi.GetPlayerById(Convert.ToInt32(playerLists.playersInGameDataList[i].ToString()));
                    if(player == null)
                    {
                        Debug.Log("Player is Null For Camera");
                        //this is most likely self, give it a check.
                        if(playerLists.playerNamesInGameDataList[i].ToString() == Networking.LocalPlayer.displayName)
                        {
                            Debug.Log("Found Local Player");
                            VRCPlayerApi.TrackingData trackingData = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
                            cameras[i].SetActive(true);
                            cameras[i].transform.position = trackingData.position;
                            cameras[i].transform.rotation = player.GetRotation();
                        }
                    }
                    else
                    {
                        VRCPlayerApi.TrackingData trackingData = player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
                        cameras[i].SetActive(true);
                        cameras[i].transform.position = trackingData.position;
                        cameras[i].transform.rotation = player.GetRotation();
                    }
                    
                }
            }
        }
    }
}
