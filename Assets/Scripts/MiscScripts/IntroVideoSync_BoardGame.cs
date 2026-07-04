
using UdonSharp;
using UnityEngine;
using UdonSharp.Video;
using VRC.SDKBase;
using VRC.Udon;

// Keeps an intro animation (e.g. flag / jets) in sync with a USharpVideo by keying it to a
// SINGLE synced moment -- the master's video-play -- instead of each client's own video load.
//
// Why: every client loads the video at a different pace (sometimes seconds apart). If each
// started its animation on its own OnUSharpVideoPlay, the animations would drift apart. USharpVideo
// already seeks late loaders to the network-synced video position, so instead we fire ONE synced
// flag when the master's video starts; every client animates off that. By the time a slow client's
// video finally appears (already seeked to the right spot), its animation is near-synced to it.
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class IntroVideoSync_BoardGame : UdonSharpBehaviour
{
    [Tooltip("The USharpVideo player whose play/end we key the animation to.")]
    [SerializeField] USharpVideoPlayer videoPlayer;
    [Tooltip("Animator whose bool is toggled to run the intro animation. Optional.")]
    [SerializeField] Animator introAnimator;
    [Tooltip("Bool parameter on the Animator to enable/disable the intro animation.")]
    [SerializeField] string introAnimBool = "Play";
    [Tooltip("Optional GameObject enabled while the animation plays (leave empty if unused).")]
    [SerializeField] GameObject animationObject;

    // Master flips this true when THEIR video starts playing, and false when the GameController
    // intro TIMER ends (see StopAnimationSynced) -- deliberately NOT on video end, so trailing
    // effects like smoke linger past the anthem. FieldChangeCallback runs on every client.
    [UdonSynced, FieldChangeCallback(nameof(AnimationPlaying))]
    public bool animationPlaying;
    public bool AnimationPlaying
    {
        set
        {
            bool wasPlaying = animationPlaying;
            animationPlaying = value;
            // Only act on an actual change so a late joiner arriving AFTER it's false (default)
            // doesn't replay the animation.
            if (value && !wasPlaying) StartAnimationLocal();
            else if (!value && wasPlaying) StopAnimationLocal();
        }
        get => animationPlaying;
    }

    void Start()
    {
        // Subscribe so USharpVideo forwards its OnUSharpVideo* events to this behaviour.
        if (videoPlayer != null) videoPlayer.RegisterCallbackReceiver(this);
    }

    // Fired when a new video begins loading. Clears any stale flag so the next play re-triggers
    // cleanly -- covers the case where a prior StopAnimationSynced was missed (e.g. master left
    // mid-intro) and the flag was left true from the previous game.
    public void OnUSharpVideoLoadStart()
    {
        if (Networking.LocalPlayer.isMaster && animationPlaying)
        {
            TakeOwnershipIfNeeded();
            AnimationPlaying = false;
            RequestSerialization();
        }
    }

    // Fired locally on each client when its own video actually starts playing.
    public void OnUSharpVideoPlay()
    {
        // Key the synced animation to the MASTER's play moment only.
        if (Networking.LocalPlayer.isMaster && !animationPlaying)
        {
            TakeOwnershipIfNeeded();
            AnimationPlaying = true;   // runs StartAnimationLocal() here on master
            RequestSerialization();    // broadcast the flag to everyone else
        }
    }

    // Called by GameController when the intro TIMER ends -- NOT when the video ends. This lets
    // lingering effects (e.g. smoke trails) keep going after the anthem finishes, right up until
    // the game controller tears the whole intro down. Master-authoritative + synced so every
    // client stops at the same moment and late joiners don't replay it.
    public void StopAnimationSynced()
    {
        if (Networking.LocalPlayer.isMaster && animationPlaying)
        {
            TakeOwnershipIfNeeded();
            AnimationPlaying = false;
            RequestSerialization();
        }
    }

    void TakeOwnershipIfNeeded()
    {
        if (!Networking.IsOwner(gameObject))
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

    void StartAnimationLocal()
    {
        if (animationObject != null) animationObject.SetActive(true);
        if (introAnimator != null && introAnimBool.Length > 0) introAnimator.SetBool(introAnimBool, true);
    }

    void StopAnimationLocal()
    {
        if (introAnimator != null && introAnimBool.Length > 0) introAnimator.SetBool(introAnimBool, false);
        if (animationObject != null) animationObject.SetActive(false);
    }
}
