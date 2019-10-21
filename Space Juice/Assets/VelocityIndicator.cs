using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VelocityIndicator : MonoBehaviour
{
    [SerializeField] Rigidbody body;
    SpaceshipController controller;
    [SerializeField] string label = "VEL";
    Text text;
    // Start is called before the first frame update
    void Start()
    {
        controller = FindObjectOfType<SpaceshipController>();
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        float vel = Mathf.Pow(body.velocity.magnitude / controller.maxVelocity , 2f) * 100;

        vel = (float)System.Math.Round(vel, 1);

        text.text = label + "\n" + vel.ToString();
    }
}
