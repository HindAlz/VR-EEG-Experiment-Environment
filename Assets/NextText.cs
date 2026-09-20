using UnityEngine;
using System.Collections;
using TMPro;

public class NextText : MonoBehaviour
{
    public TMP_Text dialogueText;   // Reference to your text UI element
    [TextArea]
    public string fullText;         // The full text you want to display
    public float typingSpeed = 0.05f; // Seconds between each letter
    public string taskname;
    public GameObject next;
    public GameObject prev;
    public GameObject nextButton;

    public void DisplayNext()
    {
        if (taskname!="") Debug.Log($"EM: {taskname}");
        StartCoroutine(ShowNextSequence());
    }

    private IEnumerator ShowNextSequence()
    {
        next.SetActive(true);
        prev.SetActive(false);
        yield return StartCoroutine(TypeText());
        yield return new WaitForSeconds(0f);
        nextButton.SetActive(true);
    }

    private IEnumerator TypeText()
    {
        dialogueText.text = ""; 
        foreach (char letter in fullText)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
