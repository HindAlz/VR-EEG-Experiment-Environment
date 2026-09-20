using UnityEngine;

public class followCamera : MonoBehaviour
{
    public Transform cameraTransform;   // Assign XR Camera
    public float followSpeed = 5f;      // Higher = snappier
    public float rotationSpeed = 5f;    // Higher = snappier
    public Vector3 offset = new Vector3(0, 0, 2f); // In front of camera

    void Update()
    {
        // Desired position in front of camera
        Vector3 targetPos = cameraTransform.position + cameraTransform.forward * offset.z
                            + cameraTransform.up * offset.y
                            + cameraTransform.right * offset.x;

        // Smooth position
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);

        // Smooth rotation (only Yaw, optional)
        Quaternion targetRot = Quaternion.LookRotation(cameraTransform.forward, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
    }
}
