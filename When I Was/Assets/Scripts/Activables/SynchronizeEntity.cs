using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynchronizeEntity : Activatable {
    private Vector3 _previousPosition;
    private bool Moved {get{return ((_previousPosition - transform.position).sqrMagnitude > 0.1);}}
    private Vector3 __DEBUGOFFSET = new Vector3(0, 50, 0);

    void Start() {
        _previousPosition = transform.position;
    }

    // Update is called once per frame
    void Update() {
        if (FutureEntity != null && Moved) {
            FutureEntity.transform.position = transform.position;// + __DEBUGOFFSET;
            FutureEntity.transform.rotation = transform.rotation;
            _previousPosition = transform.position;
        }
    }
}
