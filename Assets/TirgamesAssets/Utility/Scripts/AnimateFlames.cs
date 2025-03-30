
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class AnimateFlames : UdonSharpBehaviour
{
    public float PeriodMin = 2f;
    public float PeriodMax = 4f;
    public int IndexMin = 1;
    public int IndexMax = 1;
    public bool UseIndex = false;
    public string ParamName = "Start";
    public bool Loop = true;

    bool pulseFlame = false;
    float flameTimer = 10000f;
    float currentTime = 0;

    public Animator anim;
    void Start()
    {
        flameTimer = Random.Range(PeriodMin, PeriodMax);
    }
    private void Update()
    {
        currentTime = currentTime + Time.deltaTime;
        if(currentTime > flameTimer)
        {
            anim.SetTrigger("Start");
            flameTimer = Random.Range(PeriodMin, PeriodMax);
            currentTime = 0;
        }
    }
}
