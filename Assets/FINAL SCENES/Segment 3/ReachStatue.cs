using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ReachZone : MonoBehaviour
{
    public string playerTag = "Player";
    public UnityEvent onReached;
    public GameObject btn;
    public BoxCollider coll;
    public int taskNumber = 1; 
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log($"EM: Reached statue {taskNumber}");
            onReached?.Invoke();

            if (btn != null)
                btn.SetActive(true);

            if (coll != null)
                coll.enabled = false; 
        }
    }
}
