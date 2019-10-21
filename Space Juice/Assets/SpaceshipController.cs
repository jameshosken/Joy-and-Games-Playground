using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    Rigidbody body;

    public float maxVelocity;
    [SerializeField] float preBoostLimit = 0.96f;

    [SerializeField] float thrustSensitivity = 1;
    [SerializeField] float pitchSensitivity = 1;
    [SerializeField] float rollSensitivity = 1;
    [SerializeField] float yawSensitivity = 1;

    [SerializeField] AnimationCurve controlResponsivenessCurve; //As you approach lightspeed, it becomes harder to turn
    [SerializeField] ParticleSystem hyperspeed;

    public bool isReadyToPunch = false;

    float fwd = 0;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        fwd = Input.GetAxis("Vertical");
        float yaw = Input.GetAxis("Horizontal");

        float fRoll = Input.GetAxis("Mouse X");
        float fPitch = Input.GetAxis("Mouse Y");

        float responsiveness = controlResponsivenessCurve.Evaluate(body.velocity.magnitude / maxVelocity);

        Vector3 vRoll = new Vector3(0, 0, fRoll * rollSensitivity) * responsiveness;
        Vector3 vPitch = new Vector3(fPitch * pitchSensitivity, 0, 0) * responsiveness;
        Vector3 vYaw = new Vector3(0, yaw * yawSensitivity, 0) * responsiveness;

        body.AddRelativeTorque(vRoll);
        body.AddRelativeTorque(vPitch);
        body.AddRelativeTorque(vYaw);

        bool boost = false;
        if (Input.GetKey(KeyCode.Space))
        {
            boost = true;
        }

        isReadyToPunch = false;
        if (body.velocity.magnitude > maxVelocity * preBoostLimit && boost == false)
        {
            body.velocity = Vector3.Lerp(body.velocity, body.velocity.normalized * maxVelocity * preBoostLimit, 0.033f);
            isReadyToPunch = true;

        }
        else if (body.velocity.magnitude > maxVelocity * preBoostLimit && boost == true)
        {
            
            //body.AddRelativeForce((Vector3.forward) * fwd * thrustSensitivity * 0.33f);
            body.velocity = Vector3.Lerp(body.velocity, transform.forward.normalized * maxVelocity , 0.033f);

            if(UnityEngine.Random.Range(0,10) < 2)
            {
                hyperspeed.Emit(Random.Range(1, 3));
            }

        }
        else
        {
            body.AddRelativeForce(Vector3.forward * fwd * thrustSensitivity);
        }



        //Limit max speed;
        if (body.velocity.magnitude > maxVelocity)
        {
            body.velocity = body.velocity.normalized * maxVelocity;
        }

        if (Input.GetKey(KeyCode.X))
        {
            body.velocity *= 0.995f;

            //body.angularVelocity *= 1.1f * body.velocity.magnitude / maxVelocity;   // Add 
        }
    }

    public Vector3 getVelocity()
    {
        return body.velocity;
    }

    public float getAcceleration()
    {
        return fwd;
    }

}
