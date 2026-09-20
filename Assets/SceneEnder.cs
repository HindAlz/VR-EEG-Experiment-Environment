using UnityEngine;
using System.Collections;

public class VRURPFadeAndQuit : MonoBehaviour
{
    public CanvasGroup fadeCanvas;    // the CanvasGroup on your Screen Space–Camera canvas
    public float fadeDuration = 1.5f;
    public float holdOnBlack = 0.5f;

    bool _running;

    public void FadeInAndQuit()
    {
        if (!_running) StartCoroutine(FadeThenQuit());
    }

    IEnumerator FadeThenQuit()
    {
        if (!fadeCanvas)
        {
            Debug.LogError("Assign CanvasGroup (Screen Space–Camera)!");
            yield break;
        }

        _running = true;
        fadeCanvas.gameObject.SetActive(true);
        fadeCanvas.alpha = 0f;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }
        fadeCanvas.alpha = 1f;

        yield return new WaitForSeconds(holdOnBlack);

        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
