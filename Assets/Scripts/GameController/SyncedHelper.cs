
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SyncedHelper : UdonSharpBehaviour
{
    [SerializeField] GameVariables_BoardGame gameVariables;
    [SerializeField] PlayerList_BoardGame playerLists;
    void Start()
    {
        
    }
    public bool IsReady()
    {
        if (gameVariables == null || playerLists == null)
            return false;

        if (!gameVariables.ReceivedAllVariables)
            return false;

        // Lists initialized?
        if (playerLists.playersInGameDataList == null) return false;
        if (playerLists.playerStatusInGameDataList == null) return false;
        if (playerLists.playerNamesInGameDataList == null) return false;

        // Space list initialized?
        if (gameVariables.playerSpaceDataList == null) return false;

        return true;
    }
}
