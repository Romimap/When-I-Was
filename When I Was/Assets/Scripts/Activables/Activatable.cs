using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activatable : MonoBehaviour {
    public Activatable FutureEntity = null;
    public Activatable PastEntity = null;

    protected bool _activated = false;

    public virtual void Activate () {
        _activated = true;
        if (FutureEntity != null) {
            FutureEntity.Activate();
        }
    }

    public virtual void Deactivate () {
        _activated = false;
        if (FutureEntity != null) {
            FutureEntity.Deactivate();
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
