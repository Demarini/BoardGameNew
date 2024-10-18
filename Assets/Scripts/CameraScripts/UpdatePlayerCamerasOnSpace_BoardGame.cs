
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class UpdatePlayerCamerasOnSpace_BoardGame : UdonSharpBehaviour
{
    [SerializeField] PlayerList_BoardGame playerLists;
    [SerializeField] GameVariables_BoardGame gameVariables;
    GameObject[] boardGameSpaces;
    public GameObject[] displayPanelPlayerCameras;
    public GameObject boardGameSpacesObject;
    public int previousSpaceToDisable = 0;
    public int previousPlayerToDisable = 0;
    public void Start()
    {
        boardGameSpaces = new GameObject[boardGameSpacesObject.transform.childCount];
        for(int i = 0;i < boardGameSpacesObject.transform.childCount; i++)
        {
            boardGameSpaces[i] = boardGameSpacesObject.transform.GetChild(i).gameObject;
        }
    }
    public void UpdateDisplayPanelCameras()
    {
        for(int i = 0;i < displayPanelPlayerCameras.Length; i++)
        {
            displayPanelPlayerCameras[i].SetActive(false);
        }
        displayPanelPlayerCameras[gameVariables.CurrentPlayerIndex].SetActive(true);
    }
    public void UpdateCameraCountOnSpaces()
    {
        int indexToEnable = GetIndexToUpdate();
        Debug.Log("Index to enable: " + indexToEnable.ToString());
        for(int i = 0;i < boardGameSpaces.Length; i++)
        {
            Debug.Log("Updating camera object on space " + i.ToString());
            for(int k = 0; k < 7; k++)
            {
                if(k != indexToEnable)
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
    public void UpdatePlayerSpaces()
    {
        int indexToUpdate = GetIndexToUpdate();

        //if(gameVariables.PreviousPlayerIndex != -1)
        //{
        //    Debug.Log("Disabling Previous Index of " + gameVariables.PreviousPlayerIndex.ToString() + " on space " + gameVariables.playerSpaceDataList[gameVariables.PreviousPlayerIndex]);
        //    boardGameSpaces[Convert.ToInt32(gameVariables.playerSpaceDataList[gameVariables.PreviousPlayerIndex].Double)].transform.GetChild(indexToUpdate).GetChild(gameVariables.PreviousPlayerIndex).gameObject.SetActive(false);
        //}
        //if (gameVariables.CurrentPlayerIndex != -1)
        //{
        //    Debug.Log("Disabling Current Index of " + gameVariables.CurrentPlayerIndex.ToString() + " on space " + gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex]);
        //    boardGameSpaces[Convert.ToInt32(gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].Double)].transform.GetChild(indexToUpdate).GetChild(gameVariables.CurrentPlayerIndex).gameObject.SetActive(false);
        //}
        Debug.Log("Previous Space to Disable in Update: " + previousSpaceToDisable.ToString());
        Debug.Log("Previous Player to Disable in Update: " + previousPlayerToDisable.ToString());
        for(int k = 0;k < boardGameSpaces[previousSpaceToDisable].transform.GetChild(indexToUpdate).childCount; k++)
        {
            boardGameSpaces[previousSpaceToDisable].transform.GetChild(indexToUpdate).GetChild(k).gameObject.SetActive(false);
        }
        Debug.Log("THIS VALUE IS BREAKING THINGS: " + gameVariables.playerSpaceDataList[gameVariables.PreviousPlayerIndex].ToString());
        if(gameVariables.PreviousPlayerIndex != -1)
        {
            for (int k = 0; k < boardGameSpaces[Convert.ToInt32(gameVariables.playerSpaceDataList[gameVariables.PreviousPlayerIndex].ToString())].transform.GetChild(indexToUpdate).childCount; k++)
            {
                boardGameSpaces[Convert.ToInt32(gameVariables.playerSpaceDataList[gameVariables.PreviousPlayerIndex].Double)].transform.GetChild(indexToUpdate).GetChild(k).gameObject.SetActive(false);
            }
        }
        for (int i = 0;i < gameVariables.playerSpaceDataList.Count; i++)
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