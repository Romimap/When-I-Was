using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
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

    Rigidbody2D _rb;

    private bool OnGround {
        get {
            return _momentum.y <= 0 && _lastOnGroundAt > Time.time - coyoteTime;
        }
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();   
        _sr = GetComponentInChildren<SpriteRenderer>();
        _height = (transform.position - groundLevel.transform.position).magnitude;
        _heightOffset = 0f;
    }

    void Update() {
        //Inputs
        float x = 0;
        if (Input.GetKey(left)) x-=1;
        if (Input.GetKey(right)) x+=1;
        bool jump = Input.GetKeyDown(KeyCode.Space);


        //Movement
        _momentum = _rb.velocity;

        _targetMomentum.x = x * speed;
        if (OnGround) {
            if (jump) {
                _momentum.y = jumpForce;
                _heightOffset = 0f;
            } else {
                _momentum.y = 0;
            }
            _momentum.x = _targetMomentum.x * acceleration + _momentum.x * (1 - acceleration);
        } else {
            _momentum.y -= gravity * Time.deltaTime;
            _momentum.x = _targetMomentum.x * airAcceleration + _momentum.x * (1 - airAcceleration);
        }

        _rb.velocity = _momentum;
        
        //Snap to ground
        if (_momentum.y <= 0) {
            RaycastHit2D hit2D = Physics2D.Raycast(transform.position, new Vector2(0, -1),  _height + _heightOffset, 1);
            if (hit2D.collider != null) {
                Vector2 pos = new Vector2(transform.position.x, transform.position.y);
                float diff = _height - (pos - hit2D.point).magnitude;
                print(diff);
                transform.position += Vector3.up * diff;
                _lastOnGroundAt = Time.time;
                _heightOffset = 5;
            } else {
            _heightOffset = 0;
            }
        }

        //Animation
        if (_momentum.x > 1 && _sr.flipX) {
            _sr.flipX = false;
        }
        if (_momentum.x < -1 && !_sr.flipX) {
            _sr.flipX = true;
        }

    }
}
