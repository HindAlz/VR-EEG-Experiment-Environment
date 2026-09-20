using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(Collider))]
public class Exit : MonoBehaviour
{
    public string playerTag = "Player";
    public UnityEvent onReached;
    public GameObject btn;
    public BoxCollider coll;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log("EM: Reached Exit");
            onReached?.Invoke();

            SceneManager.LoadScene("SEG 4");

        }
    }
}
