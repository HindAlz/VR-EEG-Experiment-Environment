using UnityEngine;
using UnityEngine;
using System.Collections;
using TMPro;

public class tut : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject btn;
    public TMP_Text dialogueText;
    [TextArea]
    public string fullText; 
    public string fullText2;
    public float typingSpeed = 0.05f;
    public GameObject pickupTut;
    void Start()
    {
        DisplayNext();
    }

    public void DisplayNext()
    {
        StartCoroutine(TypeText());
        btn.SetActive(true);

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

    public void clickMe()
    {
        StartCoroutine(TypeText2());
        btn.SetActive(false);
        pickupTut.SetActive(true);


    }
    public IEnumerator TypeText2()
    {
        dialogueText.text = "";
        foreach (char letter in fullText2)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
