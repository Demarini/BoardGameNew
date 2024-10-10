
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Player : UdonSharpBehaviour
{
    public bool IsInGame;
    public int id;
    public TestEnum testEnum;
    void Start()
    {
        
    }
}
public enum TestEnum
{
    One,
    Two,
    Three
}