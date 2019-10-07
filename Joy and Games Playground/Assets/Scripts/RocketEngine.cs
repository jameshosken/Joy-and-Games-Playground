using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketEngine : MonoBehaviour
{

    enum RocketType { START, BG, CRASH };
    [SerializeField] RocketType rocketType;
    [SerializeField] float enginePower = -50f;

    [SerializeField] float ignitionDelay = 1.5f;
    [SerializeField] float BGIgnitionDelay = 15f;
    [SerializeField] float deathDelay = 30f;
    Rigidbody body;
    ParticleSystem particles;
    ParticleSystem.EmissionModule emission;



    bool ignition = false;
    bool thrust = false;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        particles = GetComponent<ParticleSystem>();
        emission = particles.emission;
        emission.enabled = false;
        body.velocity = Vector3.up*0.01f;

        if(rocketType == RocketType.BG)
        {
            Invoke("Ignite", BGIgnitionDelay);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (rocketType)
        {
            case RocketType.START:
                StartRocketUpdate();
                break;
            case RocketType.BG:
                BGRocketUpdate();
                break;
            case RocketType.CRASH:
                //Do Nothing;
                break;
            default:
                break;
        }
        
    }


    void StartRocketUpdate()
    {
        if (body.velocity.magnitude == 0 && ignition == false)
        {
            ignition = true;
            Invoke("Ignite", ignitionDelay);
        }

        if (ignition && thrust)
        {
            body.AddRelativeForce(Vector3.up * enginePower);
        }

    }

    void BGRocketUpdate()
    {

        if (thrust)
        {
            body.AddRelativeForce(Vector3.up * enginePower);
        }


        float x = 10f;
    }


    void Ignite()
    {
        thrust = true;
        emission.enabled = true;

        if(rocketType == RocketType.BG)
        {
            GetComponent<Rigidbody>().isKinematic = false;
        }

        Invoke("Die", deathDelay);  // Alive for 60 seconds
    }

    void Die()
    {
        GameObject.Destroy(gameObject);
    }
}
