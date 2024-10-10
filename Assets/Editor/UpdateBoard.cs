using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UpdateBoard : MonoBehaviour
{
    static int length = 7;
    static int width = 7;
    static float squareGap = .1f;
    static float squareScale = 1.1f;
    [MenuItem("GameObject/3D Object/CreateBoard")]
    static void CreateBoard()
    {
        GameObject board = Selection.activeGameObject;
        GameObject boardSettingsObject = board.transform.Find("CreateCustomBoard").Find("Scripts").Find("BoardSettings").gameObject;
        BoardSettings boardSettings = boardSettingsObject.GetComponent<BoardSettings>();
        GameObject spacePrefab = board.transform.Find("CreateCustomBoard").Find("Prefabs").Find("Space").gameObject;
        GameObject boardObject = board.transform.Find("Board").gameObject;
        GameObject spaceHeader = board.transform.Find("Board").Find("Spaces").gameObject;
        GameObject spaceSettingObject = board.transform.Find("CreateCustomBoard").Find("Prefabs").Find("SpaceSetting").gameObject;
        GameObject spaceSettingsHeader = board.transform.Find("CreateCustomBoard").Find("Scripts").Find("SpaceSettings").gameObject;
        
        
        for (int i = 0;i < (int)boardSettings.boardSize; i++)
        {
            bool startsOnLeft = (i % 2) == 0;
            for (int k = 0; k < (int)boardSettings.boardSize; k++)
            {
                int count = k + (i * (int)boardSettings.boardSize);
                Selection.activeGameObject = spacePrefab;
                Unsupported.CopyGameObjectsToPasteboard();
                Unsupported.PasteGameObjectsFromPasteboard();
                GameObject tempSpace = Selection.activeGameObject;
                Selection.activeGameObject = spaceSettingObject;
                Unsupported.CopyGameObjectsToPasteboard();
                Unsupported.PasteGameObjectsFromPasteboard();
                GameObject tempSpaceSetting = Selection.activeGameObject;
                tempSpace.transform.Find("Canvas").Find("Text").gameObject.GetComponent<Text>().text = count.ToString();
                tempSpace.transform.SetParent(spaceHeader.transform);
                tempSpaceSetting.transform.SetParent(spaceSettingsHeader.transform);
                if (startsOnLeft)
                {
                    tempSpace.transform.position = new Vector3((k) * 2.55f, 0, i * 2.55f);
                }
                else
                {
                    tempSpace.transform.position = new Vector3((2.55f * ((int)boardSettings.boardSize - 1)) - ((k) * 2.55f), 0, i * 2.55f);
                }
                tempSpace.name = "Space - " + count.ToString();
                tempSpaceSetting.name = "SpaceSetting - " + count.ToString();
                tempSpaceSetting.SetActive(true);
                tempSpace.SetActive(true);
            }
        }
    }
    [MenuItem("GameObject/3D Object/UpdateBoardWithSettings")]
    static void UpdateBoardWithSettings()
    {
        GameObject board = Selection.activeGameObject;
        GameObject spaceSettingsHeader = board.transform.Find("CreateCustomBoard").Find("Scripts").Find("SpaceSettings").gameObject;
        GameObject spaceHeader = board.transform.Find("Board").Find("Spaces").gameObject;
        TextSettings textSettings = board.transform.Find("CreateCustomBoard").Find("Scripts").Find("TextSettings").gameObject.GetComponent<TextSettings>();
        ImageSettings imageSettings = board.transform.Find("CreateCustomBoard").Find("Scripts").Find("ImageSettings").gameObject.GetComponent<ImageSettings>();
        GameObject boardSettingsObject = board.transform.Find("CreateCustomBoard").Find("Scripts").Find("BoardSettings").gameObject;
        BoardSettings boardSettings = boardSettingsObject.GetComponent<BoardSettings>();

        for(int i = 0;i < spaceHeader.transform.childCount; i++)
        {
            SpaceSettings spaceSettings = spaceSettingsHeader.transform.GetChild(i).gameObject.GetComponent<SpaceSettings>();
            spaceHeader.transform.GetChild(i).Find("Canvas").Find("Text (1)").gameObject.GetComponent<Text>().text = ReturnTextBasedOffSetting(spaceSettings, textSettings, i);
            if(i == 0)
            {
                spaceSettings.Start = true;
            }
            spaceHeader.transform.GetChild(i).Find("SpaceImage").gameObject.GetComponent<Renderer>().material = ReturnMaterialBasedOffSetting(spaceSettings, imageSettings);
        }
    }
    static string ReturnTextBasedOffSetting(SpaceSettings spaceSettings, TextSettings textSettings, int spaceNumber)
    {
        if( spaceNumber == 0)
        {
            return "Start";
        }
        string rollAgainText = "";
        string normalText = "";
        if(spaceSettings.DrinkXTimes > 0)
        {
            normalText = textSettings.DrinkXTimesText.Replace("{x}", spaceSettings.DrinkXTimes.ToString());
        }
        if (spaceSettings.SendBackToStart)
        {
            string addAnd = normalText != "" ? " and " : "";
            normalText = normalText + addAnd + textSettings.SendBackToStartText;
        }   
        if (spaceSettings.EveryoneDrinkXTimes > 0)
        {
            string addAnd = normalText != "" ? " and " : "";
            normalText = normalText + addAnd + textSettings.EveryoneDrinkXTimesText.Replace("{x}", spaceSettings.EveryoneDrinkXTimes.ToString());
        }
        if (spaceSettings.MoveBackXSpaces > 0)
        {
            string addAnd = normalText != "" ? " and " : "";
            normalText = normalText + addAnd + textSettings.MoveBackXSpacesText.Replace("{x}", spaceSettings.MoveBackXSpaces.ToString());
        }
        if (spaceSettings.MoveForwardXSpaces > 0)
        {
            string addAnd = normalText != "" ? " and " : "";
            normalText = normalText + addAnd + textSettings.MoveForwardXSpacesText.Replace("{x}", (spaceNumber + spaceSettings.MoveForwardXSpaces).ToString());
        }
        if (spaceSettings.DrinkWhatYouRoll)
        {
            string addAnd = normalText != "" ? " and " : "";
            normalText = normalText + addAnd + textSettings.DrinkWhatYouRollText;
        }
        if (spaceSettings.SwapWithLast)
        {
            string addAnd = normalText != "" ? " and " : "";
            normalText = normalText + addAnd + textSettings.SwapWithLastText;
        }
        if (spaceSettings.SwapWithFirst)
        {
            string addAnd = normalText != "" ? " and " : "";
            normalText = normalText + addAnd + textSettings.SwapWithFirstText;
        }
        if (spaceSettings.ImmuneFromDrinking)
        {
            string addAnd = normalText != "" ? " and " : "";
            normalText = normalText + addAnd + textSettings.ImmuneFromDrinkingText;
        }
        if (spaceSettings.MissTurn)
        {
            string addAnd = normalText != "" ? " and " : "";
            normalText = normalText + addAnd + textSettings.MissTurnText;
        }
        if (spaceSettings.DrinkWithHost)
        {
            string addAnd = normalText != "" ? " and " : "";
            normalText = normalText + addAnd + textSettings.DrinkWithHostText;
        }
        if (spaceSettings.ChooseSomeoneToDrink)
        {
            string addAnd = normalText != "" ? " and " : "";
            normalText = normalText + addAnd + textSettings.ChooseSomeoneToDrinkText;
        }
        if (spaceSettings.GirlsDrink)
        {
            string addAnd = normalText != "" ? " and " : "";
            normalText = normalText + addAnd + textSettings.GirlsDrinkText;
        }
        if (spaceSettings.GuysDrink)
        {
            string addAnd = normalText != "" ? " and " : "";
            normalText = normalText + addAnd + textSettings.GuysDrinkText;
        }
        if (spaceSettings.Finish)
        {
            string addAnd = normalText != "" ? " and " : "";
            normalText = normalText + addAnd + textSettings.FinishText;
        }
        if (spaceSettings.RollAgain)
        {
            string addAnd = normalText != "" ? " and " : "";
            normalText = normalText + addAnd + textSettings.RollAgainText;
        }
        return normalText;
    }
    static Material ReturnMaterialBasedOffSetting(SpaceSettings spaceSettings, ImageSettings imageSettings)
    {
        Material returnMat;
        if (spaceSettings.Start)
        {
            return imageSettings.StartMat;
        }
        if (spaceSettings.Finish)
        {
            returnMat = imageSettings.FinishMat;
        }
        else if (spaceSettings.SendBackToStart)
        {
            returnMat = imageSettings.SendBackToStartMat;
        }
        else if (spaceSettings.RollAgain)
        {
            returnMat = imageSettings.RollAgainMat;
        }
        else if (spaceSettings.EveryoneDrinkXTimes > 0)
        {
            returnMat = imageSettings.EveryoneDrinkXTimesMat;
        }
        else if (spaceSettings.DrinkXTimes > 0)
        {
            returnMat = imageSettings.DrinkXTimesMat;
        }
        else if (spaceSettings.MoveBackXSpaces > 0)
        {
            returnMat = imageSettings.MoveBackXSpacesMat;
        }
        else if (spaceSettings.MoveForwardXSpaces > 0)
        {
            returnMat = imageSettings.MoveForwardXSpacesMat;
        }
        else if (spaceSettings.DrinkWhatYouRoll)
        {
            returnMat = imageSettings.DrinkWhatYouRollMat;
        }
        else if (spaceSettings.SwapWithLast)
        {
            returnMat = imageSettings.SwapWithLastMat;
        }
        else if (spaceSettings.SwapWithFirst)
        {
            returnMat = imageSettings.SwapWithFirstMat;
        }
        else if (spaceSettings.ImmuneFromDrinking)
        {
            returnMat = imageSettings.ImmuneFromDrinkingMat;
        }
        else if (spaceSettings.MissTurn)
        {
            returnMat = imageSettings.MissTurnMat;
        }
        else if (spaceSettings.DrinkWithHost)
        {
            returnMat = imageSettings.DrinkWithHostMat;
        }
        else if (spaceSettings.ChooseSomeoneToDrink)
        {
            returnMat = imageSettings.ChooseSomeoneToDrinkMat;
        }
        else if (spaceSettings.GirlsDrink)
        {
            returnMat = imageSettings.GirlsDrinkMat;
        }
        else if (spaceSettings.GuysDrink)
        {
            returnMat = imageSettings.GuysDrinkMat;
        }
        else
        {
            returnMat = imageSettings.DrinkXTimesMat;
        }
        Debug.Log(returnMat.name);
        return returnMat;
    }

    [MenuItem("GameObject/3D Object/UpdateBoard")]
    static void UpdateBoardLayout()
    {
        GameObject board = Selection.activeGameObject;
        if(board.name != "Board")
        {
            return;
        }
        int spaceNum = 0;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                Debug.Log("Board Settings");
                board.transform.GetChild(i).transform.GetChild(j).gameObject.SetActive(false);
                board.transform.GetChild(i).transform.GetChild(j).localPosition = new Vector3(0, 0, 0);
            }
        }
        for (int i = 0; i < length; i++)
        {
            if (i > 0)
            {
                board.transform.GetChild(i).transform.localPosition = board.transform.GetChild(i).transform.localPosition + new Vector3(0, 0, (2.5f * i) + ((squareGap / squareScale) * i));
            }
            if (i % 2 == 0)
            {
                for (int j = 0; j < width; j++)
                {
                    board.transform.GetChild(i).transform.GetChild(j).gameObject.SetActive(true);
                    (board.transform.GetChild(i).transform.GetChild(j).GetChild(6).gameObject.GetComponentInChildren<Text>()).text = spaceNum.ToString();
                    if (j > 0)
                    {
                        board.transform.GetChild(i).transform.GetChild(j).localPosition = board.transform.GetChild(i).transform.GetChild(j).localPosition + new Vector3((2.5f * j) + ((squareGap / squareScale) * j), 0);
                    }
                    Debug.Log(board.transform.GetChild(i).transform.GetChild(j).name);
                    spaceNum++;
                }
            }
            else
            {
                for (int j = width - 1; j >= 0; j--)
                {
                    board.transform.GetChild(i).transform.GetChild(j).gameObject.SetActive(true);
                    (board.transform.GetChild(i).transform.GetChild(j).GetChild(6).gameObject.GetComponentInChildren<Text>()).text = spaceNum.ToString();
                    if (j > 0)
                    {
                        board.transform.GetChild(i).transform.GetChild(j).localPosition = board.transform.GetChild(i).transform.GetChild(j).localPosition + new Vector3((2.5f * j) + ((squareGap / squareScale) * j), 0);
                    }
                    Debug.Log(board.transform.GetChild(i).transform.GetChild(j).name);
                    spaceNum++;
                }
            }
        }
        //VRCPlayerApi.GetPlayers();
        board.transform.localScale = board.transform.localScale * squareScale;

    }
    [MenuItem("GameObject/3D Object/ResetBoard")]
    static void ResetBoard()
    {
        //GameObject board = Selection.activeGameObject;
        //if (board.name != "Board")
        //{
        //    return;
        //}
        //BoardSettings boardSettings = board.GetComponent<BoardSettings>();
        //int spaceNum = 0;
        //for (int i = 0; i < 10; i++)
        //{
        //    for (int j = 0; j < 10; j++)
        //    {
        //        Debug.Log("Board Settings");
        //        if (boardSettings == null)
        //        {
        //            Debug.Log("null");
        //        }
        //        board.transform.GetChild(i).transform.GetChild(j).gameObject.SetActive(false);
        //        board.transform.GetChild(i).transform.GetChild(j).localPosition = new Vector3(0, 0, 0);
        //    }
        //    board.transform.GetChild(i).localPosition = new Vector3(0, 0, 0);
        //}
        //board.transform.localScale = new Vector3(1, 1, 1);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
