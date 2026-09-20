using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ShowTextAfterDelay : MonoBehaviour
{
    public TextMeshProUGUI dispText;   
    public float delay = 10f;

    void Start()
    {
        dispText.gameObject.SetActive(false);
        StartCoroutine(ShowText());
    }

    IEnumerator ShowText()
    {
        yield return new WaitForSeconds(delay);
        dispText.gameObject.SetActive(true);
    }
}
