
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CameraFollowHead : UdonSharpBehaviour
{
    [SerializeField] GameVariables_BoardGame gameVariables;
    [SerializeField] PlayerList_BoardGame playerLists;
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
        takePicture = true;
    }
    void TurnOffCamera()
    {
        for (int i = 0; i < playerLists.playersInGameDataList.Count; i++)
        {
            cameras[i].SetActive(false);
        }
    }
    void SetCameraToPlayerHead()
    {
        for (int i = 0; i < playerLists.playersInGameDataList.Count; i++)
        {
            VRCPlayerApi player = VRCPlayerApi.GetPlayerById(Convert.ToInt32(playerLists.playersInGameDataList[i].Double));
            VRCPlayerApi.TrackingData trackingData = player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
            cameras[i].SetActive(true);
            cameras[i].transform.position = trackingData.position;
            cameras[i].transform.rotation = player.GetRotation();
        }
    }
}
