using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class manager : MonoBehaviour
{
    public GameObject btn;
    public GameObject btn2;

    [Header("Shelf Zones (drag from scene)")]
    public shelf redShelf;
    public shelf blueShelf;
    public shelf greenShelf;
    public shelf otherShelf;

    [Header("Expected counts")]
    public int expectedReds = 3;
    public int expectedBlues = 3;
    public int expectedGreens = 1;
    public int expectedOthers = 2;

    [Header("Dialogue")]
    public TMP_Text dialogueText;
    [TextArea] public string fullText;

    [Header("Audio Feedback")]
    [Tooltip("Audio source used for playback (must not be null if you want sound).")]
    public AudioSource audioSource;
    [Tooltip("Clip played once when puzzle is solved.")]
    public AudioClip solvedClip;

    [Header("Events")]
    public bool logProgress = true;
    public UnityEngine.Events.UnityEvent onPuzzleSolved;

    private paint[] allTubes;
    private bool puzzleSolved = false;

    void Start()
    {
        allTubes = FindObjectsOfType<paint>(includeInactive: false);

        // Optional safety: hide buttons at start if assigned
        if (btn)  btn.SetActive(false);
        if (btn2) btn2.SetActive(false);
    }

    void Update()
    {
        if (puzzleSolved) return;

        var state = Evaluate();
        if (logProgress) DebugDrawState(state);

        if (state.IsSolved)
        {
            Debug.Log("EM: task 3 done");
            puzzleSolved = true;

            if (btn) btn.SetActive(true);

            if (audioSource && solvedClip)
                audioSource.PlayOneShot(solvedClip);

            onPuzzleSolved?.Invoke();

            enabled = false; // stop further checks
        }
    }

    public PuzzleState Evaluate()
    {
        var problems = new List<string>();

        if (!redShelf || !blueShelf || !greenShelf || !otherShelf)
        {
            problems.Add("One or more shelf references are missing.");
            return PuzzleState.Fail(problems);
        }

        var redsOnRed   = CountOf(redShelf, TubeKind.Red);
        var bluesOnBlue = CountOf(blueShelf, TubeKind.Blue);
        var greensOnGreen = CountOf(greenShelf, TubeKind.Green);
        var othersOnOther = CountOf(otherShelf, TubeKind.Other);

        if (HasAnyNotKind(redShelf, TubeKind.Red))     problems.Add("Red shelf contains non-red tube(s).");
        if (HasAnyNotKind(blueShelf, TubeKind.Blue))   problems.Add("Blue shelf contains non-blue tube(s).");
        if (HasAnyNotKind(greenShelf, TubeKind.Green)) problems.Add("Green shelf contains non-green tube(s).");
        if (HasAnyNotKind(otherShelf, TubeKind.Other)) problems.Add("Other shelf contains RGB tube(s).");

        if (redsOnRed   != expectedReds)   problems.Add($"Red shelf count = {redsOnRed} (expected {expectedReds}).");
        if (bluesOnBlue != expectedBlues)  problems.Add($"Blue shelf count = {bluesOnBlue} (expected {expectedBlues}).");
        if (greensOnGreen != expectedGreens) problems.Add($"Green shelf count = {greensOnGreen} (expected {expectedGreens}).");
        if (othersOnOther != expectedOthers) problems.Add($"Other shelf count = {othersOnOther} (expected {expectedOthers}).");

        var allInShelves = new HashSet<paint>(
            redShelf.Contents.Concat(blueShelf.Contents)
                             .Concat(greenShelf.Contents)
                             .Concat(otherShelf.Contents)
        );

        var missing = allTubes.Where(t => !allInShelves.Contains(t)).ToList();
        if (missing.Count > 0) problems.Add($"{missing.Count} tube(s) not on a shelf.");

        return problems.Count == 0 ? PuzzleState.Success() : PuzzleState.Fail(problems);
    }

    int CountOf(shelf s, TubeKind kind) => s.Contents.Count(t => t && t.kind == kind);
    bool HasAnyNotKind(shelf s, TubeKind allowed) => s.Contents.Any(t => t && t.kind != allowed);

    void DebugDrawState(PuzzleState state)
    {
        if (Time.frameCount % 60 != 0) return;

        if (state.IsSolved) Debug.Log("<color=green>[PaintSorting]</color> ✅ Puzzle solved!");
        else foreach (var p in state.Problems) Debug.LogWarning($"[PaintSorting] {p}");
    }

    public struct PuzzleState
    {
        public bool IsSolved;
        public List<string> Problems;

        public static PuzzleState Success() => new PuzzleState { IsSolved = true, Problems = new List<string>() };
        public static PuzzleState Fail(List<string> issues) => new PuzzleState { IsSolved = false, Problems = issues };
    }

    // UI actions
    public void DisplayNext()
    {
        Debug.Log("EM: finished task 3");
        if (btn) btn.SetActive(false);
        StartCoroutine(TypeText());
        if (btn2) btn2.SetActive(true);
    }

    public IEnumerator TypeText()
    {
        if (!dialogueText) yield break;

        dialogueText.text = "";
        foreach (char letter in fullText)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
