using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PlayerFunctions_BoardGame : UdonSharpBehaviour
{
    [SerializeField]
    GameVariables_BoardGame gameVariables;
    [SerializeField]
    PlayerList_BoardGame playerLists;
    [SerializeField] UpdatePlayerCamerasOnSpace_BoardGame updatePlayerCamerasOnSpace;

    public void AddPlayer(int playerIdToAdd)
    {
        if (!playerLists.playersInGameDataList.Contains(playerIdToAdd))
        {
            playerLists.playersInGameDataList.Add(playerIdToAdd);
            if (gameVariables.GameStarted)
            {
                playerLists.playerStatusInGameDataList.Add((int)PlayerInGameStatus.Connected);
                playerLists.playerNamesInGameDataList.Add(VRCPlayerApi.GetPlayerById(playerIdToAdd).displayName);
                gameVariables.missedTurnDataList.Add(false);
                gameVariables.playerSpaceDataList.Add(0);
                updatePlayerCamerasOnSpace.UpdateCameraCountOnSpaces();
            }
            gameVariables.RequestSerialization();
            playerLists.RequestSerialization();
        }
        else
        {
            if (gameVariables.GameStarted)
            {
                for (int i = 0; i < playerLists.playersInGameDataList.Count; i++)
                {
                    if (playerIdToAdd == playerLists.playersInGameDataList[i])
                    {
                        playerLists.playerStatusInGameDataList[i] = (int)PlayerInGameStatus.Connected;
                        playerLists.RequestSerialization();
                    }
                }
            }
        }
    }
    public void RemovePlayer(int playerIdToRemove)
    {
        Debug.Log("Remove Player: " + playerIdToRemove.ToString());
        if (playerLists.playersInGameDataList.Contains(playerIdToRemove))
        {
            if (gameVariables.GameStarted)
            {
                Debug.Log("Game Is Started");
                //check to see if player exists in the game already and update their place in the game to be a left status
                for (int i = 0; i < playerLists.playersInGameDataList.Count; i++)
                {
                    Debug.Log("Player ID in List: " + playerLists.playersInGameDataList[i].ToString());
                    if (playerIdToRemove == playerLists.playersInGameDataList[i])
                    {
                        playerLists.playerStatusInGameDataList[i] = (int)PlayerInGameStatus.LeftGame;
                    }
                }
            }
            else
            {
                playerLists.playersInGameDataList.Remove(playerIdToRemove);
            }
            playerLists.RequestSerialization();
        }
    }
    public void SendPlayerToMasterAdd(int i)
    {
        Debug.Log("Sending event - " + "SendPlayer" + i.ToString() + "ToMasterAdd");
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "SendPlayer" + i.ToString() + "ToMasterAdd");
    }
    public void SendPlayerToMasterRemove(int i)
    {
        Debug.Log("Sending event - " + "SendPlayer" + i.ToString() + "ToMasterRemove");
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "SendPlayer" + i.ToString() + "ToMasterRemove");
    }
    public void SendPlayer1ToMasterAdd()
    {
        AddPlayer(1);
    }
    public void SendPlayer2ToMasterAdd()
    {
        AddPlayer(2);
    }
    public void SendPlayer3ToMasterAdd()
    {
        AddPlayer(3);
    }
    public void SendPlayer4ToMasterAdd()
    {
        AddPlayer(4);
    }
    public void SendPlayer5ToMasterAdd()
    {
        AddPlayer(5);
    }
    public void SendPlayer6ToMasterAdd()
    {
        AddPlayer(6);
    }
    public void SendPlayer7ToMasterAdd()
    {
        AddPlayer(7);
    }
    public void SendPlayer8ToMasterAdd()
    {
        AddPlayer(8);
    }
    public void SendPlayer9ToMasterAdd()
    {
        AddPlayer(9);
    }
    public void SendPlayer10ToMasterAdd()
    {
        AddPlayer(10);
    }
    public void SendPlayer11ToMasterAdd()
    {
        AddPlayer(11);
    }
    public void SendPlayer12ToMasterAdd()
    {
        AddPlayer(12);
    }
    public void SendPlayer13ToMasterAdd()
    {
        AddPlayer(13);
    }
    public void SendPlayer14ToMasterAdd()
    {
        AddPlayer(14);
    }
    public void SendPlayer15ToMasterAdd()
    {
        AddPlayer(15);
    }
    public void SendPlayer16ToMasterAdd()
    {
        AddPlayer(16);
    }
    public void SendPlayer17ToMasterAdd()
    {
        AddPlayer(17);
    }
    public void SendPlayer18ToMasterAdd()
    {
        AddPlayer(18);
    }
    public void SendPlayer19ToMasterAdd()
    {
        AddPlayer(19);
    }
    public void SendPlayer20ToMasterAdd()
    {
        AddPlayer(20);
    }
    public void SendPlayer21ToMasterAdd()
    {
        AddPlayer(21);
    }
    public void SendPlayer22ToMasterAdd()
    {
        AddPlayer(22);
    }
    public void SendPlayer23ToMasterAdd()
    {
        AddPlayer(23);
    }
    public void SendPlayer24ToMasterAdd()
    {
        AddPlayer(24);
    }
    public void SendPlayer25ToMasterAdd()
    {
        AddPlayer(25);
    }
    public void SendPlayer26ToMasterAdd()
    {
        AddPlayer(26);
    }
    public void SendPlayer27ToMasterAdd()
    {
        AddPlayer(27);
    }
    public void SendPlayer28ToMasterAdd()
    {
        AddPlayer(28);
    }
    public void SendPlayer29ToMasterAdd()
    {
        AddPlayer(29);
    }
    public void SendPlayer30ToMasterAdd()
    {
        AddPlayer(30);
    }
    public void SendPlayer31ToMasterAdd()
    {
        AddPlayer(31);
    }
    public void SendPlayer32ToMasterAdd()
    {
        AddPlayer(32);
    }
    public void SendPlayer33ToMasterAdd()
    {
        AddPlayer(33);
    }
    public void SendPlayer34ToMasterAdd()
    {
        AddPlayer(34);
    }
    public void SendPlayer35ToMasterAdd()
    {
        AddPlayer(35);
    }
    public void SendPlayer36ToMasterAdd()
    {
        AddPlayer(36);
    }
    public void SendPlayer37ToMasterAdd()
    {
        AddPlayer(37);
    }
    public void SendPlayer38ToMasterAdd()
    {
        AddPlayer(38);
    }
    public void SendPlayer39ToMasterAdd()
    {
        AddPlayer(39);
    }
    public void SendPlayer40ToMasterAdd()
    {
        AddPlayer(40);
    }
    public void SendPlayer41ToMasterAdd()
    {
        AddPlayer(41);
    }
    public void SendPlayer42ToMasterAdd()
    {
        AddPlayer(42);
    }
    public void SendPlayer43ToMasterAdd()
    {
        AddPlayer(43);
    }
    public void SendPlayer44ToMasterAdd()
    {
        AddPlayer(44);
    }
    public void SendPlayer45ToMasterAdd()
    {
        AddPlayer(45);
    }
    public void SendPlayer46ToMasterAdd()
    {
        AddPlayer(46);
    }
    public void SendPlayer47ToMasterAdd()
    {
        AddPlayer(47);
    }
    public void SendPlayer48ToMasterAdd()
    {
        AddPlayer(48);
    }
    public void SendPlayer49ToMasterAdd()
    {
        AddPlayer(49);
    }
    public void SendPlayer50ToMasterAdd()
    {
        AddPlayer(50);
    }
    public void SendPlayer51ToMasterAdd()
    {
        AddPlayer(51);
    }
    public void SendPlayer52ToMasterAdd()
    {
        AddPlayer(52);
    }
    public void SendPlayer53ToMasterAdd()
    {
        AddPlayer(53);
    }
    public void SendPlayer54ToMasterAdd()
    {
        AddPlayer(54);
    }
    public void SendPlayer55ToMasterAdd()
    {
        AddPlayer(55);
    }
    public void SendPlayer56ToMasterAdd()
    {
        AddPlayer(56);
    }
    public void SendPlayer57ToMasterAdd()
    {
        AddPlayer(57);
    }
    public void SendPlayer58ToMasterAdd()
    {
        AddPlayer(58);
    }
    public void SendPlayer59ToMasterAdd()
    {
        AddPlayer(59);
    }
    public void SendPlayer60ToMasterAdd()
    {
        AddPlayer(60);
    }
    public void SendPlayer61ToMasterAdd()
    {
        AddPlayer(61);
    }
    public void SendPlayer62ToMasterAdd()
    {
        AddPlayer(62);
    }
    public void SendPlayer63ToMasterAdd()
    {
        AddPlayer(63);
    }
    public void SendPlayer64ToMasterAdd()
    {
        AddPlayer(64);
    }
    public void SendPlayer65ToMasterAdd()
    {
        AddPlayer(65);
    }
    public void SendPlayer66ToMasterAdd()
    {
        AddPlayer(66);
    }
    public void SendPlayer67ToMasterAdd()
    {
        AddPlayer(67);
    }
    public void SendPlayer68ToMasterAdd()
    {
        AddPlayer(68);
    }
    public void SendPlayer69ToMasterAdd()
    {
        AddPlayer(69);
    }
    public void SendPlayer70ToMasterAdd()
    {
        AddPlayer(70);
    }
    public void SendPlayer71ToMasterAdd()
    {
        AddPlayer(71);
    }
    public void SendPlayer72ToMasterAdd()
    {
        AddPlayer(72);
    }
    public void SendPlayer73ToMasterAdd()
    {
        AddPlayer(73);
    }
    public void SendPlayer74ToMasterAdd()
    {
        AddPlayer(74);
    }
    public void SendPlayer75ToMasterAdd()
    {
        AddPlayer(75);
    }
    public void SendPlayer76ToMasterAdd()
    {
        AddPlayer(76);
    }
    public void SendPlayer77ToMasterAdd()
    {
        AddPlayer(77);
    }
    public void SendPlayer78ToMasterAdd()
    {
        AddPlayer(78);
    }
    public void SendPlayer79ToMasterAdd()
    {
        AddPlayer(79);
    }
    public void SendPlayer80ToMasterAdd()
    {
        AddPlayer(80);
    }
    public void SendPlayer81ToMasterAdd()
    {
        AddPlayer(81);
    }
    public void SendPlayer82ToMasterAdd()
    {
        AddPlayer(82);
    }
    public void SendPlayer83ToMasterAdd()
    {
        AddPlayer(83);
    }
    public void SendPlayer84ToMasterAdd()
    {
        AddPlayer(84);
    }
    public void SendPlayer85ToMasterAdd()
    {
        AddPlayer(85);
    }
    public void SendPlayer86ToMasterAdd()
    {
        AddPlayer(86);
    }
    public void SendPlayer87ToMasterAdd()
    {
        AddPlayer(87);
    }
    public void SendPlayer88ToMasterAdd()
    {
        AddPlayer(88);
    }
    public void SendPlayer89ToMasterAdd()
    {
        AddPlayer(89);
    }
    public void SendPlayer90ToMasterAdd()
    {
        AddPlayer(90);
    }
    public void SendPlayer91ToMasterAdd()
    {
        AddPlayer(91);
    }
    public void SendPlayer92ToMasterAdd()
    {
        AddPlayer(92);
    }
    public void SendPlayer93ToMasterAdd()
    {
        AddPlayer(93);
    }
    public void SendPlayer94ToMasterAdd()
    {
        AddPlayer(94);
    }
    public void SendPlayer95ToMasterAdd()
    {
        AddPlayer(95);
    }
    public void SendPlayer96ToMasterAdd()
    {
        AddPlayer(96);
    }
    public void SendPlayer97ToMasterAdd()
    {
        AddPlayer(97);
    }
    public void SendPlayer98ToMasterAdd()
    {
        AddPlayer(98);
    }
    public void SendPlayer99ToMasterAdd()
    {
        AddPlayer(99);
    }
    public void SendPlayer100ToMasterAdd()
    {
        AddPlayer(100);
    }
    public void SendPlayer101ToMasterAdd()
    {
        AddPlayer(101);
    }
    public void SendPlayer102ToMasterAdd()
    {
        AddPlayer(102);
    }
    public void SendPlayer103ToMasterAdd()
    {
        AddPlayer(103);
    }
    public void SendPlayer104ToMasterAdd()
    {
        AddPlayer(104);
    }
    public void SendPlayer105ToMasterAdd()
    {
        AddPlayer(105);
    }
    public void SendPlayer106ToMasterAdd()
    {
        AddPlayer(106);
    }
    public void SendPlayer107ToMasterAdd()
    {
        AddPlayer(107);
    }
    public void SendPlayer108ToMasterAdd()
    {
        AddPlayer(108);
    }
    public void SendPlayer109ToMasterAdd()
    {
        AddPlayer(109);
    }
    public void SendPlayer110ToMasterAdd()
    {
        AddPlayer(110);
    }
    public void SendPlayer111ToMasterAdd()
    {
        AddPlayer(111);
    }
    public void SendPlayer112ToMasterAdd()
    {
        AddPlayer(112);
    }
    public void SendPlayer113ToMasterAdd()
    {
        AddPlayer(113);
    }
    public void SendPlayer114ToMasterAdd()
    {
        AddPlayer(114);
    }
    public void SendPlayer115ToMasterAdd()
    {
        AddPlayer(115);
    }
    public void SendPlayer116ToMasterAdd()
    {
        AddPlayer(116);
    }
    public void SendPlayer117ToMasterAdd()
    {
        AddPlayer(117);
    }
    public void SendPlayer118ToMasterAdd()
    {
        AddPlayer(118);
    }
    public void SendPlayer119ToMasterAdd()
    {
        AddPlayer(119);
    }
    public void SendPlayer120ToMasterAdd()
    {
        AddPlayer(120);
    }
    public void SendPlayer121ToMasterAdd()
    {
        AddPlayer(121);
    }
    public void SendPlayer122ToMasterAdd()
    {
        AddPlayer(122);
    }
    public void SendPlayer123ToMasterAdd()
    {
        AddPlayer(123);
    }
    public void SendPlayer124ToMasterAdd()
    {
        AddPlayer(124);
    }
    public void SendPlayer125ToMasterAdd()
    {
        AddPlayer(125);
    }
    public void SendPlayer126ToMasterAdd()
    {
        AddPlayer(126);
    }
    public void SendPlayer127ToMasterAdd()
    {
        AddPlayer(127);
    }
    public void SendPlayer128ToMasterAdd()
    {
        AddPlayer(128);
    }

    public void SendPlayer1ToMasterRemove()
    {
        RemovePlayer(1);
    }
    public void SendPlayer2ToMasterRemove()
    {
        RemovePlayer(2);
    }
    public void SendPlayer3ToMasterRemove()
    {
        RemovePlayer(3);
    }
    public void SendPlayer4ToMasterRemove()
    {
        RemovePlayer(4);
    }
    public void SendPlayer5ToMasterRemove()
    {
        RemovePlayer(5);
    }
    public void SendPlayer6ToMasterRemove()
    {
        RemovePlayer(6);
    }
    public void SendPlayer7ToMasterRemove()
    {
        RemovePlayer(7);
    }
    public void SendPlayer8ToMasterRemove()
    {
        RemovePlayer(8);
    }
    public void SendPlayer9ToMasterRemove()
    {
        RemovePlayer(9);
    }
    public void SendPlayer10ToMasterRemove()
    {
        RemovePlayer(10);
    }
    public void SendPlayer11ToMasterRemove()
    {
        RemovePlayer(11);
    }
    public void SendPlayer12ToMasterRemove()
    {
        RemovePlayer(12);
    }
    public void SendPlayer13ToMasterRemove()
    {
        RemovePlayer(13);
    }
    public void SendPlayer14ToMasterRemove()
    {
        RemovePlayer(14);
    }
    public void SendPlayer15ToMasterRemove()
    {
        RemovePlayer(15);
    }
    public void SendPlayer16ToMasterRemove()
    {
        RemovePlayer(16);
    }
    public void SendPlayer17ToMasterRemove()
    {
        RemovePlayer(17);
    }
    public void SendPlayer18ToMasterRemove()
    {
        RemovePlayer(18);
    }
    public void SendPlayer19ToMasterRemove()
    {
        RemovePlayer(19);
    }
    public void SendPlayer20ToMasterRemove()
    {
        RemovePlayer(20);
    }
    public void SendPlayer21ToMasterRemove()
    {
        RemovePlayer(21);
    }
    public void SendPlayer22ToMasterRemove()
    {
        RemovePlayer(22);
    }
    public void SendPlayer23ToMasterRemove()
    {
        RemovePlayer(23);
    }
    public void SendPlayer24ToMasterRemove()
    {
        RemovePlayer(24);
    }
    public void SendPlayer25ToMasterRemove()
    {
        RemovePlayer(25);
    }
    public void SendPlayer26ToMasterRemove()
    {
        RemovePlayer(26);
    }
    public void SendPlayer27ToMasterRemove()
    {
        RemovePlayer(27);
    }
    public void SendPlayer28ToMasterRemove()
    {
        RemovePlayer(28);
    }
    public void SendPlayer29ToMasterRemove()
    {
        RemovePlayer(29);
    }
    public void SendPlayer30ToMasterRemove()
    {
        RemovePlayer(30);
    }
    public void SendPlayer31ToMasterRemove()
    {
        RemovePlayer(31);
    }
    public void SendPlayer32ToMasterRemove()
    {
        RemovePlayer(32);
    }
    public void SendPlayer33ToMasterRemove()
    {
        RemovePlayer(33);
    }
    public void SendPlayer34ToMasterRemove()
    {
        RemovePlayer(34);
    }
    public void SendPlayer35ToMasterRemove()
    {
        RemovePlayer(35);
    }
    public void SendPlayer36ToMasterRemove()
    {
        RemovePlayer(36);
    }
    public void SendPlayer37ToMasterRemove()
    {
        RemovePlayer(37);
    }
    public void SendPlayer38ToMasterRemove()
    {
        RemovePlayer(38);
    }
    public void SendPlayer39ToMasterRemove()
    {
        RemovePlayer(39);
    }
    public void SendPlayer40ToMasterRemove()
    {
        RemovePlayer(40);
    }
    public void SendPlayer41ToMasterRemove()
    {
        RemovePlayer(41);
    }
    public void SendPlayer42ToMasterRemove()
    {
        RemovePlayer(42);
    }
    public void SendPlayer43ToMasterRemove()
    {
        RemovePlayer(43);
    }
    public void SendPlayer44ToMasterRemove()
    {
        RemovePlayer(44);
    }
    public void SendPlayer45ToMasterRemove()
    {
        RemovePlayer(45);
    }
    public void SendPlayer46ToMasterRemove()
    {
        RemovePlayer(46);
    }
    public void SendPlayer47ToMasterRemove()
    {
        RemovePlayer(47);
    }
    public void SendPlayer48ToMasterRemove()
    {
        RemovePlayer(48);
    }
    public void SendPlayer49ToMasterRemove()
    {
        RemovePlayer(49);
    }
    public void SendPlayer50ToMasterRemove()
    {
        RemovePlayer(50);
    }
    public void SendPlayer51ToMasterRemove()
    {
        RemovePlayer(51);
    }
    public void SendPlayer52ToMasterRemove()
    {
        RemovePlayer(52);
    }
    public void SendPlayer53ToMasterRemove()
    {
        RemovePlayer(53);
    }
    public void SendPlayer54ToMasterRemove()
    {
        RemovePlayer(54);
    }
    public void SendPlayer55ToMasterRemove()
    {
        RemovePlayer(55);
    }
    public void SendPlayer56ToMasterRemove()
    {
        RemovePlayer(56);
    }
    public void SendPlayer57ToMasterRemove()
    {
        RemovePlayer(57);
    }
    public void SendPlayer58ToMasterRemove()
    {
        RemovePlayer(58);
    }
    public void SendPlayer59ToMasterRemove()
    {
        RemovePlayer(59);
    }
    public void SendPlayer60ToMasterRemove()
    {
        RemovePlayer(60);
    }
    public void SendPlayer61ToMasterRemove()
    {
        RemovePlayer(61);
    }
    public void SendPlayer62ToMasterRemove()
    {
        RemovePlayer(62);
    }
    public void SendPlayer63ToMasterRemove()
    {
        RemovePlayer(63);
    }
    public void SendPlayer64ToMasterRemove()
    {
        RemovePlayer(64);
    }
    public void SendPlayer65ToMasterRemove()
    {
        RemovePlayer(65);
    }
    public void SendPlayer66ToMasterRemove()
    {
        RemovePlayer(66);
    }
    public void SendPlayer67ToMasterRemove()
    {
        RemovePlayer(67);
    }
    public void SendPlayer68ToMasterRemove()
    {
        RemovePlayer(68);
    }
    public void SendPlayer69ToMasterRemove()
    {
        RemovePlayer(69);
    }
    public void SendPlayer70ToMasterRemove()
    {
        RemovePlayer(70);
    }
    public void SendPlayer71ToMasterRemove()
    {
        RemovePlayer(71);
    }
    public void SendPlayer72ToMasterRemove()
    {
        RemovePlayer(72);
    }
    public void SendPlayer73ToMasterRemove()
    {
        RemovePlayer(73);
    }
    public void SendPlayer74ToMasterRemove()
    {
        RemovePlayer(74);
    }
    public void SendPlayer75ToMasterRemove()
    {
        RemovePlayer(75);
    }
    public void SendPlayer76ToMasterRemove()
    {
        RemovePlayer(76);
    }
    public void SendPlayer77ToMasterRemove()
    {
        RemovePlayer(77);
    }
    public void SendPlayer78ToMasterRemove()
    {
        RemovePlayer(78);
    }
    public void SendPlayer79ToMasterRemove()
    {
        RemovePlayer(79);
    }
    public void SendPlayer80ToMasterRemove()
    {
        RemovePlayer(80);
    }
    public void SendPlayer81ToMasterRemove()
    {
        RemovePlayer(81);
    }
    public void SendPlayer82ToMasterRemove()
    {
        RemovePlayer(82);
    }
    public void SendPlayer83ToMasterRemove()
    {
        RemovePlayer(83);
    }
    public void SendPlayer84ToMasterRemove()
    {
        RemovePlayer(84);
    }
    public void SendPlayer85ToMasterRemove()
    {
        RemovePlayer(85);
    }
    public void SendPlayer86ToMasterRemove()
    {
        RemovePlayer(86);
    }
    public void SendPlayer87ToMasterRemove()
    {
        RemovePlayer(87);
    }
    public void SendPlayer88ToMasterRemove()
    {
        RemovePlayer(88);
    }
    public void SendPlayer89ToMasterRemove()
    {
        RemovePlayer(89);
    }
    public void SendPlayer90ToMasterRemove()
    {
        RemovePlayer(90);
    }
    public void SendPlayer91ToMasterRemove()
    {
        RemovePlayer(91);
    }
    public void SendPlayer92ToMasterRemove()
    {
        RemovePlayer(92);
    }
    public void SendPlayer93ToMasterRemove()
    {
        RemovePlayer(93);
    }
    public void SendPlayer94ToMasterRemove()
    {
        RemovePlayer(94);
    }
    public void SendPlayer95ToMasterRemove()
    {
        RemovePlayer(95);
    }
    public void SendPlayer96ToMasterRemove()
    {
        RemovePlayer(96);
    }
    public void SendPlayer97ToMasterRemove()
    {
        RemovePlayer(97);
    }
    public void SendPlayer98ToMasterRemove()
    {
        RemovePlayer(98);
    }
    public void SendPlayer99ToMasterRemove()
    {
        RemovePlayer(99);
    }
    public void SendPlayer100ToMasterRemove()
    {
        RemovePlayer(100);
    }
    public void SendPlayer101ToMasterRemove()
    {
        RemovePlayer(101);
    }
    public void SendPlayer102ToMasterRemove()
    {
        RemovePlayer(102);
    }
    public void SendPlayer103ToMasterRemove()
    {
        RemovePlayer(103);
    }
    public void SendPlayer104ToMasterRemove()
    {
        RemovePlayer(104);
    }
    public void SendPlayer105ToMasterRemove()
    {
        RemovePlayer(105);
    }
    public void SendPlayer106ToMasterRemove()
    {
        RemovePlayer(106);
    }
    public void SendPlayer107ToMasterRemove()
    {
        RemovePlayer(107);
    }
    public void SendPlayer108ToMasterRemove()
    {
        RemovePlayer(108);
    }
    public void SendPlayer109ToMasterRemove()
    {
        RemovePlayer(109);
    }
    public void SendPlayer110ToMasterRemove()
    {
        RemovePlayer(110);
    }
    public void SendPlayer111ToMasterRemove()
    {
        RemovePlayer(111);
    }
    public void SendPlayer112ToMasterRemove()
    {
        RemovePlayer(112);
    }
    public void SendPlayer113ToMasterRemove()
    {
        RemovePlayer(113);
    }
    public void SendPlayer114ToMasterRemove()
    {
        RemovePlayer(114);
    }
    public void SendPlayer115ToMasterRemove()
    {
        RemovePlayer(115);
    }
    public void SendPlayer116ToMasterRemove()
    {
        RemovePlayer(116);
    }
    public void SendPlayer117ToMasterRemove()
    {
        RemovePlayer(117);
    }
    public void SendPlayer118ToMasterRemove()
    {
        RemovePlayer(118);
    }
    public void SendPlayer119ToMasterRemove()
    {
        RemovePlayer(119);
    }
    public void SendPlayer120ToMasterRemove()
    {
        RemovePlayer(120);
    }
    public void SendPlayer121ToMasterRemove()
    {
        RemovePlayer(121);
    }
    public void SendPlayer122ToMasterRemove()
    {
        RemovePlayer(122);
    }
    public void SendPlayer123ToMasterRemove()
    {
        RemovePlayer(123);
    }
    public void SendPlayer124ToMasterRemove()
    {
        RemovePlayer(124);
    }
    public void SendPlayer125ToMasterRemove()
    {
        RemovePlayer(125);
    }
    public void SendPlayer126ToMasterRemove()
    {
        RemovePlayer(126);
    }
    public void SendPlayer127ToMasterRemove()
    {
        RemovePlayer(127);
    }
    public void SendPlayer128ToMasterRemove()
    {
        RemovePlayer(128);
    }
}
