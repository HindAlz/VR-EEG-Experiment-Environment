using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ReadPanelBB : MonoBehaviour
{
    [Header("UI")]
    public Canvas readCanvas;     // world-space canvas
    public Button readButton;     // the button the player clicks to read
    public Button closeButton;    // close button on the panel

    [Header("XR Rig")]
    public Transform xrOrigin;    // XR Origin / OVRPlayerController root
    public Transform head;        // CenterEyeAnchor / Main Camera

    [Header("Locomotion (Building Blocks or XRI)")]
    public Behaviour[] locomotionBlocks; // e.g. Continuous Move, Snap/Continuous Turn, Teleport, etc.

    [Header("Placement")]
    public float viewDistance = 1.25f;    // meters in front of head
    public float heightOffset = -0.05f;   // slight down offset feels nicer for reading
    public float faceCanvasSlerp = 10f;   // how fast the rig yaws toward canvas
    public float snapYawDuration = 0.25f; // duration to ease yaw

    bool isReading;
    Quaternion targetYaw;

    void Awake()
    {
        if (readCanvas) readCanvas.gameObject.SetActive(false);
        if (closeButton) closeButton.gameObject.SetActive(false);

        if (readButton) readButton.onClick.AddListener(OpenPanel);
        if (closeButton) closeButton.onClick.AddListener(ClosePanel);
    }

    // Call this if you still show a "Read" canvas/button in world
    public void OpenPanel()
    {
        if (!readCanvas || !xrOrigin || !head) return;

        // Place canvas in front of the head at a comfy distance and yaw-align it
        PositionCanvasInFront();

        readCanvas.gameObject.SetActive(true);
        if (closeButton) closeButton.gameObject.SetActive(true);

        // Disable BB locomotion blocks (providers)
        SetLocomotionEnabled(false);

        // Compute a target yaw so that the rig faces the canvas (horizontal only)
        Vector3 toCanvas = readCanvas.transform.position - xrOrigin.position;
        toCanvas.y = 0f;
        if (toCanvas.sqrMagnitude > 0.0001f)
            targetYaw = Quaternion.LookRotation(toCanvas.normalized, Vector3.up);
        else
            targetYaw = Quaternion.LookRotation(head.forward, Vector3.up);

        isReading = true;
        StopAllCoroutines();
        StartCoroutine(SnapYawCo());
    }

    public void ClosePanel()
    {
        isReading = false;

        if (readCanvas) readCanvas.gameObject.SetActive(false);
        if (closeButton) closeButton.gameObject.SetActive(false);

        // Re-enable locomotion
        SetLocomotionEnabled(true);
    }

    void LateUpdate()
    {
        if (!isReading || !xrOrigin) return;

        // Keep the panel comfortably in view if the user shifts a bit (billboard-lite)
        if (readCanvas && head)
        {
            // Optional soft billboard: just face the head, don’t re-place distance every frame
            Vector3 lookDir = head.position - readCanvas.transform.position;
            lookDir.y = 0f;
            if (lookDir.sqrMagnitude > 0.0001f)
            {
                Quaternion face = Quaternion.LookRotation(lookDir.normalized, Vector3.up);
                readCanvas.transform.rotation = Quaternion.Slerp(readCanvas.transform.rotation, face, Time.deltaTime * 8f);
            }
        }

        // Smoothly converge rig yaw (tiny adjustments over time = comfort)
        if (targetYaw != default)
        {
            xrOrigin.rotation = Quaternion.Slerp(xrOrigin.rotation, targetYaw, Time.deltaTime * faceCanvasSlerp);
        }
    }

    IEnumerator SnapYawCo()
    {
        if (!xrOrigin) yield break;
        float t = 0f;
        Quaternion start = xrOrigin.rotation;
        while (t < snapYawDuration)
        {
            t += Time.deltaTime;
            float u = Mathf.SmoothStep(0f, 1f, t / snapYawDuration);
            xrOrigin.rotation = Quaternion.Slerp(start, targetYaw, u);
            yield return null;
        }
    }

    void PositionCanvasInFront()
    {
        // Put canvas at a fixed distance in front of head (don’t parent; just place)
        Vector3 forwardFlat = head.forward; forwardFlat.y = 0f;
        if (forwardFlat.sqrMagnitude < 1e-4f) forwardFlat = xrOrigin.forward;

        Vector3 targetPos = head.position + forwardFlat.normalized * viewDistance;
        targetPos.y += heightOffset;

        readCanvas.transform.position = targetPos;

        // Face the head initially
        Vector3 dir = head.position - readCanvas.transform.position;
        dir.y = 0f;
        readCanvas.transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
    }

    void SetLocomotionEnabled(bool enable)
    {
        // Works for Meta Building Blocks locomotion components and XRI providers.
        if (locomotionBlocks != null)
        {
            foreach (var b in locomotionBlocks)
                if (b) b.enabled = enable;
        }

        // If you have your own BB manager that exposes a method, you can also call it here:
        // SendMessageUpwards("SetLocomotionEnabled", enable, SendMessageOptions.DontRequireReceiver);
    }
}
