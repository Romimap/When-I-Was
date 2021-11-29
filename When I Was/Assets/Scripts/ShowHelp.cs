using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

public class ShowHelp : MonoBehaviour
{
    public Image [] sprites;
    private bool isRunning = false;
    private bool isLeverHelp = false;
    private void Update()
    {         
        Debug.DrawRay( transform.position, new Vector2(-1, 0) *20.0f, Color.green );
        Debug.DrawRay( transform.position, new Vector2(1, 0) *20.0f, Color.green );
        
        RaycastHit2D hitLeft  = Physics2D.Raycast(transform.position, new Vector2(-1, 0), 20.0f );
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, new Vector2(1, 0), 20.0f);

        if ( ( hitRight.collider != null && hitRight.transform.gameObject.CompareTag("Box") )
          || ( hitLeft.collider  != null && hitLeft.transform.gameObject.CompareTag("Box")  ) )
        {
            {
                if (!isRunning && !Input.GetKey(KeyCode.LeftControl))
                    StartBlinking( sprites[0] );
                else if ( isRunning && Input.GetKey(KeyCode.LeftControl ))
                    StopBlinking();
            }
        }
        else
        {
            if (isRunning && !isLeverHelp)
                StopBlinking();
        }
        
    }
    
    IEnumerator Blink( object[] parms)
    {
         Image sr = (Image) parms[0];
         isLeverHelp = (bool)parms[1];

         isRunning = true;
         while (true)
         {
             Debug.Log(sr.color.a.ToString() );
             switch(sr.color.a.ToString())
             {
                 case "0":
                     sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1);
                     //Play sound
                     yield return new WaitForSeconds(0.75f);
                     break;
                 case "1":
                     sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0);
                     //Play sound
                     yield return new WaitForSeconds(0.75f);
                     break;
             }
         }

         isLeverHelp = false;
    }
 
     public void StartBlinking( Image sprite, bool isLever = false )
     {
         object[] parms = new object[2]{ sprite, isLever};
         StartCoroutine("Blink", parms);
     }
 
     public void StopBlinking()
     {
         Debug.Log("Stop blinkin");
         StopAllCoroutines();
         isRunning = false;
         foreach( var image in sprites)
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);

     }

}
