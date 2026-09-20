using UnityEngine;

public class Appear : MonoBehaviour
{
    public CanvasGroup canvasGroup; // Assign in Inspector
    public float fadeDuration = 0.2f;

    private Coroutine currentRoutine;

    private void Start()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (currentRoutine != null) StopCoroutine(currentRoutine);
            currentRoutine = StartCoroutine(FadeCanvas(1f));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (currentRoutine != null) StopCoroutine(currentRoutine);
            currentRoutine = StartCoroutine(FadeCanvas(0f));
        }
    }

    private System.Collections.IEnumerator FadeCanvas(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        // Update interaction state
        bool active = Mathf.Approximately(targetAlpha, 1f);
        canvasGroup.interactable = active;
        canvasGroup.blocksRaycasts = active;
    }
}
