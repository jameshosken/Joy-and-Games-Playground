using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OxygenTimer : MonoBehaviour
{
    public float gameTime;
    [SerializeField] Slider meter;
    
    // Start is called before the first frame update
    void Start()
    {
        meter.maxValue = gameTime;
    }

    // Update is called once per frame
    void Update()
    {
        gameTime -= Time.deltaTime;
        meter.value = gameTime;
    }
}
