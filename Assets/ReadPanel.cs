using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ReadPanel : MonoBehaviour
{
    public Canvas readCanvas;
    public Canvas readButton;
    public Canvas closeButton;
    public GameObject stuff;
    public GameObject coll;

    [Header("Player Rig References")]
    public Transform playerCamera;  // CenterEyeAnchor or XR Camera
    public Transform rigRoot;       // OVRPlayerController or XR Rig root

    [Header("Next Task & Location")]
    public GameObject nextTask;     // Object or group to enable next
    public GameObject pickUp;


    [Header("Debug Info")]
    public int taskNumber = 1;      // 👈 Assign this in the Inspector (1, 2, 3, ...)

    [Tooltip("How fast the rig rotates toward the next location (degrees per second). Set to 0 for instant snap.")]
    public float snapSpeed = 180f;

    private bool isReading = false;
    //public GameObject goNext;
    void Awake()
    {
        if (readCanvas) readCanvas.gameObject.SetActive(false);
        if (readButton) readButton.gameObject.SetActive(false);
        if (closeButton) closeButton.gameObject.SetActive(false);

        if (!rigRoot && playerCamera)
        {
            // Try to auto-find parent rig if not assigned
            Transform t = playerCamera;
            for (int i = 0; i < 4 && t != null; i++) t = t.parent;
            if (t) rigRoot = t;
        }
    }

    public void OpenPanel()
    {
        Debug.Log($"EM: [Task {taskNumber}] Read button pressed");
        if (readCanvas) readCanvas.gameObject.SetActive(true);
        if (closeButton) closeButton.gameObject.SetActive(true);
        if (readButton) readButton.gameObject.SetActive(false);
        isReading = true;
    }

    public void ClosePanel()
    {
        Debug.Log($"EM: [Task {taskNumber}] Read text closed");

        if (readCanvas) readCanvas.gameObject.SetActive(false);
        if (closeButton) closeButton.gameObject.SetActive(false);

        isReading = false;

        if (taskNumber == 1) {
            if (stuff) stuff.SetActive(false);
            //goNext.SetActive(true);
            // ✅ Step 1: Activate the next task logic/UI
            if (nextTask)
            {
                nextTask.SetActive(true);
                Debug.Log($"[Task {taskNumber}] Activated next task: {nextTask.name}");
            }

        }
        else
        {
            pickUp.SetActive(true);
        }
    }

    private IEnumerator RotateRigYawToLocation(Transform target)
    {
        Vector3 from = rigRoot.position;
        Vector3 to = target.position;

        Vector3 dir = to - from;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) yield break;

        Quaternion targetYaw = Quaternion.LookRotation(dir, Vector3.up);

        if (snapSpeed <= 0f)
        {
            Vector3 te = targetYaw.eulerAngles;
            rigRoot.rotation = Quaternion.Euler(0f, te.y, 0f);
            Debug.Log($"[Task {taskNumber}] Snapped rig to face {target.name}");
            yield break;
        }

        // Smooth yaw rotation
        while (true)
        {
            Vector3 te = targetYaw.eulerAngles;
            Quaternion desiredYaw = Quaternion.Euler(0f, te.y, 0f);
            rigRoot.rotation = Quaternion.RotateTowards(rigRoot.rotation, desiredYaw, snapSpeed * Time.deltaTime);

            if (Quaternion.Angle(rigRoot.rotation, desiredYaw) <= 0.5f)
            {
                rigRoot.rotation = desiredYaw;
                Debug.Log($"[Task {taskNumber}] Finished rotating toward {target.name}");
                break;
            }
            yield return null;
        }
    }
}
