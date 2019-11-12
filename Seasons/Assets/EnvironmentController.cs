using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnvironmentController : MonoBehaviour
{
    [SerializeField] Gradient fogGradient;
    [SerializeField] AnimationCurve fogDensity;
    SeasonController season;

    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        season = GetComponent<SeasonController>();
        cam = Camera.main;
        // Set the fog color to be blue
        RenderSettings.fogColor = fogGradient.Evaluate(season.season) ;
        RenderSettings.fogDensity = fogDensity.Evaluate(season.season);
        cam.backgroundColor = fogGradient.Evaluate(season.season);
    }

    // Update is called once per frame

    void UpdateSeason(float s)
    {
        RenderSettings.fogColor = fogGradient.Evaluate(s);
        RenderSettings.fogDensity = fogDensity.Evaluate(s);

        cam.backgroundColor = fogGradient.Evaluate(season.season);
    }
}
