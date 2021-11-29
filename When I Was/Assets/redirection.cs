
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class redirection : MonoBehaviour
{
    public Camera _camera;
 
    
    private void OnTriggerEnter2D (Collider2D collision) {
        if (collision.tag.Equals("Player"))
        {
            collision.transform.position = new Vector3(collision.transform.position.x, collision.transform.position.y +400.0f,0.0f);
            _camera.transform.position = new Vector3(_camera.transform.position .x, _camera.transform.position .y +400.0f,_camera.transform.position .z);
        }
    }

}
