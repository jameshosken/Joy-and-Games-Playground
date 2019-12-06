using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControlsHandler : MonoBehaviour
{
    [SerializeField] float thrust = 1f;
    [SerializeField] float torque = 1f;
    public Rigidbody body;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();   
    }

    public void ApplyRelativeThrust(Vector3 vector)
    {
        body.AddRelativeForce(vector * thrust);
    }

    public void ApplyRelativeTorque(Vector3 vector)
    {
        body.AddRelativeTorque(vector * torque);
    }

    public void ApplyTorque(Vector3 vector)
    {
        body.AddTorque(vector * torque);
    }
}
