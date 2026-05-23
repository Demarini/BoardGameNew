using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UpdateBoard : MonoBehaviour
{
    [MenuItem("BoardGame/Generate Board From JSON")]
    static void GenerateBoardFromJson()
    {
        string path = EditorUtility.OpenFilePanel("Select Board JSON", Application.dataPath + "/BoardGame/BoardDefinitions", "json");
        if (string.IsNullOrEmpty(path)) return;

        string json = File.ReadAllText(path);
        BoardDefinition def = JsonUtility.FromJson<BoardDefinition>(json);

        if (def == null || def.spaces == null || def.spaces.Length == 0)
        {
            Debug.LogError("Failed to parse board JSON or no spaces defined.");
            return;
        }

        GameObject board = Selection.activeGameObject;
        if (board == null)
        {
            Debug.LogError("Select the root board GameObject first.");
            return;
        }

        GameObject spacePrefab = board.transform.Find("CreateCustomBoard").Find("Prefabs").Find("Space").gameObject;
        GameObject spaceHeader = board.transform.Find("Board").Find("Spaces").gameObject;
        GameObject spaceSettingObject = board.transform.Find("CreateCustomBoard").Find("Prefabs").Find("SpaceSetting").gameObject;
        GameObject spaceSettingsHeader = board.transform.Find("CreateCustomBoard").Find("Scripts").Find("SpaceSettings").gameObject;
        TextSettings textSettings = board.transform.Find("CreateCustomBoard").Find("Scripts").Find("TextSettings").gameObject.GetComponent<TextSettings>();
        ImageSettings imageSettings = board.transform.Find("CreateCustomBoard").Find("Scripts").Find("ImageSettings").gameObject.GetComponent<ImageSettings>();

        int columns = def.columns > 0 ? def.columns : 7;
        int rows = def.rows > 0 ? def.rows : Mathf.CeilToInt((float)def.spaces.Length / columns);
        int totalSpaces = def.spaces.Length;

        Undo.RegisterCompleteObjectUndo(spaceHeader, "Generate Board From JSON");
        Undo.RegisterCompleteObjectUndo(spaceSettingsHeader, "Generate Board From JSON");

        ClearChildren(spaceHeader);
        ClearChildren(spaceSettingsHeader);

        for (int i = 0; i < totalSpaces; i++)
        {
            int row = i / columns;
            int col = i % columns;
            bool startsOnLeft = (row % 2) == 0;

            Selection.activeGameObject = spacePrefab;
            Unsupported.CopyGameObjectsToPasteboard();
            Unsupported.PasteGameObjectsFromPasteboard();
            GameObject tempSpace = Selection.activeGameObject;

            Selection.activeGameObject = spaceSettingObject;
            Unsupported.CopyGameObjectsToPasteboard();
            Unsupported.PasteGameObjectsFromPasteboard();
            GameObject tempSpaceSetting = Selection.activeGameObject;

            Transform canvasText = tempSpace.transform.Find("Canvas").Find("Text");
            if (canvasText != null)
                canvasText.GetComponent<Text>().text = i.ToString();

            tempSpace.transform.SetParent(spaceHeader.transform);
            tempSpaceSetting.transform.SetParent(spaceSettingsHeader.transform);

            float xPos;
            if (startsOnLeft)
                xPos = col * 2.55f;
            else
                xPos = (2.55f * (columns - 1)) - (col * 2.55f);

            tempSpace.transform.localPosition = new Vector3(xPos, 0, row * 2.55f);

            tempSpace.name = "Space - " + i;
            tempSpaceSetting.name = "SpaceSetting - " + i;
            tempSpaceSetting.SetActive(true);
            tempSpace.SetActive(true);

            SpaceSettings ss = tempSpaceSetting.GetComponent<SpaceSettings>();
            SpaceDefinition sd = def.spaces[i];
            ApplySpaceDefinition(ss, sd, i, totalSpaces);

            Transform descText = tempSpace.transform.Find("Canvas").Find("Text (1)");
            if (descText != null)
            {
                Text descTextComp = descText.GetComponent<Text>();
                descTextComp.text = !string.IsNullOrEmpty(sd.text)
                    ? sd.text
                    : ReturnTextBasedOffSetting(ss, textSettings, i);
                descTextComp.resizeTextForBestFit = true;
                descTextComp.resizeTextMaxSize = 100;
                descTextComp.resizeTextMinSize = 20;
            }

            Transform spaceImage = tempSpace.transform.Find("SpaceImage");
            if (spaceImage != null)
            {
                spaceImage.GetComponent<Renderer>().material = ReturnMaterialBasedOffSetting(ss, imageSettings);

                bool isOddRow = (row % 2) == 1;
                bool isDirectionalBack = ss.MoveBackXSpaces > 0 || ss.LeaderMoveBackXSpaces > 0;
                if (isOddRow && isDirectionalBack)
                {
                    Vector3 s = spaceImage.localScale;
                    spaceImage.localScale = new Vector3(-s.x, s.y, s.z);
                }
            }
        }

        if (def.mysteryPool != null && def.mysteryPool.Length > 0)
        {
            BakeMysteryPool(board, def.mysteryPool);
        }

        Debug.Log($"Board '{def.name}' generated: {totalSpaces} spaces ({columns}x{rows})");
        EditorUtility.SetDirty(spaceHeader);
        EditorUtility.SetDirty(spaceSettingsHeader);
    }

    static void ApplySpaceDefinition(SpaceSettings ss, SpaceDefinition sd, int index, int totalSpaces)
    {
        ss.RollAgain = false;
        ss.DrinkXTimes = 0;
        ss.SendBackToStart = false;
        ss.EveryoneDrinkXTimes = 0;
        ss.MoveBackXSpaces = 0;
        ss.MoveForwardXSpaces = 0;
        ss.DrinkWhatYouRoll = false;
        ss.SwapWithLast = false;
        ss.SwapWithFirst = false;
        ss.ImmuneFromDrinking = false;
        ss.MissTurn = false;
        ss.DrinkWithHost = false;
        ss.ChooseSomeoneToDrink = false;
        ss.GirlsDrink = false;
        ss.GuysDrink = false;
        ss.Finish = false;
        ss.Start = false;
        ss.IsMystery = false;
        ss.LeaderMoveBackXSpaces = 0;
        ss.LeaderDrinkXTimes = 0;

        ApplyEffect(ss, sd.type, sd.amount, sd.spaces);

        if (sd.extras != null)
        {
            for (int i = 0; i < sd.extras.Length; i++)
            {
                SpaceEffect ex = sd.extras[i];
                if (ex == null) continue;
                ApplyEffect(ss, ex.type, ex.amount, ex.spaces);
            }
        }

        if (index == 0) ss.Start = true;
        if (index == totalSpaces - 1) ss.Finish = true;

        EditorUtility.SetDirty(ss);
    }

    static void ApplyEffect(SpaceSettings ss, string type, int amount, int spaces)
    {
        if (type == "start") ss.Start = true;
        else if (type == "finish") ss.Finish = true;
        else if (type == "mystery") ss.IsMystery = true;
        else if (type == "drink") ss.DrinkXTimes = amount > 0 ? amount : 1;
        else if (type == "everyoneDrink") ss.EveryoneDrinkXTimes = amount > 0 ? amount : 1;
        else if (type == "rollAgain") ss.RollAgain = true;
        else if (type == "moveForward") ss.MoveForwardXSpaces = spaces > 0 ? spaces : 1;
        else if (type == "moveBack") ss.MoveBackXSpaces = spaces > 0 ? spaces : 1;
        else if (type == "swapWithFirst") ss.SwapWithFirst = true;
        else if (type == "swapWithLast") ss.SwapWithLast = true;
        else if (type == "missTurn") ss.MissTurn = true;
        else if (type == "sendBackToStart") ss.SendBackToStart = true;
        else if (type == "drinkWhatYouRoll") ss.DrinkWhatYouRoll = true;
        else if (type == "drinkWithHost") ss.DrinkWithHost = true;
        else if (type == "chooseSomeoneToDrink") ss.ChooseSomeoneToDrink = true;
        else if (type == "girlsDrink") ss.GirlsDrink = true;
        else if (type == "guysDrink") ss.GuysDrink = true;
        else if (type == "immuneFromDrinking") ss.ImmuneFromDrinking = true;
        else if (type == "leaderMoveBack")
        {
            ss.LeaderMoveBackXSpaces = spaces > 0 ? spaces : 3;
            ss.LeaderDrinkXTimes = amount > 0 ? amount : 0;
        }
    }

    static void BakeMysteryPool(GameObject board, MysteryPoolEntry[] pool)
    {
        MysteryManager mm = Object.FindObjectOfType<MysteryManager>();

        if (mm == null)
        {
            Debug.LogWarning("MysteryManager not found in scene. Skipping mystery pool bake. Add a GameObject with MysteryManager component somewhere in the scene to bake pool data.");
            return;
        }

        string[] types = new string[pool.Length];
        int[] amounts = new int[pool.Length];
        int[] spacesArr = new int[pool.Length];
        int[] weights = new int[pool.Length];
        int total = 0;

        for (int i = 0; i < pool.Length; i++)
        {
            types[i] = pool[i].type;
            amounts[i] = pool[i].amount > 0 ? pool[i].amount : pool[i].spaces;
            spacesArr[i] = pool[i].spaces;
            weights[i] = pool[i].weight > 0 ? pool[i].weight : 1;
            total += weights[i];
        }

        mm.poolTypes = types;
        mm.poolAmounts = amounts;
        mm.poolSpaces = spacesArr;
        mm.poolWeights = weights;
        mm.totalWeight = total;

        EditorUtility.SetDirty(mm);
        Debug.Log($"Mystery pool baked: {pool.Length} entries, total weight {total}");
    }

    static void ClearChildren(GameObject parent)
    {
        for (int i = parent.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(parent.transform.GetChild(i).gameObject);
        }
    }

    static string ReturnTextBasedOffSetting(SpaceSettings spaceSettings, TextSettings textSettings, int spaceNumber)
    {
        if (spaceSettings.Start && !spaceSettings.Finish)
            return "Start";

        if (spaceSettings.IsMystery)
            return textSettings.MysteryText;

        string normalText = "";
        if (spaceSettings.DrinkXTimes > 0)
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
        if (spaceSettings.LeaderMoveBackXSpaces > 0)
        {
            string addAnd = normalText != "" ? " and " : "";
            string leaderText = textSettings.LeaderMoveBackXSpacesText.Replace("{x}", spaceSettings.LeaderMoveBackXSpaces.ToString());
            if (spaceSettings.LeaderDrinkXTimes > 0)
            {
                leaderText = leaderText + " + Drinks " + spaceSettings.LeaderDrinkXTimes;
            }
            normalText = normalText + addAnd + leaderText;
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
        if (spaceSettings.Start) return imageSettings.StartMat;
        if (spaceSettings.IsMystery) return imageSettings.MysteryMat;
        if (spaceSettings.Finish) return imageSettings.FinishMat;
        if (spaceSettings.SendBackToStart) return imageSettings.SendBackToStartMat;
        if (spaceSettings.LeaderMoveBackXSpaces > 0) return imageSettings.LeaderMoveBackXSpacesMat;
        if (spaceSettings.RollAgain) return imageSettings.RollAgainMat;
        if (spaceSettings.EveryoneDrinkXTimes > 0) return imageSettings.EveryoneDrinkXTimesMat;
        if (spaceSettings.DrinkXTimes > 0) return imageSettings.DrinkXTimesMat;
        if (spaceSettings.MoveBackXSpaces > 0) return imageSettings.MoveBackXSpacesMat;
        if (spaceSettings.MoveForwardXSpaces > 0) return imageSettings.MoveForwardXSpacesMat;
        if (spaceSettings.DrinkWhatYouRoll) return imageSettings.DrinkWhatYouRollMat;
        if (spaceSettings.SwapWithLast) return imageSettings.SwapWithLastMat;
        if (spaceSettings.SwapWithFirst) return imageSettings.SwapWithFirstMat;
        if (spaceSettings.ImmuneFromDrinking) return imageSettings.ImmuneFromDrinkingMat;
        if (spaceSettings.MissTurn) return imageSettings.MissTurnMat;
        if (spaceSettings.DrinkWithHost) return imageSettings.DrinkWithHostMat;
        if (spaceSettings.ChooseSomeoneToDrink) return imageSettings.ChooseSomeoneToDrinkMat;
        if (spaceSettings.GirlsDrink) return imageSettings.GirlsDrinkMat;
        if (spaceSettings.GuysDrink) return imageSettings.GuysDrinkMat;
        return imageSettings.DrinkXTimesMat;
    }

    // Keep legacy menu items for backwards compatibility during transition
    [MenuItem("GameObject/3D Object/CreateBoard")]
    static void CreateBoardLegacy()
    {
        GameObject board = Selection.activeGameObject;
        GameObject boardSettingsObject = board.transform.Find("CreateCustomBoard").Find("Scripts").Find("BoardSettings").gameObject;
        BoardSettings boardSettings = boardSettingsObject.GetComponent<BoardSettings>();
        GameObject spacePrefab = board.transform.Find("CreateCustomBoard").Find("Prefabs").Find("Space").gameObject;
        GameObject spaceHeader = board.transform.Find("Board").Find("Spaces").gameObject;
        GameObject spaceSettingObject = board.transform.Find("CreateCustomBoard").Find("Prefabs").Find("SpaceSetting").gameObject;
        GameObject spaceSettingsHeader = board.transform.Find("CreateCustomBoard").Find("Scripts").Find("SpaceSettings").gameObject;

        for (int i = 0; i < (int)boardSettings.boardSize; i++)
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

        for (int i = 0; i < spaceHeader.transform.childCount; i++)
        {
            SpaceSettings spaceSettings = spaceSettingsHeader.transform.GetChild(i).gameObject.GetComponent<SpaceSettings>();
            spaceHeader.transform.GetChild(i).Find("Canvas").Find("Text (1)").gameObject.GetComponent<Text>().text = ReturnTextBasedOffSetting(spaceSettings, textSettings, i);
            if (i == 0)
            {
                spaceSettings.Start = true;
            }
            spaceHeader.transform.GetChild(i).Find("SpaceImage").gameObject.GetComponent<Renderer>().material = ReturnMaterialBasedOffSetting(spaceSettings, imageSettings);
        }
    }
}

[System.Serializable]
public class BoardDefinition
{
    public string name;
    public int columns;
    public int rows;
    public SpaceDefinition[] spaces;
    public MysteryPoolEntry[] mysteryPool;
}

[System.Serializable]
public class SpaceDefinition
{
    public string type;
    public int amount;
    public int spaces;
    public int weight;
    public SpaceEffect[] extras;
    public string text;
}

[System.Serializable]
public class SpaceEffect
{
    public string type;
    public int amount;
    public int spaces;
}

[System.Serializable]
public class MysteryPoolEntry
{
    public string type;
    public int amount;
    public int spaces;
    public int weight;
}
