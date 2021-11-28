using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Collect : MonoBehaviour {
    public TextMeshProUGUI score;
    public FMODUnity.StudioEventEmitter collectedSound;

    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log(other.gameObject.name);
        GameData.SCORE += 1;
        collectedSound.Play();
        Destroy(this.gameObject);
    }
    
}
