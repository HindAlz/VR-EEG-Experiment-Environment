using UnityEngine;

public class ReadText : MonoBehaviour
{
    public GameObject ReadButtonPage;
    public GameObject TextPage;
    public string type;
    public string number;
    public readInstruct listDone;


    public void displayText(){
        ReadButtonPage.SetActive(false);
        TextPage.SetActive(true);
        Debug.Log($"EM: Read {type} no. {number}");

    }
    public void LogRead(){
        Debug.Log($"EM: Closed {type} no. {number}");

        TextPage.SetActive(false);
        if (type=="painting"){
            listDone.addPainting(number);
        } else if (type=="sculpture") listDone.addSculpture(number);
    }
}
