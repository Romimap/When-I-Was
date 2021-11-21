using System;
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

// attach to UI Text component (with the full text already there)

public class TypeWriting : MonoBehaviour 
{

    TextMeshProUGUI txt;
    string story;

    public void Awake()
    {
        txt = GetComponent<TextMeshProUGUI>();
    }

    public void displayText( string story )
    {
        StartCoroutine ("PlayText", story);
    }

    public void stopDisplaying()
    {
        StopCoroutine( "PlayText" );
    }
    IEnumerator PlayText( string story)
    {        
        txt.text = "";
        foreach (char c in story) 
        {
            txt.text += c;
            yield return new WaitForSeconds (0.02f);
        }
    }

}