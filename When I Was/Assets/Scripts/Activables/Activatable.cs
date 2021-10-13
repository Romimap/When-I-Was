using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activatable : MonoBehaviour {
    public Activatable FutureActivatable = null;
    public Activatable PastActivatable = null;

    private bool _activated = false;

    public virtual void Activate () {
        _activated = true;
        if (FutureActivatable != null) {
            FutureActivatable.Activate();
        }
    }

    public virtual void Deactivate () {
        _activated = false;
        if (FutureActivatable != null) {
            FutureActivatable.Deactivate();
        }
    }

    public virtual void Toggle () {
        if (_activated) Deactivate();
        else Activate();
    }

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }
}
