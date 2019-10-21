using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Rigidbody target;
    [SerializeField] float followSpeed;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        float lerpAmt = target.velocity.magnitude * followSpeed;

        transform.position = Vector3.Lerp(transform.position, target.transform.position, lerpAmt);
    }
}
