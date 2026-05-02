
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ToggleFireworkSounds : UdonSharpBehaviour
{
    public GameObject[] sounds;
    bool needToToggle = false;
    int toggleValue = 0;
    void Start()
    {
        Debug.Log("Entered Toggle Sound");
    }
    void Update()
    {
        if (needToToggle)
        {
            Debug.Log("Need to Toggle");
            sounds[toggleValue].SetActive(true);
            toggleValue = 0;
            needToToggle = false;
        }
    }
    public void ToggleSound()
    {
        Debug.Log("TEST ANIMATION");
        int random = Random.Range(0, sounds.Length);
        Debug.Log("Got Random Value " + random.ToString());
        Debug.Log(sounds[random].name);
        sounds[random].SetActive(false);
        needToToggle = true;
        toggleValue = random;
        
    }
}
