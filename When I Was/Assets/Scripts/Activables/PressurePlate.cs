using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : Activatable {
    public Activatable link;
    private List<Collider2D> colliders = new List<Collider2D>();
    // Start is called before the first frame update
    void Start() {
        Deactivate();
    }

    public override void Activate () {
        GetComponent<Animator>().SetBool("Pressed", true);
        link.Activate();
        base.Activate();
    }

    public override void Deactivate () {
        GetComponent<Animator>().SetBool("Pressed", false);
        link.Deactivate();
        base.Deactivate();
    }

    private void OnTriggerEnter2D (Collider2D c) {
        if (c.tag.Equals("Player") || c.tag.Equals("Box")) {
            if (colliders.Count == 0) Activate();
            if (!colliders.Contains(c)) {
                colliders.Add(c);
            }
        }
    }

    private void OnTriggerExit2D (Collider2D c) {
        if (c.tag.Equals("Player") || c.tag.Equals("Box")) {
            if (colliders.Contains(c)) {
                colliders.Remove(c);
            }
            if (colliders.Count == 0) Deactivate();
        }
    }
}
