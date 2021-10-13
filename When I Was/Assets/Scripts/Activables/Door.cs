using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Activatable {
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public override void Activate() {
        GetComponent<Animator>().SetBool("Opened", true);
        base.Activate();
    }

    public override void Deactivate() {
        GetComponent<Animator>().SetBool("Opened", false);
        base.Deactivate();
    }
}
