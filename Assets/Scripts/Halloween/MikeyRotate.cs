
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class MikeyRotate : UdonSharpBehaviour
{
    [Tooltip("Objects that should rotate to face the local player.")]
    public Transform[] targets;

    [Header("Behavior")]
    [Tooltip("If true, objects only rotate around Y (no pitch/roll).")]
    public bool yawOnly = true;

    [Tooltip("Apply smooth slerp toward the target rotation.")]
    public bool smooth = true;

    [Tooltip("Higher = faster rotation if smoothing is enabled.")]
    public float smoothSpeed = 10f;

    [Tooltip("0 = every frame. Increase to reduce frequency (seconds).")]
    public float updateInterval = 0f;

    private float _nextUpdateTime;

    void Start()
    {
        // Cache main camera for editor testing (Networking.LocalPlayer is null in editor).
    }

    void Update()
    {
        if (Time.time < _nextUpdateTime) return;
        if (updateInterval > 0f) _nextUpdateTime = Time.time + updateInterval;

        Vector3 playerPos = Networking.LocalPlayer.GetPosition();

        // Rotate each target to face the player
        for (int i = 0; i < targets.Length; i++)
        {
            var t = targets[i];
            if (t == null) continue;

            Vector3 toPlayer = playerPos - t.position;

            if (yawOnly)
            {
                toPlayer.y = 0f;
                if (toPlayer.sqrMagnitude < 1e-6f) continue;
                Quaternion targetRot = Quaternion.LookRotation(toPlayer.normalized, Vector3.up);
                ApplyRotation(t, targetRot);
            }
            else
            {
                if (toPlayer.sqrMagnitude < 1e-6f) continue;
                // Up vector stays world-up to avoid roll; keeps it readable
                Quaternion targetRot = Quaternion.LookRotation(toPlayer.normalized, Vector3.up);
                ApplyRotation(t, targetRot);
            }
        }
    }

    private void ApplyRotation(Transform t, Quaternion targetRot)
    {
        if (smooth)
        {
            // Frame-rate independent smoothing
            float k = 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime);
            t.rotation = Quaternion.Slerp(t.rotation, targetRot, k);
        }
        else
        {
            t.rotation = targetRot;
        }
    }
}
