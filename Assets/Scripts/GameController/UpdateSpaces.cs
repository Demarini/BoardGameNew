
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
        for (int j = 0; j < gameVariables.playerSpaceDataList.Count; j++)
        {
            Debug.Log(gameVariables.playerSpaceDataList[j]);
        }
        int previousPlayerSpace = (gameVariables.PreviousPlayerIndex == -1 ? 0 : Convert.ToInt32(gameVariables.playerSpaceDataList[gameVariables.PreviousPlayerIndex].ToString()));
        int selfPlayerSpace = (playerLists.selfIndex != -1 ? Convert.ToInt32(gameVariables.playerSpaceDataList[playerLists.selfIndex].ToString()) : -1);
        for (int i = 0;i < spaceObjects.transform.childCount; i++)
        {
            spaceObjects.transform.GetChild(i).GetChild(7).gameObject.SetActive(false);
            spaceObjects.transform.GetChild(i).GetChild(8).gameObject.SetActive(false);
            spaceObjects.transform.GetChild(i).GetChild(9).gameObject.SetActive(false);
            spaceObjects.transform.GetChild(i).GetChild(10).gameObject.SetActive(false);
        }
        if(gameVariables.PreviousPlayerIndex == -1)
        {
            if(selfPlayerSpace != -1)
            {
                spaceObjects.transform.GetChild(selfPlayerSpace).GetChild(9).gameObject.SetActive(true);
                spaceObjects.transform.GetChild(selfPlayerSpace).GetChild(10).gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("Not in game seemingly...");
            }
        }
        else
        {
            if (gameVariables.PreviousPlayerIndex == playerLists.selfIndex)
            {
                //previous player is self, remove red outline, only outline remaining is ours.
                if (selfPlayerSpace != -1)
                {
                    spaceObjects.transform.GetChild(selfPlayerSpace).GetChild(9).gameObject.SetActive(true);
                    spaceObjects.transform.GetChild(selfPlayerSpace).GetChild(10).gameObject.SetActive(true);
                }
                else
                {
                    Debug.Log("Not in game seemingly...");
                }
            }
            else if (gameVariables.playerSpaceDataList[gameVariables.PreviousPlayerIndex] == (playerLists.selfIndex != -1 ? gameVariables.playerSpaceDataList[playerLists.selfIndex] : -1))
            {
                //The previous player is NOT us, but their space is the same as our space. check to see if self has an index(in game), if they do we can compare, if not, then this isn't getting hit anyways.
                spaceObjects.transform.GetChild(previousPlayerSpace).GetChild(9).gameObject.SetActive(true);
                spaceObjects.transform.GetChild(previousPlayerSpace).GetChild(8).gameObject.SetActive(true);
            }
            else
            {
                //this is if players are on different spaces than we update. check to see if player has index for blue space.
                spaceObjects.transform.GetChild(previousPlayerSpace).GetChild(7).gameObject.SetActive(true);
                spaceObjects.transform.GetChild(previousPlayerSpace).GetChild(8).gameObject.SetActive(true);
                if (selfPlayerSpace != -1)
                {
                    spaceObjects.transform.GetChild(selfPlayerSpace).GetChild(9).gameObject.SetActive(true);
                    spaceObjects.transform.GetChild(selfPlayerSpace).GetChild(10).gameObject.SetActive(true);
                }
                else
                {
                    Debug.Log("Not in game seemingly...");
                }
            }
        }
    }
}