
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class redirection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter2D (Collider2D collision) {
        if (collision.tag.Equals("Player")) {
            collision.transform.position = new Vector3(collision.transform.position.x, collision.transform.position.y +200.0f,0.0f);
        }
    }

}
