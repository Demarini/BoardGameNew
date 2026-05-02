
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class AnyomBoard : UdonSharpBehaviour
{
    public GameObject originalBoard;
    public GameObject anyomBoard;
    void Start()
    {
        
    }
    private void Update()
    {
        if(Networking.LocalPlayer.displayName == "Anyom" && !anyomBoard.activeSelf)
        {
            originalBoard.SetActive(false);
            anyomBoard.SetActive(true);
        }
    }
}
