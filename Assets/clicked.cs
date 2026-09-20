using UnityEngine;

public class clicked : MonoBehaviour
{
    public int clickednum=0;
    public GameObject door;
    public int time;
    public CheckPassing c;
    public GameObject win;
    public GameObject lose;
    
    public void addClick()
    {
        Debug.Log("added");
        clickednum++;
        if (clickednum == 6)
        {
            displayDoor();
        }
    }
    public void displayDoor()
    {
        time = c.timePassed;
        door.SetActive(true);

        if (time < 60)
        {
            win.SetActive(true);
        }
        else
        {
            lose.SetActive(true);

        }
    }
}
