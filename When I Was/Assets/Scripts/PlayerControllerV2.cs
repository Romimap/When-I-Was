using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class PlayerControllerV2 : MonoBehaviour
{
    //params
        //seuilSol/gravite
    public float minGroundNormY = .65f;
    public float gravityMod = 1f;
        //movement
    public float jumpInitSpeed = 7;
    public float maxSpeed = 7;
    public float jumpHandleWindow = 1;
        //respawn
    public GameObject spawnPoint;

    //etat
    protected STATE playerState = STATE.Grounded;

    //movement joueur
    protected Vector2 targetVelocity;

    protected Vector2 groundNormal;
    protected Vector2 velocity;
    protected Rigidbody2D body;

    //collisions
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuff = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBuffList = new List<RaycastHit2D>(16);

    protected const float minMoveDist = 0.001f;
    protected const float shellRadius = 0.01f;

    //timers
    float releaseTime;
    float deathTimer;


    //other
    private SpriteRenderer spriteRenderer;


    //Inits
    private void OnEnable()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    // Update is called once per frame
    void Update()
    {
        Vector2 move = Vector2.zero;

        switch (playerState)
        {
            

            case (STATE.Grounded):
                //deplacement horizontal
                move.x = Input.GetAxis("Horizontal");

                if (Input.GetButtonDown("Jump"))
                {
                    releaseTime = 0;
                    velocity.y = jumpInitSpeed;
                    playerState = STATE.Jumping;
                }

                break;

            case (STATE.Jumping):
                releaseTime += Time.deltaTime;

                //deplacement horizontal
                move.x = Input.GetAxis("Horizontal");

                if (Input.GetButtonUp("Jump") && releaseTime < jumpHandleWindow)
                {
                    if(velocity.y > 0)
                        velocity.y = velocity.y * (releaseTime / jumpHandleWindow);
                }

                break;

            case (STATE.Switching):
                //voir avec Charles
                break;

            case (STATE.Dead):
                //spriteRenderer.enabled = false;
                deathTimer += Time.deltaTime;


                if(deathTimer > 5.0f)
                {
                    if (this.transform.position != spawnPoint.transform.position)
                    {
                        this.transform.position = Vector2.MoveTowards(this.transform.position, spawnPoint.transform.position, 1 * Time.deltaTime);
                    }
                    else
                    {
                        playerState = STATE.Jumping;
                        //spriteRenderer.enabled = true;
                    }
                }

                break;

            default:
                Debug.Log("Le perso ne devrait pas passer dans default\n");
                break;
        }

        targetVelocity = move * maxSpeed;

    }




    //Movement
    private void FixedUpdate()
    {
        velocity += gravityMod * Physics2D.gravity * Time.deltaTime;
        velocity.x = targetVelocity.x;

        Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);
        Vector2 deltaPos = velocity * Time.deltaTime;

        //horizontal Movement
        Vector2 move = moveAlongGround * deltaPos.x;
        Movement(move, false);


        //gravity Movement
        move = Vector2.up * deltaPos.y;
        Movement(move, true);
    }

    void Movement(Vector2 move, bool yMove)
    {
        float distance = move.magnitude;

        if (distance > minMoveDist)
        {
            int count = body.Cast(move, contactFilter, hitBuff, distance + shellRadius);
            hitBuffList.Clear();
            for (int i = 0; i < count; i++)
            {
                hitBuffList.Add(hitBuff[i]);
            }

            foreach (RaycastHit2D hit in hitBuffList)
            {
                Vector2 currentNormal = hit.normal;

                if (currentNormal.y > minGroundNormY)
                {
                    playerState = STATE.Grounded;
                    if (yMove)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }
                else
                    playerState = STATE.Jumping;

                float projection = Vector2.Dot(velocity, currentNormal);
                if (projection < 0)
                {
                    velocity = velocity - projection * currentNormal;
                }

                float modifiedDist = hit.distance - shellRadius;
                distance = modifiedDist < distance ? modifiedDist : distance;
            }
        }

        body.position += move.normalized * distance;
    }





    //Collisions
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Danger"))
        {
            this.playerState = STATE.Dead;
            deathTimer = 0;
        }
    }
}
