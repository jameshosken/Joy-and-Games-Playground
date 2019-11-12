using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] AnimationCurve curve;

    AudioSource source;


    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void UpdateSeason(float f)
    {
        source.volume = curve.Evaluate(f);
    }
}
