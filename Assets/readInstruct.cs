using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class readInstruct : MonoBehaviour
{
    public GameObject helloBtn;
    public GameObject instructions;
    public GameObject panel;
    public List<string> paintingsRead = new List<string>();
    public List<string> sculpturesRead = new List<string>();
    public GameObject HomePage;
    public GameObject paintingsPage;
    public GameObject sculpturesPage;
    public GameObject notAllPaintReadWarning;
    public GameObject notAllSculpWarning;
    public GameObject selectWarn;
    public int numDone = 0;

    public GameObject ty;
    public GameObject review;
    public void close()
    {
        panel.SetActive(false);
        HomePage.SetActive(true);
    }
    
    public void OpenPaintings()
    {
        bool allPaintingsRead =
            paintingsRead.Contains("1") &&
            paintingsRead.Contains("2") &&
            paintingsRead.Contains("3");

        if (allPaintingsRead)
        {
            Debug.Log("EM: Opened paintings review page");

            if (HomePage) HomePage.SetActive(false);
            if (paintingsPage) paintingsPage.SetActive(true);
        }
        else
        {
            if (notAllPaintReadWarning) notAllPaintReadWarning.SetActive(true);
        }
    }
    public void OpenSculptures()
    {
        bool allSculpturesRead =
            sculpturesRead.Contains("1") &&
            sculpturesRead.Contains("2") &&
            sculpturesRead.Contains("3");

        if (allSculpturesRead)
        {
            Debug.Log("EM: Opened sculptures review page");

            if (HomePage) HomePage.SetActive(false);
            if (paintingsPage) sculpturesPage.SetActive(true);
        }
        else
        {
            if (notAllSculpWarning) notAllSculpWarning.SetActive(true);
        }
    }
    public void addPainting(string no){
        paintingsRead.Add(no);
    }
    public void addSculpture(string no)
    {
        sculpturesRead.Add(no);
    }
    
    public void closePWarn()
    {
        notAllPaintReadWarning.SetActive(false);
    }public void closeSWarn()
    {
        notAllSculpWarning.SetActive(false);
    }
    public void selectWarnClose()
    {
        selectWarn.SetActive(false);
    }
    public void AddDone()
    {
        if (numDone == 0)
        {
            numDone = 1;
        }
        else if (numDone == 1)
        {

            ty.SetActive(true);
            Debug.Log("EM: Segment done");
            review.SetActive(false);
            SceneManager.LoadScene("SEG 5");

        }
    }
    public void StartSeg()
    {
        helloBtn.SetActive(false);
        instructions.SetActive(true);
        Debug.Log("EM: Opened Instructions");
    }
}
