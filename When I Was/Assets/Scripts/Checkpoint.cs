using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public GameObject newSpawn;
    public bool activate = false; 
    public Animator animator; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (activate)
        {
            animator.SetBool("ActivateCheckPoint",true);
        }
        else
        {
            animator.SetBool("ActivateCheckPoint",false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // collision.gameObject.GetComponent<Checkpoint>().activate = false;
        
        if (collision.CompareTag("Player"))
        {
            activate = true;
            PlayerController playerController =(PlayerController) collision.GetComponent("PlayerController");
            playerController.spawnPoint = newSpawn;
        }
    }
}
