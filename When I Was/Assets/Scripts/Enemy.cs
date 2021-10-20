using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float delay = 0;
    private Vector3 offset = new Vector3(1, 0, 0);

    public bool patrolling = false;
    public float moveSpeed = 1;

    public bool shooting = true;
    public float RoF = 0.5f;
    public GameObject weaponPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        delay += Time.deltaTime;

        if(shooting)
            if(delay > (1 / RoF))
            {
                delay = 0;
                GameObject newLaser = Instantiate(weaponPrefab, transform.position, transform.rotation);
            }

        if(patrolling)
            this.gameObject.transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);

    }


    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Danger"))
            this.gameObject.transform.Rotate(new Vector3(0, 180, 0));
    }
}
