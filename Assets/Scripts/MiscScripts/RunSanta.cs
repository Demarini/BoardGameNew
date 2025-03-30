
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RunSanta : UdonSharpBehaviour
{

    public Animator santaAnimController;
    public float santaTimer = 0;


    [UdonSynced, FieldChangeCallback(nameof(SantaAnim))]
    public int santaAnim = 0;
    public int SantaAnim
    {
        set
        {
            santaAnim = value;
            //Debug.Log("Shuffled Updated");
            //Debug.Log(shuffled);
        }
        get => santaAnim;
    }
    public int tmpSantaAnim = 0;

    private void Start()
    {
        if (Networking.LocalPlayer.isMaster)
        {
            SantaAnim++;
            RequestSerialization();
            Debug.Log("UpdateDd santa anim start");
        }
    }
    private void Update()
    {
        if (Networking.LocalPlayer.isMaster)
        {
            if (santaTimer > 240)
            {
                if (SantaAnim == 6)
                {
                    SantaAnim = 1;
                }
                else
                {
                    SantaAnim++;
                }
                RequestSerialization();
                santaTimer = 0;
            }
            else
            {
                santaTimer = Time.deltaTime + santaTimer;
            }
        }
        else
        {
            Debug.Log("Not master lol");
        }
    }
    public override void OnPreSerialization()
    {
        Debug.Log("On Preserialization");
        if (tmpSantaAnim != SantaAnim)
        {
            Debug.Log("Updating Animation");
            santaAnimController.SetInteger("SantaMov", SantaAnim);
            tmpSantaAnim = SantaAnim;
        }
    }
    public override void OnDeserialization()
    {
        if(tmpSantaAnim != SantaAnim)
        {
            santaAnimController.SetInteger("SantaMov", SantaAnim);
            tmpSantaAnim = SantaAnim;
        }
    }
}
