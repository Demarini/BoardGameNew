
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

// One pre-made button in the chooser menu's pool. PlayerChoiceMenu re-assigns
// slotIndex + label each time the menu opens; slotIndex equals the player index
// this button targets. Wire the UI Button's OnClick -> OnPressed in the editor.
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class PlayerChoiceButton : UdonSharpBehaviour
{
    [SerializeField] PlayerChoiceMenu menu;
    public Text label;
    public int slotIndex = -1;

    public void OnPressed()
    {
        if (menu != null) menu.OnSlotChosen(slotIndex);
    }
}
