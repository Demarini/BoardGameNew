
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class HelperFunctions_BoardGame : UdonSharpBehaviour
{
    public DataList DeserializeDataList(string dataListJson, DataList previousDataList)
    {
        if (VRCJson.TryDeserializeFromJson(dataListJson, out DataToken result))
        {
            return result.DataList;
        }
        else
        {
            return previousDataList;
        }
    }
    public string SerializeDataList(DataList list, string previousString)
    {
        string returnJson = "";
        if (VRCJson.TrySerializeToJson(list, JsonExportType.Minify, out DataToken result))
        {
            returnJson = result.ToString();
        }
        else
        {
            returnJson = previousString;
        }
        Debug.Log(returnJson);
        return returnJson;
    }
}
