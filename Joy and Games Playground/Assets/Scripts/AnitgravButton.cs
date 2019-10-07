using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnitgravButton : MonoBehaviour
{


    [SerializeField] FlipCam camRotationScript;
    //Script to switch gravity when ball 
    [SerializeField] float gravAdjustSpeed = 0.1f;
    Vector3 targetPos;

    Vector3 grav;
    bool flipped = false;
    bool active = false;
    
    // Start is called before the first frame update
    void Start()
    {
        targetPos = transform.position;
        grav = Physics.gravity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        
        if(transform.position != targetPos)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, 0.1f);
             
        }
        if (!active)
        {
            return;
        }


        if (grav!= Physics.gravity)
        {
            Physics.gravity = Vector3.Lerp(Physics.gravity, grav, gravAdjustSpeed);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (flipped )
        {
            return;
        }

        if(collision.collider.gameObject.tag == "Player")
        {
            active = true;
            flipped = true;
            grav = Physics.gravity * -1; 
            targetPos = transform.position - transform.up;
            Invoke("ActivateCameraFlip", 5f);
            Invoke("Deactivate", 10f);
        }
    }

    void ActivateCameraFlip()
    {
        camRotationScript.Flip();
    }
    
    void Deactivate()
    {
        active = false;
        flipped = false;
        targetPos = transform.position + transform.up;
    }
}
