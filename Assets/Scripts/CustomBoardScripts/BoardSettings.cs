
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BoardSettings : UdonSharpBehaviour
{
    public BoardSize boardSize;
    void Start()
    {
        
    }
}
public enum BoardSize
{
    TwoByTwo = 2,
    ThreeByThree = 3,
    FourByFour = 4,
    FiveByFive = 5,
    SixBySix = 6,
    SevenBySeven = 7,
    EightByEight = 8,
    NineByNine = 9,
    TenByTen = 10
}