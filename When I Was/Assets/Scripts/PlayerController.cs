using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PhysicsObject {

    public float jumpInitSpeed = 7;
    public float maxSpeed = 7;
    public GameObject spawnPoint;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Danger"))
        {
            Death();
        }
    }


    protected void Death()
    {
        
        this.transform.position = spawnPoint.transform.position;

    }
}
