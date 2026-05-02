
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RunMikeyPlayer : UdonSharpBehaviour
{
    [Header("Candidates (keep these inactive at start)")]
    public GameObject[] candidates;

    [Header("Timing (seconds)")]
    public float minInterval = 30f;
    public float maxInterval = 60f;

    [Tooltip("How often to re-check eligibility / gaze.")]
    public float pollInterval = 0.1f;

    [Header("Angles (degrees)")]
    [Tooltip("Minimum angle from player's view forward to count as 'behind' (e.g., 120 = well behind).")]
    public float behindMinDeg = 120f;

    [Tooltip("FOV threshold to dismiss active object when looked at.")]
    public float gazeFOVDeg = 45f;

    [Header("Dismiss behavior")]
    [Tooltip("Must keep the active object within FOV for this long before it disappears.")]
    public float dismissHoldSeconds = 0.25f;

    [Tooltip("Extra degrees of cushion to avoid flicker around the FOV edge.")]
    public float gazeHysteresisDeg = 3f;

    [Header("Distance filter (optional)")]
    [Tooltip("Ignore candidates closer than this distance (0 = no min).")]
    public float minDistance = 0f;

    [Tooltip("Ignore candidates farther than this distance (0 = no max).")]
    public float maxDistance = 0f;

    public AudioSource mikeySpotted;

    private float _nextTickTime;
    private float _nextTriggerTime;
    private int _activeIndex = -1;

    // Tracks how long we've been looking at the active object
    private float _lookStartTime = -1f;

    void Start()
    {
        if (maxInterval < minInterval) maxInterval = minInterval;

        // Ensure inactive at start
        for (int i = 0; i < candidates.Length; i++)
        {
            if (candidates[i] != null) candidates[i].SetActive(false);
        }

        ScheduleNextTrigger();
    }

    void Update()
    {
        VRCPlayerApi lp = Networking.LocalPlayer;
        if (lp == null) return;

        if (Time.time < _nextTickTime) return;
        _nextTickTime = Time.time + pollInterval;

        var head = lp.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
        Vector3 headPos = head.position;
        Vector3 headForward = head.rotation * Vector3.forward;

        if (_activeIndex >= 0)
        {
            GameObject go = candidates[_activeIndex];
            if (go == null)
            {
                _activeIndex = -1;
                _lookStartTime = -1f;
                ScheduleNextTrigger();
                return;
            }

            Vector3 toActive = go.transform.position - headPos;
            float angleToActive = Vector3.Angle(headForward, toActive);

            // Use hysteresis so we don't flap on the threshold
            float lookEnter = gazeFOVDeg;                     // start counting when inside this
            float lookExit = gazeFOVDeg + gazeHysteresisDeg; // reset timer only when outside this

            if (angleToActive <= lookEnter)
            {
                if (_lookStartTime < 0f) _lookStartTime = Time.time;

                if (Time.time - _lookStartTime >= dismissHoldSeconds)
                {
                    go.SetActive(false);
                    mikeySpotted.Play();
                    _activeIndex = -1;
                    _lookStartTime = -1f;
                    ScheduleNextTrigger();
                }
            }
            else if (angleToActive >= lookExit)
            {
                // Outside the relaxed FOV — stop counting
                _lookStartTime = -1f;
            }

            return;
        }

        // No active object: try to trigger
        if (Time.time >= _nextTriggerTime)
        {
            int[] pool = BuildEligiblePool(headPos, headForward);
            if (pool == null || pool.Length == 0)
            {
                _nextTriggerTime = Time.time + Mathf.Min(2f, Mathf.Max(pollInterval, 0.25f));
                return;
            }

            int pick = pool[Random.Range(0, pool.Length)];
            GameObject choice = candidates[pick];
            if (choice != null)
            {
                choice.SetActive(true);
                _activeIndex = pick;
                _lookStartTime = -1f; // reset look timer when a new one appears
                // Timer remains paused until dismissed
            }
            else
            {
                ScheduleNextTrigger();
            }
        }
    }

    private void ScheduleNextTrigger()
    {
        float wait = Random.Range(minInterval, maxInterval);
        _nextTriggerTime = Time.time + wait;
    }

    private bool PassesDistance(float sqrDist)
    {
        if (minDistance > 0f && sqrDist < (minDistance * minDistance)) return false;
        if (maxDistance > 0f && sqrDist > (maxDistance * maxDistance)) return false;
        return true;
    }

    private int[] BuildEligiblePool(Vector3 headPos, Vector3 headForward)
    {
        int count = 0;
        for (int i = 0; i < candidates.Length; i++)
            if (IsEligible(i, headPos, headForward)) count++;

        if (count == 0) return new int[0];

        int[] pool = new int[count];
        int w = 0;
        for (int i = 0; i < candidates.Length; i++)
            if (IsEligible(i, headPos, headForward)) pool[w++] = i;

        return pool;
    }

    private bool IsEligible(int idx, Vector3 headPos, Vector3 headForward)
    {
        GameObject go = candidates[idx];
        if (go == null) return false;
        if (go.activeSelf) return false;

        Vector3 toObj = go.transform.position - headPos;
        float sqr = toObj.sqrMagnitude;
        if (sqr < 1e-6f) return false;
        if (!PassesDistance(sqr)) return false;

        float angle = Vector3.Angle(headForward, toObj);
        return angle >= behindMinDeg;
    }
}
