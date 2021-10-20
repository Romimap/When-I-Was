using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Activatable {
    private bool _preventClosing = false;
    private bool _wantsToClose = false;
    // Start is called before the first frame update
    void Start() {
        Deactivate();
    }

    // Update is called once per frame
    void Update() {
        if (_wantsToClose && !_preventClosing) {
            Deactivate();
        }
    }

    public override void Activate() {
        GetComponent<Animator>().SetBool("Opened", true);
        base.Activate();
    }

    public override void Deactivate() {
        if (_preventClosing) {
            _wantsToClose = true;
            print("_wantsToClose " + _wantsToClose);
            return;
        }
        GetComponent<Animator>().SetBool("Opened", false);
        base.Deactivate();
        _wantsToClose = false;
        print("_wantsToClose " + _wantsToClose);
    }

    private void OnTriggerEnter2D (Collider2D c) {
        print(c.name + " enter");
        if (c.tag.Equals("Player")) {
            _preventClosing = true;
            _wantsToClose = !_activated;
            Activate();
        }
    }

    private void OnTriggerExit2D (Collider2D c) {
        print(c.name + " exit");
        if (c.tag.Equals("Player")) {
            _preventClosing = false;
        }
    }
}
