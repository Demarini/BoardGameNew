
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class UpdateSpaces : UdonSharpBehaviour
{
    public GameObject spaceObjects;
    [SerializeField] GameVariables_BoardGame gameVariables;
    [SerializeField] PlayerList_BoardGame playerLists;
    public void UpdateOutlineSpaces()
    {
        Debug.Log("UPDATE OUTLINE SPACE"); 
        Debug.Log("Previous Player Index: " + gameVariables.PreviousPlayerIndex.ToString());
        Debug.Log("Self Index " + playerLists.selfIndex.ToString());
        Debug.Log("Previous Player Space: " + gameVariables.playerSpaceDataList[gameVariables.PreviousPlayerIndex].ToString());
        Debug.Log("Current Player Space: " + gameVariables.playerSpaceDataList[playerLists.selfIndex].ToString());
        for(int j = 0;j < gameVariables.playerSpaceDataList.Count; j++)
        {
            Debug.Log(gameVariables.playerSpaceDataList[j]);
        }
        TokenType t = gameVariables.playerSpaceDataList[gameVariables.PreviousPlayerIndex].TokenType;
        TokenType t2 = gameVariables.playerSpaceDataList[playerLists.selfIndex].TokenType;
        Debug.Log("Token Types");
        Debug.Log(t.ToString());
        Debug.Log(t2.ToString());
        int previousPlayerSpace = Convert.ToInt32(gameVariables.playerSpaceDataList[gameVariables.PreviousPlayerIndex].ToString());
        int selfPlayerSpace = Convert.ToInt32(gameVariables.playerSpaceDataList[playerLists.selfIndex].ToString());
        for (int i = 0;i < spaceObjects.transform.childCount - 1; i++)
        {
            spaceObjects.transform.GetChild(i).GetChild(7).gameObject.SetActive(false);
            spaceObjects.transform.GetChild(i).GetChild(8).gameObject.SetActive(false);
            spaceObjects.transform.GetChild(i).GetChild(9).gameObject.SetActive(false);
            spaceObjects.transform.GetChild(i).GetChild(10).gameObject.SetActive(false);
        }
        if(gameVariables.PreviousPlayerIndex == playerLists.selfIndex)
        {
            spaceObjects.transform.GetChild(selfPlayerSpace).GetChild(9).gameObject.SetActive(true);
            spaceObjects.transform.GetChild(selfPlayerSpace).GetChild(10).gameObject.SetActive(true);
        }
        else if(gameVariables.playerSpaceDataList[gameVariables.PreviousPlayerIndex] == gameVariables.playerSpaceDataList[playerLists.selfIndex])
        {
            spaceObjects.transform.GetChild(selfPlayerSpace).GetChild(9).gameObject.SetActive(true);
            spaceObjects.transform.GetChild(selfPlayerSpace).GetChild(8).gameObject.SetActive(true);
        }
        else
        {
            spaceObjects.transform.GetChild(previousPlayerSpace).GetChild(7).gameObject.SetActive(true);
            spaceObjects.transform.GetChild(previousPlayerSpace).GetChild(8).gameObject.SetActive(true);
            spaceObjects.transform.GetChild(selfPlayerSpace).GetChild(9).gameObject.SetActive(true);
            spaceObjects.transform.GetChild(selfPlayerSpace).GetChild(10).gameObject.SetActive(true);
        }
        
    }
}