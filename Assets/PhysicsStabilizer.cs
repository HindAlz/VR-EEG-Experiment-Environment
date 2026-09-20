using System.Collections;
using UnityEngine;

public class PhysicsStabilizer : MonoBehaviour
{
    [Tooltip("Force safe physics defaults on scene load.")]
    public bool applySafeDefaults = true;

    [Tooltip("Seconds to keep dynamic Rigidbodies kinematic on load.")]
    public float settleSeconds = 0.1f;

    [Tooltip("Clamp how violently Unity separates overlaps (lower = safer).")]
    public float maxDepenVel = 3f;

    void Awake()
    {
        if (applySafeDefaults)
        {
            Physics.gravity = new Vector3(0, -9.81f, 0);
            Physics.defaultMaxDepenetrationVelocity = maxDepenVel;
            // Optionally: Physics.defaultContactOffset = 0.01f;
        }
    }

    void Start() => StartCoroutine(FreezeThenRelease());

    IEnumerator FreezeThenRelease()
    {
        var rbs = FindObjectsOfType<Rigidbody>(includeInactive: false);

        foreach (var rb in rbs)
        {
            if (!rb || rb.isKinematic) continue;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.Sleep();
            rb.isKinematic = true;
        }

        yield return new WaitForSeconds(settleSeconds);

        foreach (var rb in rbs)
        {
            if (!rb) continue;
            if (rb.isKinematic) rb.isKinematic = false;
            rb.WakeUp();
        }
    }
}
