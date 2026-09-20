using UnityEngine;

public class MovementEM : MonoBehaviour
{
    [Tooltip("Transform to track (typically the CenterEyeAnchor of OVRCameraRig).")]
    public Transform headOrRig;

    [Tooltip("Speed (m/s) above which we consider 'started moving'.")]
    public float moveThreshold = 0.05f;

    [Tooltip("Seconds to wait after Start before checking movement.")]
    public float startDelay = 2f;

    private bool _fired;
    private bool _ready;
    private Vector3 _prevPos;

    void Start()
    {
        if (!headOrRig)
            headOrRig = Camera.main ? Camera.main.transform : transform;

        // Start coroutine to delay tracking
        StartCoroutine(InitAfterDelay());
    }

    System.Collections.IEnumerator InitAfterDelay()
    {
        yield return new WaitForSeconds(startDelay);
        _prevPos = headOrRig.position;
        _ready = true;
    }

    void Update()
    {
        if (!_ready || _fired) return;

        Vector3 p = headOrRig.position;
        float speed = (p - _prevPos).magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        _prevPos = p;

        if (speed > moveThreshold)
        {
            _fired = true;
            Debug.Log("EM: User starts moving");
            SendMessageUpwards("OnUserStartedMoving", SendMessageOptions.DontRequireReceiver);
        }
    }
}
