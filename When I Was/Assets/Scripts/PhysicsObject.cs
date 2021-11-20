using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{

    public float minGroundNormY = .65f;
    public float gravityMod = 1f;

    //movement joueur
    protected Vector2 targetVelocity;

    protected bool grounded;
    protected Vector2 groundNormal;
    protected Vector2 velocity;
    protected Rigidbody2D body;

    //collisions
    
    protected int pastLayer = 7;
    protected int presentLayer = 8;
    protected int persistentLayer = 9;
    
    protected ContactFilter2D contactFilter;
    
    protected ContactFilter2D contactFilterPast;
    protected ContactFilter2D contactFilterPresent;
    
    protected RaycastHit2D[] hitBuff = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBuffList = new List<RaycastHit2D>(16);

    protected const float minMoveDist = 0.001f;
    protected const float shellRadius = 0.01f;


    private void OnEnable()
    {
        body = GetComponent<Rigidbody2D>();
    }


    // Start is called before the first frame update
    void Start()
    {
        contactFilterPast.SetLayerMask(Physics2D.GetLayerCollisionMask(pastLayer));
        contactFilterPresent.SetLayerMask(Physics2D.GetLayerCollisionMask(presentLayer));
      
        contactFilterPast.useLayerMask = true;
        contactFilterPresent.useLayerMask = true;

        contactFilter = contactFilterPresent;

    }

    // Update is called once per frame
    void Update()
    {
    }

    protected virtual void ComputeVelocity()
    {

    }

    private void FixedUpdate()
    {
        velocity += gravityMod * Physics2D.gravity * Time.deltaTime;
        velocity.x = targetVelocity.x;

        grounded = false;

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

        if(distance > minMoveDist)
        {
            int count = body.Cast(move, contactFilter, hitBuff, distance + shellRadius);

            hitBuffList.Clear();
            for(int i = 0; i < count; i++)
            {
                hitBuffList.Add(hitBuff[i]);
            }
            
            foreach (RaycastHit2D hit in hitBuffList)
            {
                Vector2 currentNormal = hit.normal;

                if(currentNormal.y > minGroundNormY)
                {
                    grounded = true;
                    if (yMove)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(velocity, currentNormal);
                if(projection < 0)
                {
                    velocity = velocity - projection * currentNormal;
                }

                float modifiedDist = hit.distance - shellRadius;
                distance = modifiedDist < distance ? modifiedDist : distance;
            }
        }

        body.position += move.normalized * distance;
    }
}
