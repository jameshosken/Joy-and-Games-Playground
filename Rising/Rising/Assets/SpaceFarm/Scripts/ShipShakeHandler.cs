using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipShakeHandler : MonoBehaviour
{

    [SerializeField] float freq;
    [SerializeField] float maxAmp;

    float offset;

    float amp = 0;

    bool isShaking = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (amp > 0)
        {
            transform.localPosition = Vector3.zero;
            Vector3 shakePosition = new Vector3(
                Mathf.PerlinNoise(offset, 0),
                Mathf.PerlinNoise(0, offset),
                Mathf.PerlinNoise(offset, offset)
                );

            transform.localPosition = shakePosition * amp;

            offset += freq;
        }


        if (isShaking)
        {
            amp = Mathf.Lerp(amp, maxAmp, 0.1f);
        }
        else
        {
            amp = Mathf.Lerp(amp, 0, 0.1f);
        }
    }

    public void SetIsShaking(bool b)
    {
        isShaking = b;
    }
}
