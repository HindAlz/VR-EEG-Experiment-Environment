using UnityEngine;

public class enableArrow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject arrow;
    public void enableAr()
    {
        arrow.SetActive(true);
    }
}
