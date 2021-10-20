using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : Activatable {
    private Vector3 start;
    private Vector3 end;
    private float timer = 0;
    private float speed = 1;
    private float direction = -1;

    void Start() {
        start = transform.position;
        end = transform.Find("End Position").position;
    }

    void Update() {
        timer += Time.deltaTime * speed * direction;
        timer = Mathf.Clamp(timer, 0, 1);

        float t = easeInOutQuad(timer);
        transform.position = end * t + start * (1 - t);
    }

    float easeInOutQuad(float x) {
        return x < 0.5 ? 2 * x * x : 1 - Mathf.Pow(-2 * x + 2, 2) / 2;
    }

    public override void Activate() {
        direction = 1;
        base.Activate();
    }

    public override void Deactivate() {
        direction = -1;
        base.Deactivate();
    }
}
