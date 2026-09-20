using UnityEngine;
using System.Collections;
using Oculus.Interaction; // Meta XR Interaction SDK namespace

public class GrabEM : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private Grabbable grabbable; // assign in Inspector
    [SerializeField] private string objectName = "Grabbable";
    [SerializeField] private int taskNumber = 1;

    [Header("Flow")]
    public GameObject nextTask;     // object to enable after release
    public Transform rigRoot;       // OVRPlayerController or XR Rig root
    public Transform nextLocation;  // where to face next
    public float snapSpeed = 180f;
    public GameObject stuff;

    private void Awake()
    {
        if (grabbable == null)
            grabbable = GetComponent<Grabbable>();

        if (grabbable == null)
            Debug.LogError($"[GrabEM] ({name}) No Oculus.Interaction.Grabbable found.");
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
                Debug.Log($"EM: [Task {taskNumber}] {objectName} grabbed");
                SendMessageUpwards("OnObjectGrabbed", objectName, SendMessageOptions.DontRequireReceiver);
                break;

            case PointerEventType.Unselect:
                Debug.Log($"EM: [Task {taskNumber}] {objectName} released");
                SendMessageUpwards("OnObjectReleased", objectName, SendMessageOptions.DontRequireReceiver);

                if (stuff) stuff.SetActive(false);

                if (nextTask)
                {
                    nextTask.SetActive(true);
                    Debug.Log($"[Task {taskNumber}] Activated next task: {nextTask.name}");
                }

                if (rigRoot && nextLocation)
                {
                    StopAllCoroutines();
                    StartCoroutine(RotateRigYawToLocation(nextLocation));
                }
                else
                {
                    Debug.LogWarning($"[Task {taskNumber}] Missing rigRoot or nextLocation reference.");
                }
                break;
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
