using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityProjection : MonoBehaviour
{

    [SerializeField] int projectionSteps;

    [SerializeField] float stepSize = 0.5f;

    Rigidbody body;
    LineRenderer line;
    GravityCalculator grav;
    DroneController controller;


    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        line = GetComponent<LineRenderer>();
        grav = GetComponent<GravityCalculator>();
        controller = GetComponent<DroneController>();

        line.positionCount = projectionSteps;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        


        if (controller.boostCharge > 0)
        {
            line.enabled = true;
            GetProjectionFromVelocity(controller.boostVec + body.velocity);
        }
        else
        {
            if (line.enabled == true && grav.isNearGround)
            {
                line.enabled = false;
            }

            if (line.enabled == false && !grav.isNearGround)
            {
                line.enabled = true;
            }
            GetProjectionFromVelocity(body.velocity);
        }
        


    }

    void GetProjectionFromVelocity(Vector3 vel)
    {
        Vector3 projectedPos = transform.position;
        Vector3 projectedVelocity = vel;
        for (int i = 0; i < projectionSteps; i++)
        {

            line.SetPosition(i, projectedPos);

            projectedPos += projectedVelocity * stepSize;
            Vector3 stepGrav = grav.ProjectedGravityForceAtPosition(projectedPos) * stepSize;
            projectedVelocity += stepGrav;

        }
    }
}
