
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class UpdatePlayerCamerasOnSpace_BoardGame : UdonSharpBehaviour
{
    [SerializeField] PlayerList_BoardGame playerLists;
    [SerializeField] GameVariables_BoardGame gameVariables;
    public GameObject[] boardGameSpaces;

    public void UpdateCameraCountOnSpaces()
    {
        int indexToEnable = GetIndexToUpdate();
        
        for(int i = 0;i < boardGameSpaces.Length; i++)
        {
            for(int k = 0; k < 7; k++)
            {
                if(k != indexToEnable)
                {
                    boardGameSpaces[i].transform.GetChild(i).GetChild(k).gameObject.SetActive(false);
                }
                else
                {
                    boardGameSpaces[i].transform.GetChild(i).GetChild(k).gameObject.SetActive(true);
                }
            }
        }
    }
    public void UpdatePlayerSpaces()
    {
        int indexToUpdate = GetIndexToUpdate();
        if(gameVariables.PreviousPlayerIndex != -1)
        {
            boardGameSpaces[Convert.ToInt32(gameVariables.playerSpaceDataList[gameVariables.PreviousPlayerIndex].Double)].transform.GetChild(indexToUpdate).GetChild(gameVariables.PreviousPlayerIndex).gameObject.SetActive(false);
        }
        if (gameVariables.CurrentPlayerIndex != -1)
        {
            boardGameSpaces[Convert.ToInt32(gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].Double)].transform.GetChild(indexToUpdate).GetChild(gameVariables.CurrentPlayerIndex).gameObject.SetActive(false);
        }
        for(int i = 0;i < gameVariables.playerSpaceDataList.Count; i++)
        {
            boardGameSpaces[Convert.ToInt32(gameVariables.playerSpaceDataList[i].Double)].transform.GetChild(indexToUpdate).GetChild(i).gameObject.SetActive(true);
        }
    }
    int GetIndexToUpdate()
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