using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaygroundMove : MonoBehaviour
{

    [SerializeField] float speed = 1;
    [SerializeField] float turnspeed = 1;
    Rigidbody body;

    float fwd = 0;
    float turn = 0;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        fwd = Input.GetAxis("Vertical");

        turn = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        body.AddForce(transform.forward * fwd * speed);

        body.AddTorque(transform.up * turn * turnspeed);
    }
}
