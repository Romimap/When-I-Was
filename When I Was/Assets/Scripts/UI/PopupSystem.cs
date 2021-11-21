using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupSystem : MonoBehaviour
{
    public GameObject popup;
    public Animator popupAnimator;
    public TextMeshProUGUI popupText;
    public Image image;
    
    // Start is called before the first frame update
    public void popUp(string text)
    {
        popupAnimator.SetTrigger("pop");
        popupText.GetComponent<TypeWriting>().displayText( text );

    }
    public void close()
    {
        popupAnimator.SetTrigger("close");
        popupText.GetComponent<TypeWriting>().stopDisplaying();

    }
}
