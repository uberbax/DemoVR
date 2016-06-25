using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDController : MonoBehaviour {

    public GameObject message;
    public Text showText;
    public Usable currentSelected;

    public void Show(string message, Usable sender)
    {
        currentSelected = sender;
        this.message.SetActive(true);
        showText.text = message;
    }

    public void Hide(Usable sender)
    {
        if (currentSelected != sender)
        {
            return;
        }

        this.message.SetActive(false);
        showText.text = "";
    }

}
