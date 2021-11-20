using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private SpriteRenderer _sr;
    public Vector2 _momentum;
    private Vector2 _targetMomentum;
    private float _height;
    private float _heightOffset;
    private float _lastOnGroundAt = 0;


    public GameObject groundLevel;

    public float coyoteTime = 0.1f;
    public float jumpForce = 100;
    public float gravity = 10;
    public float acceleration = 0.1f;
    public float airAcceleration = 0.05f;
    public float speed = 30;
    public KeyCode left;
    public KeyCode right;
    public KeyCode jump;

    public GameObject past;
    public GameObject present;
    public GameObject persistent;

    protected int pastLayer = 8;
    protected int presentLayer = 9;
    protected int persistentLayer = 10;
    protected int playerPastLayer = 12;
    protected int playerPresentLayer = 13;

    protected int currentWorldLayer;

    protected ContactFilter2D contactFilterPast;
    protected ContactFilter2D contactFilterPresent;
    bool doubleJump = false;
    bool useDoubleJump = false;
    bool useWallJump = false;

    private enum State
    {
        Past,
        Present
    };

    private State gabbyState = State.Present;

    public float circleRadius = 6f;

    public GameObject pastParent; //objects to hide when we switch to present
    public GameObject presentParent; // same but vice-versa

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

        initializeGO();
    }

    void Update()
    {
        //Inputs
        float x = 0;
        if (Input.GetKey(left)) x -= 1;
        if (Input.GetKey(right)) x += 1;
        bool jump = Input.GetKeyDown(KeyCode.Space);

        //Movement
        _momentum = _rb.velocity;
        RaycastHit2D hit2DWallRight = Physics2D.Raycast(transform.position, new Vector2(1, 0),
            7.0f, (int) Mathf.Pow(2, currentWorldLayer));
        RaycastHit2D hit2DWallLeft = Physics2D.Raycast(transform.position, new Vector2(-1, 0),
            7.0f, (int) Mathf.Pow(2, currentWorldLayer));
        // Debug.DrawRay(transform.position, new Vector2(10.0f, 0),
        //     Color.blue, 0.1f);


        _targetMomentum.x = x * speed;
        if (OnGround)
        {
            if (jump)
            {
                _momentum.y = jumpForce;
                _heightOffset = 0f;
            }
            else
            {
                _momentum.y = 0;
            }

            _momentum.x = _targetMomentum.x * acceleration + _momentum.x * (1 - acceleration);
            useDoubleJump = false;
        }
        else if (doubleJump)
        {
            _momentum.y = jumpForce;
            _heightOffset = 0f;
            doubleJump = false;
            useDoubleJump = true;
        }

        else if (hit2DWallRight.collider != null && !useWallJump && jump)
        {
            if (jump && !useWallJump)
            {
                _momentum.x = -jumpForce * 2;
                _momentum.y = jumpForce;
                _heightOffset = 0f;
                useWallJump = true;
            }
        }
        else if (hit2DWallLeft.collider != null && !useWallJump && jump)
        {
            _momentum.x = jumpForce * 2;
            _momentum.y = jumpForce;
            _heightOffset = 0f;
            useWallJump = true;
        }

        else
        {
            useWallJump = false;
            if (!useDoubleJump) doubleJump = Input.GetKeyDown(KeyCode.Space);
            _momentum.y -= gravity * Time.deltaTime;
            _momentum.x = _targetMomentum.x * airAcceleration + _momentum.x * (1 - airAcceleration);
        }

        _rb.velocity = _momentum;

        //Snap to ground
        if (_momentum.y <= 0)
        {
            RaycastHit2D hit2D = Physics2D.Raycast(transform.position, new Vector2(0, -1),
                _height + _heightOffset, (int) Mathf.Pow(2, currentWorldLayer));
            if (hit2D.collider != null)
            {
                Vector2 pos = new Vector2(transform.position.x, transform.position.y);
                float diff = _height - (pos - hit2D.point).magnitude;
                transform.position += Vector3.up * diff;
                _lastOnGroundAt = Time.time;
                _heightOffset = 5;
            }
            else
            {
                _heightOffset = 0;
            }
        }


        //Animation
        if (_momentum.x > 1 && _sr.flipX)
        {
            _sr.flipX = false;
        }

        if (_momentum.x < -1 && !_sr.flipX)
        {
            _sr.flipX = true;
        }

        //Swap timeline
        if (Input.GetKeyDown(KeyCode.T))
        {
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

                showBranch(pastParent);
                hideBranch(presentParent);
            }
            else if (gabbyState == State.Past && collisionsPresent.Count == 0)
            {
                gabbyState = State.Present;

                gameObject.layer = playerPresentLayer;
                currentWorldLayer = presentLayer;

                showBranch(presentParent);
                hideBranch(pastParent);
            }
            else
            {
                Debug.LogError("Collision !");
            }
        }
    }

    void initializeGO()
    {
        showBranch(presentParent);
        hideBranch(pastParent);
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

    void showBranch(GameObject parent)
    {
        GameObject toHide = parent.transform.Find("ToHide").gameObject;
        GameObject toDisable = parent.transform.Find("ToDisable").gameObject;

        toDisable.SetActive(true);
        SetVisibility(toHide, true);
    }

    void hideBranch(GameObject parent)
    {
        GameObject toHide = parent.transform.Find("ToHide").gameObject;
        GameObject toDisable = parent.transform.Find("ToDisable").gameObject;

        toDisable.SetActive(false);
        SetVisibility(toHide, false);
    }
}