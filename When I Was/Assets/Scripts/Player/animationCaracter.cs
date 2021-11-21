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
        bool doubleJump = this.transform.parent.GetComponent<PlayerController>().usedDoubleJump ;
        if (_momentum.x < -0.5f ||_momentum.x > 0.5f )
        {
            m_Animator.SetBool("move",true);
        }
        else
        {
            m_Animator.SetBool("move",false);
        }
        
        if (_momentum.y < -0.9f ||_momentum.y > 0.9f )
        {
            m_Animator.SetBool("jump",true);
            if(doubleJump) m_Animator.SetBool("doubleJump",true);
        }
        else
        {
            m_Animator.SetBool("jump",false);
            m_Animator.SetBool("doubleJump",false);
        }
        
    }
}
