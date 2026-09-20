using UnityEngine;
using UnityEngine;
using System.Collections;
using TMPro;

public class CollisionCameraSnap : MonoBehaviour
{
    [Header("References")]
    public GameObject btn;
    public GameObject arrow;
    public Transform avatar;          // The target location to snap the camera to
    public AudioSource audioSource;   // The AudioSource component to play sounds
    public AudioClip wrongBuzzerClip; // The sound to play when it's the wrong object
    public AudioClip rightBuzzerClip; // The sound to play when it's the wrong object
    public Camera mainCamera;         // The camera to move (optional if not main)
    public TMP_Text dialogueText;
    [TextArea]
    public string fullText; 
    public float typingSpeed = 0.05f;
    private void Reset()
    {
        mainCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.ToLower().Contains("sphere"))
        {
            audioSource.PlayOneShot(rightBuzzerClip);
            Debug.Log("EM: placed sphere");
            arrow.SetActive(false);
            SnapCameraToAvatar();
        }
        else
        {
            PlayWrongBuzzer();
        }
    }

    void SnapCameraToAvatar()
    {
        if (mainCamera != null && avatar != null)
        {
            mainCamera.transform.position = avatar.position;
            mainCamera.transform.rotation = avatar.rotation;
            btn.SetActive(true);

        }
        else
        {
            Debug.LogWarning("Missing camera or avatar reference!");
        }
    }

    private void PlayWrongBuzzer()
    {
        if (audioSource != null && wrongBuzzerClip != null)
        {
            audioSource.PlayOneShot(wrongBuzzerClip);
            Debug.Log("Played wrong buzzer sound.");
        }
        else
        {
            Debug.LogWarning("Missing AudioSource or buzzer clip!");
        }
    }

     public void DisplayNext()
    {
        Debug.Log("EM: Task 2 reading");
        StartCoroutine(TypeText());
        btn.SetActive(false);
    }
     public IEnumerator TypeText()
    {
        dialogueText.text = ""; 
        foreach (char letter in fullText)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
