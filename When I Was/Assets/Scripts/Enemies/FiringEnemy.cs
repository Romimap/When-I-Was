using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiringEnemy : MonoBehaviour
{
    private float delay = 0;

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

        if (delay > (1 / RoF))
        {
            delay = 0;
            GameObject newLaser = Instantiate(weaponPrefab, transform.position, transform.rotation);
        }
    }
}
