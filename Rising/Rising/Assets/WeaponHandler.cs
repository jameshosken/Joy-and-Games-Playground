using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{

    public bool isFiring = false;

    [SerializeField] GameObject harpoonTemplate;
    [SerializeField] Transform[] harpoonEmitters;
    [SerializeField] float harpoonForce = 10f;

    [SerializeField] float firingRate = 1f;

    float lastFireTime = 0;

    int activeEmitter = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isFiring)
        {
            HandleFireTiming();
        }
    }

    private void HandleFireTiming()
    {
        if(Time.realtimeSinceStartup - lastFireTime > firingRate)
        {
            HandleFireEvent();
            lastFireTime = Time.realtimeSinceStartup;
        }
    }

    private void HandleFireEvent()
    {

        GameObject harpoon = Instantiate(
            harpoonTemplate, 
            harpoonEmitters[activeEmitter].position, 
            harpoonEmitters[activeEmitter].rotation * Quaternion.Euler( UnityEngine.Random.insideUnitSphere*0.1f) );

        harpoon.GetComponent<Rigidbody>().AddRelativeForce(
            Vector3.forward * harpoonForce, 
            ForceMode.Impulse);
        harpoon.GetComponent<Rigidbody>().AddForce(
            GetComponent<Rigidbody>().velocity);

        activeEmitter = (activeEmitter + 1) % harpoonEmitters.Length;
    }
}
