
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

// Upstream client -> master channel for interactive "pick a player" spaces.
//
// Ownership of this object is pre-assigned to the current player at turn start
// (GameController.AssignChoiceRelayOwner), so by the time they pick from the menu
// they already own it and can serialize without a SetOwner round-trip.
//
// Only the OWNER can write synced vars, and the owner does NOT receive its own
// OnDeserialization -- so SubmitChoice resolves locally when the owner is master,
// and OnDeserialization resolves when the choice arrives from a remote chooser.
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class PlayerChoiceRelay : UdonSharpBehaviour
{
    [SerializeField] RollDiceHelper_BoardGame rollDiceHelper;

    [UdonSynced] public int chosenPlayerIndex = -1;
    [UdonSynced] public int choiceIncrement = 0;
    int localChoiceIncrement = 0;

    // Called on the chooser's client (the current owner of this object) from the menu.
    public void SubmitChoice(int playerIndex)
    {
        // Master resolves locally and must NOT touch the synced counter. NextPlayer (inside
        // OnChooseResolved) reassigns this relay's ownership in the SAME frame, so a queued
        // serialize from master would be dropped (only the owner serializes at end of frame),
        // leaving the next chooser one increment behind -- which the dedup below would reject.
        if (Networking.LocalPlayer.isMaster)
        {
            Debug.Log($"[Relay] Master submit, resolving locally for target={playerIndex}");
            rollDiceHelper.OnChooseResolved(playerIndex);
            return;
        }

        if (!Networking.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }
        chosenPlayerIndex = playerIndex;
        choiceIncrement++;
        localChoiceIncrement = choiceIncrement; // owner won't get its own OnDeserialization
        Debug.Log($"[Relay] Client submit target={playerIndex}, choiceIncrement={choiceIncrement}");
        RequestSerialization();
    }

    public override void OnDeserialization()
    {
        Debug.Log($"[Relay] OnDeserialization choiceIncrement={choiceIncrement}, local={localChoiceIncrement}, isMaster={Networking.LocalPlayer.isMaster}");
        if (choiceIncrement == localChoiceIncrement) return;
        localChoiceIncrement = choiceIncrement;

        // Master is authoritative: it alone acts on the received choice.
        if (Networking.LocalPlayer.isMaster)
        {
            rollDiceHelper.OnChooseResolved(chosenPlayerIndex);
        }
    }
}
