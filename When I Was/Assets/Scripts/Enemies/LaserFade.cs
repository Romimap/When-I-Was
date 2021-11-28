using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserFade : MonoBehaviour
{
    public float velocity = 2;

    private float timer = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        
        this.gameObject.transform.Translate(Vector3.right * Time.deltaTime * velocity);
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Danger") && timer > 0.5f)
            Destroy(this.gameObject);
    }
}
