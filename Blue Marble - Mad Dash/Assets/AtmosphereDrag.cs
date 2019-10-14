using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtmosphereDrag : MonoBehaviour
{
    [SerializeField] float drag = 0.01f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Rigidbody>())
        {
            Rigidbody body = other.GetComponent<Rigidbody>();

            body.drag = drag;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Rigidbody>())
        {
            Rigidbody body = other.GetComponent<Rigidbody>();

            body.drag = 0;
        }
    }
}
