using UnityEngine;
using System.Collections;
using TMPro;

public class BothSnappedAudio : MonoBehaviour
{
    public GameObject task3;

    [Header("References")]
    public snap paintingA;
    public snap paintingB;
    public AudioSource audioSource;

    [Header("Behavior")]
    [Tooltip("Play the sound only once ever, even if paintings are moved again.")]
    public bool playOnlyOnce = true;

    public GameObject btn;

    bool _hasPlayed; // tracks if we've already played it once (only used if playOnlyOnce is true)

    [Header("Dialogue")]
    public TMP_Text dialogueText;
    [TextArea] public string fullText;
    public float typingSpeed = 0.05f;

    Coroutine typingRoutine;

    void OnEnable()
    {
        if (paintingA != null) paintingA.onSnappedChanged.AddListener(OnSnappedChanged);
        if (paintingB != null) paintingB.onSnappedChanged.AddListener(OnSnappedChanged);
    }

    void OnDisable()
    {
        if (paintingA != null) paintingA.onSnappedChanged.RemoveListener(OnSnappedChanged);
        if (paintingB != null) paintingB.onSnappedChanged.RemoveListener(OnSnappedChanged);
    }

    void OnSnappedChanged(bool _)
    {
        if (paintingA == null || paintingB == null) return;

        // Trigger only when both are snapped
        if (paintingA.IsSnapped && paintingB.IsSnapped)
        {
            if (audioSource != null && (!playOnlyOnce || !_hasPlayed))
            {
                audioSource.Play();
                if (btn != null) btn.SetActive(true);
                if (task3 != null) task3.SetActive(true);

                if (playOnlyOnce) _hasPlayed = true;
            }
        }
    }

    public void DisplayNext()
    {
        Debug.Log("EM: reading Task 3");

        if (typingRoutine != null) StopCoroutine(typingRoutine);
        typingRoutine = StartCoroutine(TypeTextTMP());

        if (btn != null) btn.SetActive(false);
    }

    // TMP-friendly typewriter that never shows rich-text tags mid-typing
    IEnumerator TypeTextTMP()
    {
        if (dialogueText == null) yield break;

        dialogueText.richText = true;
        dialogueText.text = fullText;

        // Ensure TMP builds textInfo before we query it
        dialogueText.ForceMeshUpdate();

        int totalChars = dialogueText.textInfo.characterCount;
        dialogueText.maxVisibleCharacters = 0;

        while (dialogueText.maxVisibleCharacters < totalChars)
        {
            dialogueText.maxVisibleCharacters++;
            yield return new WaitForSeconds(typingSpeed);
        }

        typingRoutine = null;
    }
}
