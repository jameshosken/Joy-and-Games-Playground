using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineCrashApplyForce : MonoBehaviour
{

    [SerializeField] float thrust = 100f;

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
        if (other.gameObject.GetComponent<Rigidbody>())
        {
            Rigidbody body =  other.gameObject.GetComponent<Rigidbody>();
            body.AddForce(transform.up * thrust);

        }
    }

    

   
}
