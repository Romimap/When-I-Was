using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUp : MonoBehaviour  {
    public bool doUpgrade = true;
    private bool upgraded = false;
    [TextArea]
    public string powerUpText;
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player"))
        {
            PopupSystem pop = GameObject.FindGameObjectWithTag("Popup").GetComponent<PopupSystem>();
            pop.popUp(powerUpText);

            if (doUpgrade && !upgraded){
                other.GetComponent<PlayerController>().powerUp();
                upgraded = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            PopupSystem pop = GameObject.FindGameObjectWithTag("Popup").GetComponent<PopupSystem>();
            pop.close();
        }
    }
}
