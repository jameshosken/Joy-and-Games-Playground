using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityIndicator : MonoBehaviour
{
    [SerializeField] Rigidbody ship;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vel = ship.velocity;

        transform.LookAt(transform.position + vel);
    }
}
