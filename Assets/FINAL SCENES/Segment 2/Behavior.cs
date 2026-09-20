using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Behavior : MonoBehaviour
{
    [Header("Audio")]
    [Tooltip("Audio that plays first.")]
    public AudioClip firstAudio;

    [Tooltip("Optional override. If > 0, this duration is used instead of firstAudio.length.")]
    public float firstAudioDurationOverride = 0f;

    [Tooltip("Audio that plays once the first audio ends and when re-triggered every 5s.")]
    public AudioClip secondAudio;

    [Header("UI")]
    [Tooltip("Button that appears after the first audio ends.")]
    public Button proceedButton;

    [Header("Flow")]
    [Tooltip("Scene to load when the button is clicked.")]
    public string nextSceneName = "NextScene";

    [Tooltip("Seconds after button shows before re-playing A2 if not clicked.")]
    public float secondsBeforeReplay = 5f;

    private AudioSource _audioSource;
    private bool _proceeded = false;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        if (proceedButton != null)
        {
            proceedButton.gameObject.SetActive(false); // Hide at start
            proceedButton.onClick.AddListener(OnProceedClicked);
        }
        else
        {
            Debug.LogWarning("[AudioThenRepeatAndButton] Proceed Button is not assigned.");
        }
    }

    void Start()
    {
        Debug.Log("EM: SEG 2 START");

        StartCoroutine(RunSequence());
    }

    private IEnumerator RunSequence()
    {
        // --- 1) Play first audio (A) ---

        if (firstAudio != null)
        {
            _audioSource.Stop();
            _audioSource.clip = firstAudio;
            Debug.Log("EM: Playing audio 1");
            _audioSource.Play();

            float waitTime = (firstAudioDurationOverride > 0f)
                ? firstAudioDurationOverride
                : firstAudio.length;

            yield return new WaitForSeconds(waitTime);
        }
        else
        {
            Debug.LogWarning("[AudioThenRepeatAndButton] First audio clip is not assigned. Skipping to the next step.");
        }

        // --- 2) Play A2 and show button ---
        Debug.Log("EM: audio 2 playing");

        PlaySecondAudio();

        if (proceedButton != null)
            proceedButton.gameObject.SetActive(true);

        // --- 3) Every 'secondsBeforeReplay' seconds, replay A2 if not clicked ---
        while (!_proceeded)
        {
            yield return new WaitForSeconds(secondsBeforeReplay);
            if (_proceeded) break;
            
            Debug.Log("EM: audio 2 playing again");
            PlaySecondAudio(); // re-trigger A2
        }
    }

    private void PlaySecondAudio()
    {
        if (secondAudio == null)
        {
            Debug.LogWarning("[AudioThenRepeatAndButton] Second audio clip is not assigned.");
            return;
        }

        // Restart A2 from the beginning (even if it was already playing)
        _audioSource.Stop();
        _audioSource.clip = secondAudio;
        _audioSource.Play();
    }

    private void OnProceedClicked()
    {
        if (_proceeded) return;
        _proceeded = true;

        // Optional: small UX polish—stop audio immediately when proceeding
        _audioSource.Stop();

        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("[AudioThenRepeatAndButton] nextSceneName is empty. Assign a scene name in the Inspector.");
            return;
        }
        Debug.Log("EM: Clicked enter");
        SceneManager.LoadScene(nextSceneName);
    }

    void OnDestroy()
    {
        if (proceedButton != null)
            proceedButton.onClick.RemoveListener(OnProceedClicked);
    }
}
