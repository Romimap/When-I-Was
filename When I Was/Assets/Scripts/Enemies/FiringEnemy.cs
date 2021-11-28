using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiringEnemy : MonoBehaviour
{
    private float delay = 0;

    public float RoF = 0.5f;
    public GameObject weaponPrefab;
    public float minAngle = 0.0f;
    public float maxAngle = 360.0f;
    public float rotSpeed = 1.0f;
    private float currentAngle;
    private int rotDir = 1; 
    // Start is called before the first frame update
    void Start()
    {
        currentAngle = minAngle;
    }

    // Update is called once per frame
    void Update()
    {
        delay += Time.deltaTime;
        currentAngle += Time.deltaTime * rotSpeed * rotDir;

        if(currentAngle >= maxAngle)
            rotDir = -1;
        if(currentAngle <= minAngle)
            rotDir = 1;
        
        this.transform.rotation = Quaternion.Euler(0.0f,0.0f,currentAngle);
        if (delay > (1 / RoF))
        {
            delay = 0;
            GameObject newLaser = Instantiate(weaponPrefab, transform.position, transform.rotation);
            newLaser.layer = this.gameObject.layer;
        }
    }
}
