using UnityEngine;
using UnityEngine.Events;
using UnityEngine;
using System.Collections;
using Oculus.Interaction; // Meta XR Interaction SDK namespace

public enum TubeKind
{
    Red,
    Blue,
    Green,
    Other
}

public class paint : MonoBehaviour
{
    [SerializeField] private Grabbable grabbable; // assign in Inspector

    [Header("Auto from name: contains 'Red', 'Blue', 'Green' (case-insensitive)")]
    public TubeKind kind = TubeKind.Other;

    void Awake()
    {
        DetectFromName();
        LogDetectedColor();
    }

    void DetectFromName()
    {
        var n = gameObject.name.ToLowerInvariant();

        if (n.Contains("red"))       kind = TubeKind.Red;
        else if (n.Contains("blue")) kind = TubeKind.Blue;
        else if (n.Contains("green"))kind = TubeKind.Green;
        else                         kind = TubeKind.Other;
    }

    void LogDetectedColor()
    {
        // Color-coded console output (works in Unity console)
        string colorTag = kind switch
        {
            TubeKind.Red => "<color=red>RED</color>",
            TubeKind.Blue => "<color=blue>BLUE</color>",
            TubeKind.Green => "<color=green>GREEN</color>",
            _ => "<color=grey>OTHER</color>"
        };

        Debug.Log($"[PaintTube] {gameObject.name} detected as {colorTag}", this);
    }
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
                Debug.Log($"EM: paint {gameObject.name} grabbed");
                //SendMessageUpwards("OnObjectGrabbed", objectName, SendMessageOptions.DontRequireReceiver);
                break;

            case PointerEventType.Unselect:
                Debug.Log($"EM: paint {gameObject.name} released");
                //SendMessageUpwards("OnObjectReleased", objectName, SendMessageOptions.DontRequireReceiver);
                break;
        }
    }
}
