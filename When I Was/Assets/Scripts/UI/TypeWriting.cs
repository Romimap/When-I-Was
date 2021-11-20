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

    void Awake () 
    {
        txt = GetComponent<TextMeshProUGUI>();
        story = txt.text;

    }

    private void OnEnable()
    {
        StartCoroutine ("PlayText");
    }

    IEnumerator PlayText()
    {        
        txt.text = "";
        foreach (char c in story) 
        {
            txt.text += c;
            yield return new WaitForSeconds (0.125f);
        }
    }

}