
using System.Collections.Generic;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Data;

public class TestScript : UdonSharpBehaviour
{
    public GameObject[] objectsToToggle;
    bool toggle;
    public void ToggleObjects()
    {
        float timer = Time.realtimeSinceStartup;
        toggle = !toggle;
        int count = 0;
        for(int i = 0; i< objectsToToggle.Length; i++)
        {
            objectsToToggle[i].SetActive(toggle);
            for(int k = 0;k < objectsToToggle[i].transform.childCount; k++)
            {
                objectsToToggle[i].transform.GetChild(k).gameObject.SetActive(toggle);
                count++;
                //Debug.Log(count);
            }
        }
        //Debug.Log("Total Time Ran: " + (Time.realtimeSinceStartup - timer).ToString());
    }
    [SerializeField] Player[] players;
    [UdonSynced, FieldChangeCallback(nameof(PlayerJson))]
    private string playerJson;
    public string PlayerJson
    {
        set
        {
            playerJson = value;
        }
        get => playerJson;
    }
    DataList playersDataList = new DataList();
    void Start()
    {
        foreach(Player p in players)
        {
            playersDataList.Add(p);
        }
        if (VRCJson.TrySerializeToJson(playersDataList, JsonExportType.Minify, out DataToken result))
        {
            //Debug.Log("TEST SERIALIZE PLAYER LIST");
            PlayerJson = result.String;
            //Debug.Log(PlayerJson);
        }
        else
        {
            //Debug.LogError(result.ToString());
        }
    }
    private void SerializeDataToken(DataToken token)
    {

    }
}