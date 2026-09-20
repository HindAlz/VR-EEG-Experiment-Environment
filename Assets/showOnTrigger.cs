using UnityEngine;
using UnityEngine.SceneManagement;

public class ShowOnTrigger : MonoBehaviour
{
    // The object you want to display
    public GameObject objectToShow;

    void Start()
    {
        // Make sure it's hidden at the start
        if (objectToShow != null)
            objectToShow.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the collider's name CONTAINS "Player"
        if (other.gameObject.name.Contains("Player"))
        {
            if (objectToShow != null)
                objectToShow.SetActive(true);
        }
    }

    public void StartSegments()
    {
        SceneManager.LoadScene("SEG 1");

    }
}
