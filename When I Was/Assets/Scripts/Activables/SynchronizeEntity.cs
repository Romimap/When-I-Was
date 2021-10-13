using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynchronizeEntity : Activatable {
    private Transform _previousTransform;
    private bool Moved {get{return ((_previousTransform.position - transform.position).sqrMagnitude > 1);}}
    private Vector3 __DEBUGOFFSET = new Vector3(0, 100, 0);

    void Start() {
        _previousTransform = transform;
    }

    // Update is called once per frame
    void Update() {
        if (FutureActivatable != null && Moved) {
            FutureActivatable.transform.position = transform.position + __DEBUGOFFSET;
            FutureActivatable.transform.rotation = transform.rotation;
            _previousTransform = transform;
        }
    }
}
