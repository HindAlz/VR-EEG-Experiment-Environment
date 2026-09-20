using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public Button button1;
    public Button button2;
    public Button button3;
    public bool paintDone;
    public GameObject HomePage;
    public GameObject paintingsPage;
    public GameObject noSelectionWarning;
    public GameObject reviewButtn;
    public readInstruct readInst;
    public string type;
    void Start()
    {
        // Assign listeners
        button1.onClick.AddListener(() => OnButtonClicked(button1));
        button2.onClick.AddListener(() => OnButtonClicked(button2));
        button3.onClick.AddListener(() => OnButtonClicked(button3));
    }

    void OnButtonClicked(Button clickedButton)
    {
        // Disable all buttons except the one clicked
        paintDone=true;
        if (clickedButton == button1)
        {
            button2.interactable = false;
            button3.interactable = false;
        }
        else if (clickedButton == button2)
        {
            button1.interactable = false;
            button3.interactable = false;
        }
        else if (clickedButton == button3)
        {
            button1.interactable = false;
            button2.interactable = false;
        }

        Debug.Log(clickedButton.name + " clicked!");
    }

    public void closePage(){
        if (paintDone){
            Debug.Log($"EM: Submit {type} review page");

            paintingsPage.SetActive(false);
            HomePage.SetActive(true);
            reviewButtn.SetActive(false);
            readInst.AddDone();

        }else{
            noSelectionWarning.SetActive(true);
        }
    }

    public void closeWarning(){
        noSelectionWarning.SetActive(false);
    }
}
