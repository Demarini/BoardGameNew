
using System.Collections.Generic;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Data;

public class TestScript : UdonSharpBehaviour
{
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
            Debug.Log("TEST SERIALIZE PLAYER LIST");
            PlayerJson = result.String;
            Debug.Log(PlayerJson);
        }
        else
        {
            Debug.LogError(result.ToString());
        }
    }
    private void SerializeDataToken(DataToken token)
    {

    }
}