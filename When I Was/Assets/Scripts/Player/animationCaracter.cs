using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationCaracter : MonoBehaviour
{
    
    public Animator m_Animator;

    // Update is called once per frame
    void Update()
    {
        Vector2 _momentum = this.transform.parent.GetComponent<PlayerController>()._momentum ;
        if (_momentum.x < -0.5f ||_momentum.x > 0.5f )
        {
            m_Animator.SetBool("move",true);
        }
        else
        {
            m_Animator.SetBool("move",false);
        }
        
        if (_momentum.y < -0.5f ||_momentum.y > 0.5f )
        {
            m_Animator.SetBool("jump",true);
        }
        else
        {
            m_Animator.SetBool("jump",false);
        }
    }
}
