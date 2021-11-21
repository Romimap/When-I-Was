using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollingEnemy : MonoBehaviour
{

    public float moveSpeed = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
    }



    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Danger") && !collision.CompareTag("Player"))
            this.gameObject.transform.Rotate(new Vector3(0, 180, 0));
    }
}
