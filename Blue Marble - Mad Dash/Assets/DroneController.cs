using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DroneController : MonoBehaviour
{
    [SerializeField] float forwardThrust;
    [SerializeField] float turnThrust;
    [SerializeField] float boostIncrement = .25f;
    [SerializeField] float inertialDampIncrement = .25f;
    [SerializeField] float maxDampener = 10;
    [SerializeField] float maxBoost = 10;
    [SerializeField] Transform camPivot;
    [SerializeField] ParticleSystem dustParticles;
    

    [SerializeField] ParticleSystem leftParticles;
    [SerializeField] ParticleSystem rightParticles;
    [SerializeField] Slider boostSlider;
    [SerializeField] Slider dampnerSlider;

    LineRenderer line;

    ParticleSystem.EmissionModule leftEm;
    ParticleSystem.EmissionModule rightEm;
    Rigidbody body;

    GravityCalculator gravCalc;

    public float boostCharge = 0;
    public float inertialDampenerCharge;

    public Vector3 boostVec = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        gravCalc = GetComponent<GravityCalculator>();

        leftEm = leftParticles.emission;
        rightEm = rightParticles.emission;

        boostSlider.maxValue = maxBoost;
        dampnerSlider.maxValue = maxDampener;

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        float boost = 1;
        

        float fwd = Input.GetAxis("Vertical");
        float turn = Input.GetAxis("Horizontal");

        if (gravCalc.isGrounded)
        {
            body.AddForce(camPivot.forward * fwd * forwardThrust * boost);
            body.AddForce(camPivot.right * turn * turnThrust);

            body.velocity *= 0.997f;
        }



        

        


        

    }

    private void Update()
    {
        HandleBoosts();
    }

    private void HandleBoosts()
    {
        if (inertialDampenerCharge < maxDampener)
        {
            inertialDampenerCharge += inertialDampIncrement;
        }

        

        if (Input.GetKey(KeyCode.Space) && gravCalc.isNearGround)
        {
            if(boostCharge < maxBoost)
            {
                boostCharge += boostIncrement;

            }
        }
        else
        {
               //boostCharge -= boostIncrement;
        }

        boostVec = camPivot.up * boostCharge;

        if (Input.GetKeyUp(KeyCode.Space))
        {

            body.AddForce(boostVec, ForceMode.Impulse);
            rightParticles.Emit(40);
            leftParticles.Emit(40);

            boostCharge = 0;
        }

        if (Input.GetKey(KeyCode.X) && inertialDampenerCharge > 0)
        {
            body.velocity *= 0.97f;
            inertialDampenerCharge -= 0.1f ;
        }

        boostSlider.value = boostCharge;
        dampnerSlider.value = inertialDampenerCharge;

    }

    private void OnCollisionStay(Collision collision)
    {
        dustParticles.transform.position = collision.contacts[0].point;
        dustParticles.Emit(1);
    }
}
