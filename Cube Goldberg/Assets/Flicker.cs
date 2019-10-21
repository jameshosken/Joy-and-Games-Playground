using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flicker : MonoBehaviour
{
    [SerializeField] float speed = 0.01f;
    Light light;

    float intensityMax;

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
        intensityMax = light.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        light.intensity = Mathf.PerlinNoise(Time.time * speed, 0) * intensityMax;
    }
}
