using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityCalculator : MonoBehaviour
{

    [SerializeField] float GravitationalConstant = 10;
    [SerializeField] GravityBody[] GravityBodies;
    Rigidbody body;

    public bool isGrounded = false;

    public bool isNearGround = false;

    GravityBody currentGroundedPlanetBody;
    // Start is called before the first frame update
    void Start()
    {
        GravityBodies = FindObjectsOfType<GravityBody>();
        currentGroundedPlanetBody = GravityBodies[0];
        body = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {


        Vector3 force = CalculateTotalGravityForceAtPosition(transform.position);

        body.AddForce(force);
    }

    public Vector3 ProjectedGravityForceAtPosition(Vector3 position)
    {
        Vector3 force = Vector3.zero;
       
        for (int i = 0; i < GravityBodies.Length; i++)
        {
            Vector3 grav = CalculateGravityForceAtPosition(GravityBodies[i], position);
                
            force += grav;
        }
        

        return force;
    }

    Vector3 CalculateTotalGravityForceAtPosition(Vector3 position)
    {
        Vector3 force = Vector3.zero;
        if (isNearGround)
        {
            force += CalculateGravityForceAtPosition(currentGroundedPlanetBody, position);
        }

        else
        {
            float maxGrav = 0;
            int strongestGravIndex = -1;
            for (int i = 0; i < GravityBodies.Length; i++)
            {
                Vector3 grav = CalculateGravityForceAtPosition(GravityBodies[i], position);
                //Debug.Log("Grav " + i + ": " + grav.magnitude);
                if (grav.magnitude > maxGrav)
                {
                    maxGrav = grav.magnitude;
                    strongestGravIndex = i;
                }
                force += grav;
            }
            //Debug.Log("strongest Index: " + strongestGravIndex + " at: " + maxGrav);
            //If strongest grav body has changed
            if (GravityBodies[strongestGravIndex] != currentGroundedPlanetBody)
            {

                //Debug.Log("Overpowering " + currentGroundedPlanetBody.name + "with " + GravityBodies[strongestGravIndex].name);
                //Debug.Log("From " + currentGroundedPlanetBody.mass + "to " + GravityBodies[strongestGravIndex].mass);
                currentGroundedPlanetBody = GravityBodies[strongestGravIndex];
            }
        }

        return force;
    }

    Vector3 CalculateGravityForceAtPosition(GravityBody gravbody, Vector3 position)
    {
        // F = g * M1.M2/r^2

        float r = Vector3.Distance(position, gravbody.transform.position);

        float m1 = gravbody.mass;

        Vector3 directionTowardsCenter = gravbody.transform.position - position;
        directionTowardsCenter = directionTowardsCenter.normalized;

        return directionTowardsCenter * (GravitationalConstant * m1 / Mathf.Pow(r, 2));

    }

    public Vector3 getCurrentUp()
    {
        //Debug.Log(currentGroundedPlanetBody.name);
        if (currentGroundedPlanetBody == null)
        {
            return Vector3.up;
        }
        else
        {
            return (transform.position - currentGroundedPlanetBody.transform.position).normalized;
        }
    }


    private void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }


    private void OnCollisionEnter(Collision collision)
    {

        isGrounded = true;
        if (collision.collider.GetComponent<GravityBody>() != null)
        {
            currentGroundedPlanetBody = collision.collider.GetComponent<GravityBody>();

            
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "GroundContact")
        {
            Debug.Log("CONTACT");
            isNearGround = true;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.name == "GroundContact")
        {

            Debug.Log("NO CONTACT");
            isNearGround = false;
        }

    }
}
