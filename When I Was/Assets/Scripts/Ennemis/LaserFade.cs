using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserFade : MonoBehaviour
{
    public float velocity = 2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.Translate(Vector3.right * Time.deltaTime * velocity);
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Danger"))
            Destroy(this.gameObject);
    }
}
