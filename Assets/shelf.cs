using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class shelf : MonoBehaviour
{
    [Tooltip("Human-friendly label for debugging (e.g., 'Shelf 1 - Red Only')")]
    public string shelfLabel = "Shelf";

    // Tubes currently inside this shelf’s trigger
    private readonly HashSet<paint> contents = new HashSet<paint>();
    public IReadOnlyCollection<paint> Contents => contents;

    void Reset()
    {
        var c = GetComponent<Collider>();
        c.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (TryGetPaint(other, out var tube)){
            contents.Add(tube);
            Debug.Log($"EM: {other} added to shelf {shelfLabel}");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (TryGetPaint(other, out var tube))
            contents.Remove(tube);
    }

    // Be robust to colliders on children or rigidbodies on parents
    static bool TryGetPaint(Collider col, out paint p)
    {
        // Same GameObject
        if (col.TryGetComponent(out p)) return true;

        // If collider is on a child, tube script likely on parent
        p = col.GetComponentInParent<paint>();
        if (p) return true;

        // If collider uses an attached rigidbody, check that root too
        var rb = col.attachedRigidbody;
        if (rb && rb.TryGetComponent(out p)) return true;
        if (rb && (p = rb.GetComponentInParent<paint>())) return true;

        p = null;
        return false;
    }

    // Visualize the trigger in the editor
    void OnDrawGizmosSelected()
    {
        var c = GetComponent<Collider>();
        if (c is BoxCollider b)
        {
            Gizmos.color = Color.cyan;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(b.center, b.size);
        }
    }
}
