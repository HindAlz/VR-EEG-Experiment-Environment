using UnityEngine;

public class snapbehavior : MonoBehaviour
{
    [Tooltip("Assign ALL five PaintingSnapMeta components here (drag from scene).")]
    public snap[] allPaintings = new snap[5];

    [Header("Choose exactly TWO that should snap")]
    public snap snappableA;
    public snap snappableB;

    [ContextMenu("Apply Selection")]
    public void ApplySelection()
    {
        if (allPaintings == null) return;

        foreach (var p in allPaintings)
        {
            if (p == null) continue;
            bool isChosen = (p == snappableA) || (p == snappableB);
            p.canSnap = isChosen;
        }
    }

    // Optionally keep enforcing at runtime
    void LateUpdate()
    {
        ApplySelection();
    }
}
