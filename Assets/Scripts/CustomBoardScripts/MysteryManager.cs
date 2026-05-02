
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class MysteryManager : UdonSharpBehaviour
{
    [SerializeField] GameController_BoardGame gameController;
    [SerializeField] RollDiceHelper_BoardGame rollDiceHelper;
    [SerializeField] GameVariables_BoardGame gameVariables;
    [SerializeField] ImageSettings imageSettings;
    [SerializeField] TextSettings textSettings;

    [Header("Mystery Pool - Baked by Editor Script")]
    public string[] poolTypes;
    public int[] poolAmounts;
    public int[] poolWeights;
    public int totalWeight;

    [Header("Board Visual Reference")]
    public GameObject boardSpacesVisual;

    [Header("Animation Settings")]
    public float spinDuration = 2.5f;
    public float minInterval = 0.04f;
    public float maxInterval = 0.3f;
    public AudioSource spinAudioSource;
    public AudioClip spinTickSound;
    public AudioClip spinLandSound;

    [Header("Runtime State")]
    [UdonSynced] public int mysteryResultIndex = -1;
    [UdonSynced] public int mysteryResolveIncrement;
    int localMysteryResolveIncrement;

    bool isSpinning;
    float spinTimer;
    float nextTickTime;
    float tickInterval;
    int displayIndex;
    int targetIndex;
    GameObject activeSpaceObject;
    Text activeSpaceText;
    Renderer activeSpaceRenderer;

    SpaceSettings pendingSpaceSettings;
    bool waitingForSpinComplete;

    public bool IsSpinning()
    {
        return isSpinning;
    }

    public void ResolveMystery(int spaceIndex)
    {
        if (Networking.LocalPlayer.isMaster)
        {
            targetIndex = PickWeightedRandom();
            mysteryResultIndex = targetIndex;
            mysteryResolveIncrement++;
            RequestSerialization();
        }
    }

    public override void OnDeserialization()
    {
        if (mysteryResolveIncrement != localMysteryResolveIncrement)
        {
            localMysteryResolveIncrement = mysteryResolveIncrement;
            targetIndex = mysteryResultIndex;
            StartSpinAnimation();
        }
    }

    void StartSpinAnimation()
    {
        isSpinning = true;
        spinTimer = 0f;
        tickInterval = minInterval;
        nextTickTime = 0f;
        displayIndex = 0;

        int playerSpace = 0;
        if (gameVariables != null && gameVariables.CurrentPlayerIndex >= 0)
        {
            playerSpace = gameVariables.playerSpaceDataList[gameVariables.CurrentPlayerIndex].Int;
        }

        int currentLandingSpace = gameController.CalculateLandingSpace(gameVariables.CurrentRoll, playerSpace);
        activeSpaceObject = boardSpacesVisual.transform.GetChild(currentLandingSpace).gameObject;

        Transform canvas = activeSpaceObject.transform.Find("Canvas");
        if (canvas != null)
        {
            Transform textObj = canvas.Find("Text (1)");
            if (textObj != null)
            {
                activeSpaceText = textObj.GetComponent<Text>();
            }
        }

        Transform spaceImage = activeSpaceObject.transform.Find("SpaceImage");
        if (spaceImage != null)
        {
            activeSpaceRenderer = spaceImage.GetComponent<Renderer>();
        }
    }

    void Update()
    {
        if (!isSpinning) return;

        spinTimer += Time.deltaTime;
        float progress = Mathf.Clamp01(spinTimer / spinDuration);

        float easedProgress = progress * progress * progress;
        tickInterval = Mathf.Lerp(minInterval, maxInterval, easedProgress);

        if (spinTimer >= nextTickTime)
        {
            nextTickTime = spinTimer + tickInterval;

            if (progress < 0.85f)
            {
                displayIndex = Random.Range(0, poolTypes.Length);
            }
            else
            {
                displayIndex = targetIndex;
            }

            UpdateSpaceVisuals(displayIndex);

            if (spinTickSound != null && spinAudioSource != null && progress < 0.85f)
            {
                spinAudioSource.pitch = Mathf.Lerp(1.2f, 0.8f, easedProgress);
                spinAudioSource.PlayOneShot(spinTickSound);
            }
        }

        if (progress >= 1f)
        {
            isSpinning = false;
            displayIndex = targetIndex;
            UpdateSpaceVisuals(targetIndex);

            if (spinLandSound != null && spinAudioSource != null)
            {
                spinAudioSource.pitch = 1f;
                spinAudioSource.PlayOneShot(spinLandSound);
            }

            if (Networking.LocalPlayer.isMaster && waitingForSpinComplete)
            {
                waitingForSpinComplete = false;
                ApplyMysteryResult();
                rollDiceHelper.OnMysteryResolved();
            }
        }
    }

    void UpdateSpaceVisuals(int poolIndex)
    {
        if (poolIndex < 0 || poolIndex >= poolTypes.Length) return;

        string type = poolTypes[poolIndex];
        int amount = poolAmounts[poolIndex];

        if (activeSpaceText != null)
        {
            activeSpaceText.text = GetTextForType(type, amount);
        }
        if (activeSpaceRenderer != null)
        {
            activeSpaceRenderer.material = GetMaterialForType(type);
        }
    }

    public void StartMysteryAndWait(SpaceSettings spaceSettings)
    {
        pendingSpaceSettings = spaceSettings;
        waitingForSpinComplete = true;
        ResolveMystery(0);
    }

    void ApplyMysteryResult()
    {
        if (pendingSpaceSettings == null) return;
        if (targetIndex < 0 || targetIndex >= poolTypes.Length) return;

        string type = poolTypes[targetIndex];
        int amount = poolAmounts[targetIndex];

        pendingSpaceSettings.RollAgain = false;
        pendingSpaceSettings.DrinkXTimes = 0;
        pendingSpaceSettings.SendBackToStart = false;
        pendingSpaceSettings.EveryoneDrinkXTimes = 0;
        pendingSpaceSettings.MoveBackXSpaces = 0;
        pendingSpaceSettings.MoveForwardXSpaces = 0;
        pendingSpaceSettings.DrinkWhatYouRoll = false;
        pendingSpaceSettings.SwapWithLast = false;
        pendingSpaceSettings.SwapWithFirst = false;
        pendingSpaceSettings.ImmuneFromDrinking = false;
        pendingSpaceSettings.MissTurn = false;
        pendingSpaceSettings.DrinkWithHost = false;
        pendingSpaceSettings.ChooseSomeoneToDrink = false;
        pendingSpaceSettings.GirlsDrink = false;
        pendingSpaceSettings.GuysDrink = false;

        if (type == "drink") pendingSpaceSettings.DrinkXTimes = amount;
        else if (type == "everyoneDrink") pendingSpaceSettings.EveryoneDrinkXTimes = amount;
        else if (type == "rollAgain") pendingSpaceSettings.RollAgain = true;
        else if (type == "moveForward") pendingSpaceSettings.MoveForwardXSpaces = amount;
        else if (type == "moveBack") pendingSpaceSettings.MoveBackXSpaces = amount;
        else if (type == "swapWithFirst") pendingSpaceSettings.SwapWithFirst = true;
        else if (type == "swapWithLast") pendingSpaceSettings.SwapWithLast = true;
        else if (type == "missTurn") pendingSpaceSettings.MissTurn = true;
        else if (type == "drinkWhatYouRoll") pendingSpaceSettings.DrinkWhatYouRoll = true;
        else if (type == "drinkWithHost") pendingSpaceSettings.DrinkWithHost = true;
        else if (type == "chooseSomeoneToDrink") pendingSpaceSettings.ChooseSomeoneToDrink = true;
        else if (type == "girlsDrink") pendingSpaceSettings.GirlsDrink = true;
        else if (type == "guysDrink") pendingSpaceSettings.GuysDrink = true;
        else if (type == "immuneFromDrinking") pendingSpaceSettings.ImmuneFromDrinking = true;
        else if (type == "sendBackToStart") pendingSpaceSettings.SendBackToStart = true;

        pendingSpaceSettings = null;
    }

    public SpaceSettings GetResolvedSpaceSettings()
    {
        return pendingSpaceSettings;
    }

    int PickWeightedRandom()
    {
        int roll = Random.Range(0, totalWeight);
        int cumulative = 0;
        for (int i = 0; i < poolWeights.Length; i++)
        {
            cumulative += poolWeights[i];
            if (roll < cumulative) return i;
        }
        return poolWeights.Length - 1;
    }

    string GetTextForType(string type, int amount)
    {
        if (type == "drink") return textSettings.DrinkXTimesText.Replace("{x}", amount.ToString());
        if (type == "everyoneDrink") return textSettings.EveryoneDrinkXTimesText.Replace("{x}", amount.ToString());
        if (type == "rollAgain") return textSettings.RollAgainText;
        if (type == "moveForward") return textSettings.MoveForwardXSpacesText.Replace("{x}", amount.ToString());
        if (type == "moveBack") return textSettings.MoveBackXSpacesText.Replace("{x}", amount.ToString());
        if (type == "swapWithFirst") return textSettings.SwapWithFirstText;
        if (type == "swapWithLast") return textSettings.SwapWithLastText;
        if (type == "missTurn") return textSettings.MissTurnText;
        if (type == "drinkWhatYouRoll") return textSettings.DrinkWhatYouRollText;
        if (type == "drinkWithHost") return textSettings.DrinkWithHostText;
        if (type == "chooseSomeoneToDrink") return textSettings.ChooseSomeoneToDrinkText;
        if (type == "girlsDrink") return textSettings.GirlsDrinkText;
        if (type == "guysDrink") return textSettings.GuysDrinkText;
        if (type == "immuneFromDrinking") return textSettings.ImmuneFromDrinkingText;
        if (type == "sendBackToStart") return textSettings.SendBackToStartText;
        return "???";
    }

    Material GetMaterialForType(string type)
    {
        if (type == "drink") return imageSettings.DrinkXTimesMat;
        if (type == "everyoneDrink") return imageSettings.EveryoneDrinkXTimesMat;
        if (type == "rollAgain") return imageSettings.RollAgainMat;
        if (type == "moveForward") return imageSettings.MoveForwardXSpacesMat;
        if (type == "moveBack") return imageSettings.MoveBackXSpacesMat;
        if (type == "swapWithFirst") return imageSettings.SwapWithFirstMat;
        if (type == "swapWithLast") return imageSettings.SwapWithLastMat;
        if (type == "missTurn") return imageSettings.MissTurnMat;
        if (type == "drinkWhatYouRoll") return imageSettings.DrinkWhatYouRollMat;
        if (type == "drinkWithHost") return imageSettings.DrinkWithHostMat;
        if (type == "chooseSomeoneToDrink") return imageSettings.ChooseSomeoneToDrinkMat;
        if (type == "girlsDrink") return imageSettings.GirlsDrinkMat;
        if (type == "guysDrink") return imageSettings.GuysDrinkMat;
        if (type == "immuneFromDrinking") return imageSettings.ImmuneFromDrinkingMat;
        if (type == "sendBackToStart") return imageSettings.SendBackToStartMat;
        return imageSettings.MysteryMat;
    }

}
