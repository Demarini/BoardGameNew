
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RotateHourglass : UdonSharpBehaviour
{
    public GameObject hourGlass;
    public GameObject followPlayer;
    void Start()
    {
        
    }
    public void Update()
    {
        //followPlayer.transform.position = Networking.LocalPlayer.GetPosition();
        //hourGlass.transform.LookAt(followPlayer.transform);
    }
}
