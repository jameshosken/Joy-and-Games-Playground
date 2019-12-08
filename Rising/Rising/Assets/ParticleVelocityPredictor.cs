using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleVelocityPredictor : MonoBehaviour
{
    [SerializeField] Rigidbody body;
    [SerializeField] float predictionDistanceMultiplier = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.parent.position +  body.velocity * predictionDistanceMultiplier;
    }
}
