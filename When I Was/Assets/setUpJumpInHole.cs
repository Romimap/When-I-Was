using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setUpJumpInHole : MonoBehaviour
{
    public Camera _camera;
    public float followSpeedCamera = 5.0f;
    private bool triggered = false;
    private void Start()
    {
        
    }

    private void Update()
    {
        if(triggered)
        _camera.GetComponent<CameraScript>().FollowSpeed = _camera.GetComponent<CameraScript>().FollowSpeed * 0.999f + followSpeedCamera * 0.001f;
    }

    private void OnTriggerEnter2D (Collider2D collision) {
        if (collision.tag.Equals("Player"))
        {   
            
            // collision.GetComponent<PlayerController>().
            triggered = true;


        }
    }
    
}
