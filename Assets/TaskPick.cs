using UnityEngine;
using UnityEngine.SceneManagement;

public class TaskPick : MonoBehaviour
{
    public void repeat()
    {
        SceneManager.LoadScene("SEG 0.5");
    }
    public void proceed()
    {
        SceneManager.LoadScene("SEG 0.5.2");
    }
}
