using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booster : MonoBehaviour
{
    //Will apply this force along positive Z
    [SerializeField] float force = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Rigidbody>())
        {
            Rigidbody body = other.GetComponent<Rigidbody>();

            body.AddForce(transform.forward * force);
        }
    }
}
