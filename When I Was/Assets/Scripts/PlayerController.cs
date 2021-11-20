using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PhysicsObject
{

    public GameObject past;
    public GameObject present;
    public GameObject persistent;

    private enum State {
        Past, Present
    };
    private State gabbyState = State.Present;

    public float circleRadius = 0.5f;
    
    private List<GameObject> pastToHide = new List<GameObject>();    //objects to hide when we switch to present
    private List<GameObject> presentToHide = new List<GameObject>(); // same but vice-versa

    public float jumpInitSpeed = 7;
    public float maxSpeed = 7;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        initializeGO();
    }

    void initializeGO()
    {
        foreach (Transform child in past.transform)
        {
            pastToHide.Add(child.gameObject);
        }
        
        foreach (Transform child in present.transform)
        {
            presentToHide.Add(child.gameObject);
        }
        showGameObjects( presentToHide );
        hideGameObjects( pastToHide );
    }
    
    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;

        //d�placement horizontal
        move.x = Input.GetAxis("Horizontal");

        if(Input.GetButtonDown("Jump") && grounded)   //saut possible que si au sol (rajouter condition si on veut wall jump)
        {
            velocity.y = jumpInitSpeed;
        }else if (Input.GetButtonUp("Jump"))          //stop le saut mid air (== si appui long grand saut, sinon petit saut)
        {
            if(velocity.y > 0)
            {
                velocity.y = velocity.y * 0.5f;
            }
        }
        bool flipSprite = spriteRenderer.flipX ? move.x > 0.01f : move.x < 0.01f;

        if (flipSprite)
            spriteRenderer.flipX = !spriteRenderer.flipX;

        targetVelocity = move * maxSpeed;
    }

    void activateGameObjects(List<GameObject> list)
    {
        foreach ( var go in list)
            go.SetActive(true);
    }

    void deactivateGameObjects(List<GameObject> list)
    {
        foreach ( var go in list)
            go.SetActive(false);
    }

    void showGameObjects(List<GameObject> list)
    {
        foreach ( var go in list )
            go.GetComponent<Renderer>().enabled = true;
    }
    
    void hideGameObjects(List<GameObject> list)
    {
        foreach ( var go in list)
            go.GetComponent<Renderer>().enabled = false;
    }

    bool onlyCollidesWithPersistent(List<Collider2D> colliders)
    {
        foreach( Collider2D col in colliders )
            if (col.gameObject.layer != persistentLayer)
                return false;
        return true;
    }
    
    void Update()
    {

        targetVelocity = Vector2.zero;
        ComputeVelocity();

        if (Input.GetKeyDown(KeyCode.T))
        {
            List<Collider2D> collisionsPast = new List<Collider2D>();
            List<Collider2D> collisionsPresent = new List<Collider2D>();
            
            Physics2D.OverlapCircle(transform.position, circleRadius, contactFilterPast, collisionsPast);
            Physics2D.OverlapCircle(transform.position, circleRadius, contactFilterPresent, collisionsPresent);
            
            foreach( Collider2D co in collisionsPast )
                Debug.Log( co.gameObject.name  );
            
            foreach( Collider2D co in collisionsPresent )
                Debug.Log( co.gameObject.name  );
        
            if (gabbyState == State.Present && onlyCollidesWithPersistent( collisionsPast ) )
            {
                gabbyState = State.Past;
                
                gameObject.layer = pastLayer;
                contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(pastLayer));

                showGameObjects(pastToHide);
                hideGameObjects(presentToHide);

            }
            
            else if (gabbyState == State.Past && onlyCollidesWithPersistent( collisionsPresent ))
            {
                gabbyState = State.Present;

                gameObject.layer = presentLayer;
                contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(presentLayer));

                showGameObjects(presentToHide);
                hideGameObjects(pastToHide);

            }
            else
            {
                Camera mainCamera = Camera.main;
                StartCoroutine(mainCamera.GetComponent<CameraShake>().Shake(0.15f, 0.1f) );
            }
        }
    }
}
   

