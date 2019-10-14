using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationFollower : MonoBehaviour
{
    Rigidbody body;

    [SerializeField] Transform target;
    [SerializeField] float followSpeed = 0.1f;
    GravityCalculator grav;

    Vector3 worldUp = Vector3.up;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponentInParent<Rigidbody>();
        grav = GetComponentInParent<GravityCalculator>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        worldUp = Vector3.Lerp(worldUp, grav.getCurrentUp(), 0.05f);

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(body.velocity, worldUp), followSpeed);
        //transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, followSpeed);
    }
}
