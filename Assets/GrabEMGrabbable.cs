using UnityEngine;

public class GrabEMGrabbable : OVRGrabbable
{
    [Header("EM")]
    public string objectName = "Grabbable";
    public int taskNumber = 1;
    public GameObject nextTask;
    public Transform rigRoot;
    public Transform nextLocation;
    public float snapSpeed = 180f;
    public GameObject stuff;

    public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        base.GrabBegin(hand, grabPoint);
        Debug.Log($"EM: [Task {taskNumber}] {objectName} grabbed (by {hand?.name}, via {grabPoint?.name})");
        SendMessageUpwards("OnObjectGrabbed", objectName, SendMessageOptions.DontRequireReceiver);
    }

    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        base.GrabEnd(linearVelocity, angularVelocity);
        Debug.Log($"EM: [Task {taskNumber}] {objectName} released");

        SendMessageUpwards("OnObjectReleased", objectName, SendMessageOptions.DontRequireReceiver);
        if (stuff) stuff.SetActive(false);

        if (nextTask)
        {
            nextTask.SetActive(true);
            Debug.Log($"[Task {taskNumber}] Activated next task: {nextTask.name}");
        }

        if (rigRoot && nextLocation)
            (new GameObject("GrabEM_Rotator")).AddComponent<_OneShotRotator>()
                .Init(rigRoot, nextLocation, snapSpeed, taskNumber);
        else
            Debug.LogWarning($"[Task {taskNumber}] Missing rigRoot or nextLocation reference.");
    }

    // helper to rotate after release without needing a Mono on the same object
    private class _OneShotRotator : MonoBehaviour
    {
        Transform rig, target;
        float speed; int task;
        public void Init(Transform r, Transform t, float s, int taskNum)
        { rig = r; target = t; speed = s; task = taskNum; StartCoroutine(Rotate()); }
        System.Collections.IEnumerator Rotate()
        {
            Vector3 dir = target.position - rig.position; dir.y = 0;
            if (dir.sqrMagnitude < 1e-5f) { Destroy(gameObject); yield break; }
            Quaternion targetYaw = Quaternion.LookRotation(dir, Vector3.up);

            if (speed <= 0f)
            {
                var te = targetYaw.eulerAngles;
                rig.rotation = Quaternion.Euler(0f, te.y, 0f);
                Debug.Log($"[Task {task}] Snapped rig to face {target.name}");
                Destroy(gameObject); yield break;
            }

            while (true)
            {
                var te = targetYaw.eulerAngles;
                Quaternion desiredYaw = Quaternion.Euler(0f, te.y, 0f);
                rig.rotation = Quaternion.RotateTowards(rig.rotation, desiredYaw, speed * Time.deltaTime);
                if (Quaternion.Angle(rig.rotation, desiredYaw) <= 0.5f)
                {
                    rig.rotation = desiredYaw;
                    Debug.Log($"[Task {task}] Finished rotating toward {target.name}");
                    break;
                }
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}
