using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private SpriteRenderer _sr;
    public Vector2 _momentum;
    public float speedMaxY = -40.0f;
    private Vector2 _targetMomentum;
    private float _height;
    private float _heightOffset;
    private float _lastOnGroundAt = 0;
    
    public GameObject groundLevel;

    public float coyoteTime = 0.1f;
    public float airCoyoteTime = 0.1f;

    public float jumpForce = 100;
    public Vector2 wallJumpForce = new Vector2(170, 100);
    public float gravity = 10;
    public float acceleration = 0.1f;
    public float airAcceleration = 0.05f;
    public float speed = 30;

    protected int pastLayer = 8;
    protected int presentLayer = 9;
    protected int persistentLayer = 10;
    protected int playerPastLayer = 12;
    protected int playerPresentLayer = 13;
    
    protected int currentWorldLayer;

    protected ContactFilter2D contactFilterPast;
    protected ContactFilter2D contactFilterPresent;

    public Material SceneRenderMaterial;
    private Rigidbody2D Grabbed = null;
    private Vector3 grabbedOffset;
    
    public int level = 0;
    public int Level { get { return level; } set { level = value; } }
    
    bool doubleJump = false;
    public bool usedDoubleJump = false;
    bool usedWallJump = false;

    public FMODUnity.StudioEventEmitter jumpSound;
    public FMODUnity.StudioEventEmitter swapSound;
    public FMODUnity.StudioEventEmitter dieSound;
    public FMODUnity.StudioEventEmitter music;

    float airInputOffsetTimer = 0;

    private bool airAccelerationInterpolation = false;
    private bool accelerationInterpolation = true;
    private bool gravityInterpolation = true;
    private bool useLowGravity = false;
    private bool jumpCommand = false;
    private bool leftWallJumpCommand = false;
    private bool rightWallJumpCommand = false;

    public bool controlleDisable = false;
    private enum State {
        Past, Present
    };
    private State gabbyState = State.Present;

    public float circleRadius = 6f;

    public GameObject spawnPoint;//where the character respawns on death

    Rigidbody2D _rb;

    private bool OnGround
    {
        get { return _momentum.y <= 0 && _lastOnGroundAt > Time.time - coyoteTime; }
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponentInChildren<SpriteRenderer>();
        _height = (transform.position - groundLevel.transform.position).magnitude;
        _heightOffset = 0f;

        contactFilterPast.SetLayerMask((int) Mathf.Pow(2, pastLayer));
        contactFilterPresent.SetLayerMask((int) Mathf.Pow(2, presentLayer));
        contactFilterPast.useLayerMask = true;
        contactFilterPresent.useLayerMask = true;
        currentWorldLayer = presentLayer;

        //initializeGO();

        StartCoroutine("RenderBlendCoroutine");
    }

    void Update() {

        //Inputs
        float x = Input.GetAxis("Horizontal");
        bool jump = Input.GetKeyDown(KeyCode.Space);

        if (Grabbed != null) {
            jump = false;
            Grabbed.velocity = (transform.position + grabbedOffset - Grabbed.transform.position) * 10;
        }
        

        RaycastHit2D hit2DWallRight = Physics2D.Raycast(transform.position, new Vector2(1, 0),
            9.0f, (int) Mathf.Pow(2, currentWorldLayer));
        RaycastHit2D hit2DWallLeft = Physics2D.Raycast(transform.position, new Vector2(-1, 0),
            9.0f, (int) Mathf.Pow(2, currentWorldLayer));
        // Debug.DrawRay(transform.position, new Vector2(10.0f, 0),
        //     Color.blue, 0.1f);

        //Slower if you grabbed something
        if (Grabbed != null) {
            _targetMomentum.x = x * speed / 2;
        } else {
            _targetMomentum.x = x * speed;
        }

        if (OnGround) {
            gravityInterpolation = false;
            accelerationInterpolation = true;
            if (jump) {
                jumpCommand = true;
            } else {
                _rb.velocity = new Vector2(_rb.velocity.x, 0);
            }
            usedDoubleJump = false;
        } else {
            accelerationInterpolation = false;
            gravityInterpolation = true;
            usedWallJump = false;

            if (level >= 1 && !usedDoubleJump) doubleJump = Input.GetKeyDown(KeyCode.Space);

            if ((Mathf.Abs(_momentum.x) < Mathf.Abs(_targetMomentum.x) || _momentum.x * x < 0) && airInputOffsetTimer < 0) {
                airAccelerationInterpolation = true;
            } else {
                airAccelerationInterpolation = false;
            }

            airInputOffsetTimer -= Time.deltaTime;


            if (jump && !usedDoubleJump) {
                jumpCommand = true;
                usedDoubleJump = true;
            }
            if (hit2DWallRight.collider != null && !usedWallJump && level >= 2 && jump && !leftWallJumpCommand) {
                leftWallJumpCommand = true;
                usedWallJump = true;
            }
            if (hit2DWallLeft.collider != null && !usedWallJump && level >= 2 && jump && !rightWallJumpCommand) {
                rightWallJumpCommand = true;
                usedWallJump = true;
            }

            if (hit2DWallRight.collider != null || hit2DWallLeft.collider != null) {
                useLowGravity = true;
            } else {
                useLowGravity = false;
            }
        } 

        

        //Snap to ground
        if (_rb.velocity.y <= 0) {
            RaycastHit2D hit2D = Physics2D.Raycast(transform.position, new Vector2(0, -1), _height + _heightOffset, (int) Mathf.Pow(2, currentWorldLayer));
            if (hit2D.collider != null) {
                Vector2 pos = new Vector2(transform.position.x, transform.position.y);
                float diff = _height - (pos - hit2D.point).magnitude;
                transform.position += Vector3.up * diff;
               
                _lastOnGroundAt = Time.time;
                _heightOffset = 5;
            } else {
                _heightOffset = 0;
            }
        }
        //Animation
        if (_rb.velocity.x > 1 && _sr.flipX) {
            _sr.flipX = false;
        }

        if (_rb.velocity.x < -1 && !_sr.flipX) {
            _sr.flipX = true;
        }

        //Swap timeline
        if (level >= 3 && Input.GetKeyDown(KeyCode.LeftShift)){
            Grabbed = null;
            
            List<Collider2D> collisionsPast = new List<Collider2D>();
            List<Collider2D> collisionsPresent = new List<Collider2D>();

            Physics2D.OverlapCircle(transform.position, circleRadius, contactFilterPast, collisionsPast);
            Physics2D.OverlapCircle(transform.position, circleRadius, contactFilterPresent, collisionsPresent);

            foreach (Collider2D co in collisionsPast)
                Debug.Log(co.gameObject.name);

            foreach (Collider2D co in collisionsPresent)
                Debug.Log(co.gameObject.name);

            if (gabbyState == State.Present && collisionsPast.Count == 0)
            {
                gabbyState = State.Past;

                gameObject.layer = playerPastLayer;
                currentWorldLayer = pastLayer;

                //showBranch(pastParent);
                //hideBranch(presentParent);

                swapSound.Play();
            }
            else if (gabbyState == State.Past && collisionsPresent.Count == 0)
            {
                gabbyState = State.Present;

                gameObject.layer = playerPresentLayer;
                currentWorldLayer = presentLayer;

                //showBranch(presentParent);
                //hideBranch(pastParent);

                swapSound.Play();
            }
            else
            {
                noiseTimer = 1;
                Debug.LogError("Collision !");
            }
        }

        //Grab objects
        if (Input.GetKeyDown(KeyCode.LeftControl)) {
            Vector2 direction = new Vector2(1, 0);
            RaycastHit2D hit2D = Physics2D.Raycast(transform.position, direction, 15, (int)Mathf.Pow(2, currentWorldLayer));
            if (hit2D.collider != null && hit2D.collider.tag.Equals("Box")) {
                Rigidbody2D hit = hit2D.collider.gameObject.GetComponent<Rigidbody2D>();
                Grabbed = hit;
                if (Grabbed != null) {
                    grabbedOffset = Grabbed.transform.position - transform.position + new Vector3(direction.x, direction.y, 0) * 2;
                }
            } else {
                direction = -direction;
                hit2D = Physics2D.Raycast(transform.position, direction, 15, (int)Mathf.Pow(2, currentWorldLayer));
                if (hit2D.collider != null && hit2D.collider.tag.Equals("Box")) {
                    Rigidbody2D hit = hit2D.collider.gameObject.GetComponent<Rigidbody2D>();
                    Grabbed = hit;
                    if (Grabbed != null) {
                        grabbedOffset = Grabbed.transform.position - transform.position + new Vector3(direction.x, direction.y, 0) * 2;
                    }
                }
            }
        } if (Input.GetKeyUp(KeyCode.LeftControl) && Grabbed != null) {
            Debug.Log("Released " + Grabbed.name);
            Grabbed = null;
        }

    }

    float noiseTimer = 0;
    IEnumerator RenderBlendCoroutine () {
        float timer = 0;
        while (true) {
            if (gabbyState == State.Present) {
                timer -= Time.deltaTime * 2;
            } else {
                timer += Time.deltaTime * 2;
            }
            timer = Mathf.Clamp01(timer);

            noiseTimer -= Time.deltaTime * 2;
            noiseTimer = Mathf.Clamp01(noiseTimer);

            SceneRenderMaterial.SetFloat("_Blend", timer);
            SceneRenderMaterial.SetFloat("_Noise", noiseTimer);
            music.SetParameter("Switching", timer * 10);


            yield return new WaitForEndOfFrame();
        }
    }

    private void FixedUpdate () {
        //Movement
        _momentum = _rb.velocity;
        if (_momentum.y < speedMaxY) _momentum.y = speedMaxY;

        if (airAccelerationInterpolation) {
            _momentum.x = _targetMomentum.x * airAcceleration + _momentum.x * (1 - airAcceleration);
        }
        if (accelerationInterpolation) {
            _momentum.x = _targetMomentum.x * acceleration + _momentum.x * (1 - acceleration);
        }
        if (gravityInterpolation) {
            if (useLowGravity) {
                _momentum.y -= gravity * Time.fixedDeltaTime * 0.5f;
            } else {
                _momentum.y -= gravity * Time.fixedDeltaTime;
            }
        }
        if (jumpCommand && !leftWallJumpCommand && !rightWallJumpCommand) {
            _momentum.y = jumpForce;

            airInputOffsetTimer = airCoyoteTime;
            _heightOffset = 0f;
            jumpSound.Play();

            jumpCommand = false;
        } 
        if (leftWallJumpCommand) {
            _momentum.y = wallJumpForce.y;
            _momentum.x = -wallJumpForce.x;

            airInputOffsetTimer = airCoyoteTime;
            _heightOffset = 0f;
            jumpSound.Play();

            leftWallJumpCommand = false;
        }
        if (rightWallJumpCommand) {
            _momentum.y = wallJumpForce.y;
            _momentum.x = wallJumpForce.x;

            airInputOffsetTimer = airCoyoteTime;
            _heightOffset = 0f;
            jumpSound.Play();

            rightWallJumpCommand = false;
        }

        //Set Velocity
        _rb.velocity = _momentum;
    }

    //void initializeGO()
    //{
    //    showBranch(presentParent);
    //    hideBranch(pastParent);
    //}

    //void SetVisibility (GameObject o, bool visible) {
    //    Renderer r = o.GetComponent<Renderer>();
    //    if (r != null) r.enabled = visible;
    //    for (int i = 0; i < o.transform.childCount; i++) { //TODO
    //        SetVisibility(o.transform.GetChild(i).gameObject, visible);
    //    }
    //}

    void showBranch (GameObject parent) {
        GameObject toHide = parent.transform.Find("ToHide").gameObject;
        GameObject toDisable = parent.transform.Find("ToDisable").gameObject;

        toDisable.SetActive(true);
        //SetVisibility(toHide, true);
    }

    void hideBranch (GameObject parent) {
        GameObject toHide = parent.transform.Find("ToHide").gameObject;
        GameObject toDisable = parent.transform.Find("ToDisable").gameObject;

        toDisable.SetActive(false);
        //SetVisibility(toHide, false);
    }

    void SetVisibility(GameObject o, bool visible)
    {
        Renderer r = o.GetComponent<Renderer>();
        if (r != null) r.enabled = visible;
        for (int i = 0; i < o.transform.childCount; i++)
        {
            //TODO
            SetVisibility(o.transform.GetChild(i).gameObject, visible);
        }
    }

    public void powerUp(  )
    {
        level +=1; 
        Debug.Log( level );
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Danger"))
            StartCoroutine(killPlayer());
    }
    
    private IEnumerator killPlayer() {
        dieSound.Play();
        yield return new WaitForSeconds(0.3f);
        Death();
    }
    
    protected void Death()
    {
        this.transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, 0);
        _rb.velocity = new Vector2(0, 0);
    }
}