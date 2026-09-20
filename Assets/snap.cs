using UnityEngine;
using UnityEngine.Events;
using UnityEngine;
using System.Collections;
using Oculus.Interaction; // Meta XR Interaction SDK namespace


[RequireComponent(typeof(Rigidbody))]
public class snap : MonoBehaviour
{
    bool hasPlayed;
    public AudioSource audioSource;

    public enum RotationAxis { X, Y, Z }

    [Header("Snap Settings (for the 2 snappable paintings)")]
    public bool canSnap = false;
    public RotationAxis rotationAxis = RotationAxis.Z;
    [Range(0f, 360f)] public float targetAngle = 0f;

    [SerializeField] private Grabbable grabbable; // assign in Inspector
    [Tooltip("Enter alignment when within this many degrees.")]
    [Range(0f, 45f)] public float alignEnterDeg = 7.5f;

    [Tooltip("Declare 'snapped' when within this many degrees (must be < alignEnterDeg).")]
    [Range(0f, 10f)] public float snappedStateDeg = 1.0f;

    [Tooltip("If angle drifts back out by this much beyond snappedStateDeg, we consider it unlocked.")]
    [Range(0f, 10f)] public float snapReleaseHysteresisDeg = 0.75f;

    [Tooltip("Critically-damped rotation time (sec) used by SmoothDampAngle during alignment.")]
    [Range(0.02f, 0.6f)] public float snapSmoothTime = 0.12f;

    [Tooltip("Low-pass filter factor for measured angle (0=no LPF, 1=freeze).")]
    [Range(0f, 0.5f)] public float angleLowPass = 0.25f;

    [Tooltip("Hard cap per physics step if SmoothDamp would move too far (deg/FixedUpdate).")]
    public float maxDegPerStep = 8f;

    [Tooltip("Stay inside snappedStateDeg for at least this time before declaring snapped (sec).")]
    [Range(0f, 0.25f)] public float snappedDwellTime = 0.06f;

    [Header("Behavior")]
    [Tooltip("If true, aligning/repelling acts only when released. If false, can act while held.")]
    public bool onlyWhenReleased = true;

    [Tooltip("Temporarily set Rigidbody.isKinematic = true while aligning/repelling to eliminate physics jitter.")]
    public bool kinematicWhileAligning = true;

    [Header("Lock After Snapped (snappable only)")]
    public bool disableGrabbingWhenSnapped = true;
    public bool makeKinematicWhenSnapped = true;

    [Header("Non-Snappable: Forbidden Angle (repel)")]
    public bool forbidStraightAngle = true;
    [Range(0f, 360f)] public float forbiddenAngle = 0f;
    [Tooltip("Enter repel band within this many degrees of the forbidden angle.")]
    [Range(0.1f, 15f)] public float forbidEnterDeg = 2f;
    [Tooltip("Exit repel band only after exceeding enter + this margin (hysteresis).")]
    [Range(0f, 10f)] public float forbidReleaseHysteresisDeg = 1.5f;
    [Tooltip("Extra margin beyond the band we bias toward when repelling.")]
    [Range(0f, 10f)] public float keepOutMarginDeg = 1.5f;

    [Header("Events")]
    public UnityEvent onSnappedEnter;
    public UnityEvent<bool> onSnappedChanged; // true=snapped, false=unsnapped
    bool grabpaused;
    Rigidbody _rb;
    OVRGrabbable _grabbable;
    Collider[] _grabPointColliders;

    // State
    bool _isSnapped;                  public bool IsSnapped => _isSnapped;
    bool _pendingGrabLockUntilReleased;
    bool _aligning;
    bool _repelling;
    bool _inForbidBand;
    float _insideSnapWindowTimer;
    float _angleVel;                  // for SmoothDampAngle
    float _angleLPF;                  // low-pass filtered measured angle
    bool _hadKinematicOverride;       // remember if we toggled kinematic this frame

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _grabbable = GetComponent<OVRGrabbable>();
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.angularDamping = Mathf.Max(_rb.angularDamping, 0.05f);

        if (_grabbable != null && _grabbable.grabPoints != null && _grabbable.grabPoints.Length > 0)
            _grabPointColliders = _grabbable.grabPoints;
        else
            _grabPointColliders = GetComponents<Collider>(); // fallback

        _angleLPF = GetAxisAngle(_rb.rotation.eulerAngles);
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        bool isGrabbed = _grabbable != null && _grabbable.isGrabbed;
        if(isGrabbed) Debug.Log("EM: grabbed painting");
        if (isGrabbed & !grabpaused) {
            grabpaused = true;
            Debug.Log("EM: grabbed painting");
        } else if (!isGrabbed & grabpaused)
        {
            Debug.Log("EM: let go of painting");
            grabpaused = false;


        }
        // Pending lock if we snapped while held
        if (_pendingGrabLockUntilReleased && !isGrabbed)
        {
            LockGrabbing();
            _pendingGrabLockUntilReleased = false;
        }

        _hadKinematicOverride = false;

        if (canSnap)
        {
            // ----- Snappable path -----
            if (onlyWhenReleased && isGrabbed)
            {
                TrackSnappedDwell(dt);
                return;
            }

            float currentRaw = GetAxisAngle(_rb.rotation.eulerAngles);
            _angleLPF = Mathf.LerpAngle(_angleLPF, currentRaw, Mathf.Clamp01(angleLowPass));
            float delta = Mathf.Abs(Mathf.DeltaAngle(_angleLPF, targetAngle));

            // Align enter/exit with hysteresis
            if (!_aligning && delta <= alignEnterDeg) _aligning = true;
            else if (_aligning && delta > alignEnterDeg + 0.5f) _aligning = false;

            if (_aligning)
            {
                BeginKinematicOverride();

                // SmoothDampAngle toward the target on the chosen axis
                float newAngle = Mathf.SmoothDampAngle(_angleLPF, targetAngle, ref _angleVel, snapSmoothTime, Mathf.Infinity, dt);

                // Cap per-step move
                float moved = Mathf.DeltaAngle(_angleLPF, newAngle);
                if (Mathf.Abs(moved) > maxDegPerStep)
                    newAngle = _angleLPF + Mathf.Sign(moved) * maxDegPerStep;

                // Build rotation from current Euler with the smoothed angle
                Vector3 e = _rb.rotation.eulerAngles;
                Quaternion targetRot = rotationAxis switch
                {
                    RotationAxis.X => Quaternion.Euler(newAngle, e.y, e.z),
                    RotationAxis.Y => Quaternion.Euler(e.x, newAngle, e.z),
                    _               => Quaternion.Euler(e.x, e.y, newAngle),
                };

                // Kill physics spin so we stay smooth
                _rb.angularVelocity = Vector3.zero;
                _rb.MoveRotation(targetRot);

                _angleLPF = newAngle; // advance filtered angle to new commanded angle
            }

            // Dwell & finalize
            bool wasSnapped = _isSnapped;
            TrackSnappedDwell(dt);

            if (!_isSnapped && _insideSnapWindowTimer >= snappedDwellTime)
            {
                // Hard-set exact target and settle
                HardSetToTarget();
                SetSnapped(true);

                if (disableGrabbingWhenSnapped)
                {
                    if (isGrabbed) _pendingGrabLockUntilReleased = true;
                    else LockGrabbing();
                }
                if (makeKinematicWhenSnapped) _rb.isKinematic = true;

                _aligning = false;
                _angleVel = 0f;
            }

            // Unlock if drifted out beyond hysteresis (rare unless you re-enable grab)
            if (_isSnapped)
            {
                float unlockDelta = snappedStateDeg + snapReleaseHysteresisDeg;
                float deltaNow = Mathf.Abs(Mathf.DeltaAngle(GetAxisAngle(_rb.rotation.eulerAngles), targetAngle));
                if (deltaNow > unlockDelta)
                {
                    SetSnapped(false);
                    if (makeKinematicWhenSnapped) _rb.isKinematic = false;
                }
            }
        }
        else
        {
            // ----- Non-snappable path: repel from forbidden angle -----
            SetSnapped(false);

            if (!forbidStraightAngle) return;
            if (onlyWhenReleased && isGrabbed) return;

            float currentRaw = GetAxisAngle(_rb.rotation.eulerAngles);
            _angleLPF = Mathf.LerpAngle(_angleLPF, currentRaw, Mathf.Clamp01(angleLowPass));

            float d = Mathf.Abs(Mathf.DeltaAngle(_angleLPF, forbiddenAngle));
            float exitThresh = forbidEnterDeg + forbidReleaseHysteresisDeg;

            if (!_inForbidBand && d <= forbidEnterDeg) _inForbidBand = true;
            else if (_inForbidBand && d > exitThresh) _inForbidBand = false;

            if (_inForbidBand)
            {
                BeginKinematicOverride();
                if (!hasPlayed)
                {
                    audioSource.Play();
                    hasPlayed = true;

                }
                float sign = Mathf.Sign(Mathf.DeltaAngle(_angleLPF, forbiddenAngle));
                if (sign == 0f) sign = 1f;
                float safe = Normalize360(forbiddenAngle + sign * (forbidEnterDeg + keepOutMarginDeg));

                float newAngle = Mathf.SmoothDampAngle(_angleLPF, safe, ref _angleVel, snapSmoothTime, Mathf.Infinity, dt);
                float moved = Mathf.DeltaAngle(_angleLPF, newAngle);
                if (Mathf.Abs(moved) > maxDegPerStep)
                    newAngle = _angleLPF + Mathf.Sign(moved) * maxDegPerStep;

                Vector3 e = _rb.rotation.eulerAngles;
                Quaternion targetRot = rotationAxis switch
                {
                    RotationAxis.X => Quaternion.Euler(newAngle, e.y, e.z),
                    RotationAxis.Y => Quaternion.Euler(e.x, newAngle, e.z),
                    _               => Quaternion.Euler(e.x, e.y, newAngle),
                };

                _rb.angularVelocity = Vector3.zero;
                _rb.MoveRotation(targetRot);
                _angleLPF = newAngle;
                _repelling = true;
            }
            else
            {
                _repelling = false;
                _angleVel = 0f;
            }
        }

        // If we temporarily made it kinematic for stability, and we’re not snapped or actively repelling/aligning, revert
        if (kinematicWhileAligning && _hadKinematicOverride && !_aligning && !_repelling && !_isSnapped)
            _rb.isKinematic = false;
    }
    private void OnEnable()
    {
        if (grabbable != null)
        {
            // These are simple Action events in the Meta XR SDK
            grabbable.WhenPointerEventRaised += OnPointerEventRaised;
        }
    }

    private void OnDisable()
    {
        if (grabbable != null)
            grabbable.WhenPointerEventRaised -= OnPointerEventRaised;
    }

    private void OnPointerEventRaised(PointerEvent evt)
    {
        // Meta XR’s PointerEvent.Type tells us what happened
        switch (evt.Type)
        {
            case PointerEventType.Select:
                Debug.Log($"EM: painting grabbed");
                //SendMessageUpwards("OnObjectGrabbed", objectName, SendMessageOptions.DontRequireReceiver);
                break;

            case PointerEventType.Unselect:
                Debug.Log($"EM: painting released");
                //SendMessageUpwards("OnObjectReleased", objectName, SendMessageOptions.DontRequireReceiver);
                break;
        }
    }
    // ---------- Helpers ----------
    void BeginKinematicOverride()
    {
        if (!kinematicWhileAligning) return;
        if (!_rb.isKinematic) { _rb.isKinematic = true; _hadKinematicOverride = true; }
        else _hadKinematicOverride = true;
    }

    void TrackSnappedDwell(float dt)
    {
        float current = GetAxisAngle(_rb.rotation.eulerAngles);
        float delta = Mathf.Abs(Mathf.DeltaAngle(current, targetAngle));
        if (delta <= snappedStateDeg) _insideSnapWindowTimer += dt;
        else _insideSnapWindowTimer = 0f;
    }

    void HardSetToTarget()
    {
        Vector3 e = _rb.rotation.eulerAngles;
        Quaternion targetRot = rotationAxis switch
        {
            RotationAxis.X => Quaternion.Euler(targetAngle, e.y, e.z),
            RotationAxis.Y => Quaternion.Euler(e.x, targetAngle, e.z),
            _               => Quaternion.Euler(e.x, e.y, targetAngle),
        };
        _rb.MoveRotation(targetRot);
        _rb.angularVelocity = Vector3.zero;
        _angleLPF = targetAngle;
        _angleVel = 0f;
    }

    void SetSnapped(bool state)
    {
        if (_isSnapped == state) return;
        _isSnapped = state;
        onSnappedChanged?.Invoke(_isSnapped);
        if (_isSnapped) onSnappedEnter?.Invoke();
    }

    float GetAxisAngle(Vector3 euler)
    {
        return rotationAxis switch
        {
            RotationAxis.X => Normalize360(euler.x),
            RotationAxis.Y => Normalize360(euler.y),
            _               => Normalize360(euler.z),
        };
    }

    static float Normalize360(float degrees)
    {
        degrees %= 360f;
        if (degrees < 0f) degrees += 360f;
        return degrees;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = canSnap ? Color.yellow : (forbidStraightAngle ? Color.red : Color.gray);
        Vector3 pos = transform.position;
        Vector3 dir = rotationAxis == RotationAxis.X ? transform.right :
                      rotationAxis == RotationAxis.Y ? transform.up :
                      transform.forward;
        Gizmos.DrawLine(pos, pos + dir * 0.5f);
        Gizmos.DrawWireSphere(pos + dir * 0.5f, 0.03f);
    }

    void LockGrabbing()
    {
        Debug.Log("EM: Locked painting");
        if (_grabbable != null) _grabbable.enabled = false;
        if (_grabPointColliders != null)
            foreach (var c in _grabPointColliders) if (c) c.enabled = false;
    }
}
