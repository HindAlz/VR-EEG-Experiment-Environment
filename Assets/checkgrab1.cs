using System.Collections;
using UnityEngine;
using Oculus.Interaction; // Meta XR Interaction SDK namespace
using TMPro;

public class checkgrab2 : MonoBehaviour
{
    public GameObject extraPan;
    public GameObject cube;
    [SerializeField] private Grabbable grabbable; // assign in Inspector
    public TMP_Text dialogueText;
    [TextArea]
    public string fullText;
    public float typingSpeed = 0.05f;
    public GameObject tp;
    public bool big;

    private void OnEnable()
    {
        if (grabbable != null)
        {
            grabbable.WhenPointerEventRaised += OnPointerEventRaised;
        }
    }

    private void OnDisable()
    {
        if (grabbable != null)
            grabbable.WhenPointerEventRaised -= OnPointerEventRaised;
    }

    private void OnPointerEventRaised(PointerEvent evt)
    {
        switch (evt.Type)
        {
            case PointerEventType.Select:
                // object grabbed
                break;

            case PointerEventType.Unselect:
                DisplayNext();
                break;
        }
    }

    public void DisplayNext()
    {
        // Get the cube's Y rotation in degrees
        float y = cube.transform.eulerAngles.y;

        // Compute shortest signed angle from 0 to y (handles wrap-around)
        float shortest = Mathf.Abs(Mathf.DeltaAngle(0f, y));

        // If rotated more than 90 degrees on the Y axis, proceed
        if (shortest > 90f)
        {
            if (!big) extraPan.SetActive(false);
            dialogueText.text = fullText;
            tp.SetActive(true);
            cube.SetActive(false);
        }
        else
        {
            // Hint to the user (can be adjusted / replaced with UI feedback)
            dialogueText.text = "Rotate the cube more ( > 90° ) to continue.";
            // Optionally you can flash a visual hint or animate the cube to show direction
            // e.g. StartCoroutine(ShowTemporaryHint(...));
        }
    }

    // optional helper coroutine if you want the hint to disappear after a few seconds
    private IEnumerator ShowTemporaryHint(string msg, float duration = 2f)
    {
        string prev = dialogueText.text;
        dialogueText.text = msg;
        yield return new WaitForSeconds(duration);
        dialogueText.text = prev;
    }
}
