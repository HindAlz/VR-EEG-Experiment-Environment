using UnityEngine;
using System.Collections;


public class CheckPassing : MonoBehaviour
{
    public int timePassed;

    public void startTimer()
    {
        StartCoroutine(Timer());
    }
    IEnumerator Timer()
    {
        while (timePassed<=60){
            Debug.Log("time: " + timePassed);
            timePassed++;
            yield return new WaitForSeconds(1f);
        }
    }


}
