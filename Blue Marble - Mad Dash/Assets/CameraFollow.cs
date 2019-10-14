using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] float followSpeed = 0.5f;
    [SerializeField] float velocityFollowSpeed = 0.01f;
    [SerializeField] Rigidbody droneBody;
    [SerializeField] GameObject pivot;
    [SerializeField] GravityCalculator grav;

    Vector3 velocity = Vector3.zero;


    public Vector3 worldUp = Vector3.up;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.transform.position, followSpeed);

        worldUp = Vector3.Lerp(worldUp, grav.getCurrentUp(), velocityFollowSpeed);


        velocity = Vector3.Lerp(velocity, droneBody.velocity, velocityFollowSpeed);
        //pivot.transform.LookAt(transform.position + velocity, worldUp);
        Quaternion lookRot = Quaternion.LookRotation( velocity, worldUp);
        pivot.transform.rotation = Quaternion.Slerp(pivot.transform.rotation, lookRot, velocityFollowSpeed);

    }
}
