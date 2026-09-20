using UnityEngine;
using Oculus.Interaction; // Meta XR Interaction SDK namespace
using UnityEngine;
using UnityEngine;
using System.Collections;
using TMPro;
public class checkgrab : MonoBehaviour
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
            // These are simple Action events in the Meta XR SDK
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
        // Meta XR’s PointerEvent.Type tells us what happened
        switch (evt.Type)
        {
            case PointerEventType.Select:
                //SendMessageUpwards("OnObjectGrabbed", objectName, SendMessageOptions.DontRequireReceiver);
                break;

            case PointerEventType.Unselect:
                DisplayNext();
                break;
        }
    }
    public void DisplayNext()
    {
        if (!big) extraPan.SetActive(false);
        dialogueText.text = fullText;
        tp.SetActive(true);
        cube.SetActive(false);

    }
    
}
