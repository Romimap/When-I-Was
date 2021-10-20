using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : Activatable {
    public Activatable link;
    private bool _canBeActivated = false;

    void Start() {
        Deactivate();
    }

    public override void Activate () {
        GetComponent<Animator>().SetBool("Activated", true);
        link.Activate();
        base.Activate();
    }

    public override void Deactivate () {
        GetComponent<Animator>().SetBool("Activated", false);
        link.Deactivate();
        base.Deactivate();
    }

    private void OnTriggerEnter2D (Collider2D c) {
        print(c.name + " enter");
        if (c.tag.Equals("Player")) {
            _canBeActivated = true;
        }
    }

    private void OnTriggerExit2D (Collider2D c) {
        print(c.name + " exit");
        if (c.tag.Equals("Player")) {
            _canBeActivated = false;
        }
    }

    void Update() {
        if (_canBeActivated && Input.GetKeyDown(KeyCode.E)) {
            Toggle();
        }
    }
}
