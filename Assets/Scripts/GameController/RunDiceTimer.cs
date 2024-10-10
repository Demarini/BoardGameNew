
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RunDiceTimer : UdonSharpBehaviour
{
    [SerializeField] GameController_BoardGame gameController;

    public bool RunTimer;
    float timeRan;
    public GameObject timerObject;
    void Update()
    {
        if (RunTimer)
        {
            timerObject.SetActive(true);
            timeRan = timeRan += Time.deltaTime;
            if(timeRan > 7)
            {
                Debug.Log("Timer Up: Check Interact");
                RunTimer = false;
                timeRan = 0;
                timerObject.SetActive(false);
                gameController.CheckToUpdateDiceClickerInteract();
            }
        }
    }
}
