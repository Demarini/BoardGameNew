
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class UpdatePlayerCamerasOnSpace_BoardGame : UdonSharpBehaviour
{
    [SerializeField] PlayerList_BoardGame playerLists;
    [SerializeField] GameVariables_BoardGame gameVariables;
    [SerializeField] SyncedHelper syncedHelper;
    GameObject[] boardGameSpaces;
    public GameObject[] displayPanelPlayerCameras;
    public GameObject boardGameSpacesObject;
    public int previousSpaceToDisable = 0;
    public int previousPlayerToDisable = 0;
    bool spacesCleared = false;
    public void Start()
    {
        boardGameSpaces = new GameObject[boardGameSpacesObject.transform.childCount];
        for (int i = 0; i < boardGameSpacesObject.transform.childCount; i++)
        {
            boardGameSpaces[i] = boardGameSpacesObject.transform.GetChild(i).gameObject;
        }
    }
    public void UpdateDisplayPanelCameras()
    {
        if (!syncedHelper.IsReady()) return;
        if (gameVariables.CurrentPlayerIndex != -1)
        {
            for (int i = 0; i < displayPanelPlayerCameras.Length; i++)
            {
                displayPanelPlayerCameras[i].SetActive(false);
            }
            if (displayPanelPlayerCameras.Length > 0)
            {
                if (gameVariables.CurrentPlayerIndex >= 0)
                {
                    displayPanelPlayerCameras[gameVariables.CurrentPlayerIndex].SetActive(true);
                }   
            }
        }
    }
    public void UpdateCameraCountOnSpaces()
    {
        if (!syncedHelper.IsReady()) return;
        spacesCleared = true;
        int indexToEnable = GetIndexToUpdate();
        Debug.Log("Index to enable: " + indexToEnable.ToString());
        for (int i = 0; i < boardGameSpaces.Length; i++)
        {
            //Debug.Log("Updating camera object on space " + i.ToString());
            for (int k = 0; k < 7; k++)
            {
                if (k != indexToEnable)
                {
                    boardGameSpaces[i].transform.GetChild(k).gameObject.SetActive(false);
                }
                else
                {
                    boardGameSpaces[i].transform.GetChild(k).gameObject.SetActive(true);
                }
            }
        }
    }
    public void ClearAllSpacesOfPictures()
    {
        if (!spacesCleared)
        {
            for (int i = 0; i < boardGameSpaces.Length; i++)
            {
                //Debug.Log("Updating camera object on space " + i.ToString());
                for (int k = 0; k < 7; k++)
                {
                    for (int j = 0; j < boardGameSpaces[i].transform.GetChild(k).childCount; j++)
                    {
                        boardGameSpaces[i].transform.GetChild(k).GetChild(j).gameObject.SetActive(false);
                    }
                }
            }
            spacesCleared = true;
        }
        
    }
    public void UpdatePlayerSpaces()
    {
        if (!syncedHelper.IsReady()) return;
        int indexToUpdate = GetIndexToUpdate();
        spacesCleared = true;
        for (int i = 0; i < boardGameSpaces.Length; i++)
        {
            //Debug.Log("Updating camera object on space " + i.ToString());
            for (int k = 0; k < 7; k++)
            {
                if (k != indexToUpdate)
                {
                    boardGameSpaces[i].transform.GetChild(k).gameObject.SetActive(false);
                }
                else
                {
                    boardGameSpaces[i].transform.GetChild(k).gameObject.SetActive(true);
                    for (int j = 0; j < boardGameSpaces[i].transform.GetChild(k).childCount; j++)
                    {
                        boardGameSpaces[i].transform.GetChild(k).GetChild(j).gameObject.SetActive(false);
                    }
                }
            }
        }
        for (int i = 0; i < gameVariables.playerSpaceDataList.Count; i++)
        {
            boardGameSpaces[Convert.ToInt32(gameVariables.playerSpaceDataList[i].Double)].transform.GetChild(indexToUpdate).GetChild(i).gameObject.SetActive(true);
        }
    }
    public int GetIndexToUpdate()
    {
        int indexToUpdate = 0;
        if (playerLists.playersInGameDataList.Count > 49)
        {
            indexToUpdate = 6;
        }
        else if (playerLists.playersInGameDataList.Count > 36)
        {
            indexToUpdate = 5;
        }
        else if (playerLists.playersInGameDataList.Count > 25)
        {
            indexToUpdate = 4;
        }
        else if (playerLists.playersInGameDataList.Count > 16)
        {
            indexToUpdate = 3;
        }
        else if (playerLists.playersInGameDataList.Count > 9)
        {
            indexToUpdate = 2;
        }
        else if (playerLists.playersInGameDataList.Count > 4)
        {
            indexToUpdate = 1;
        }
        else
        {
            indexToUpdate = 0;
        }
        return indexToUpdate;
    }
}