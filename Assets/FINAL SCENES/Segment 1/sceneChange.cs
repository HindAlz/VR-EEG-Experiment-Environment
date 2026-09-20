using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class sceneChange : MonoBehaviour
{
    public string nextSceneName = "NextScene";
    public float delay = 120f;

    void Start()
    {
        Debug.Log("EM: SEG 1 START");
        StartCoroutine(ChangeSceneAfterDelay());
    }

    IEnumerator ChangeSceneAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("EM: SEG 1 END");
        SceneManager.LoadScene(nextSceneName);
    }
}
