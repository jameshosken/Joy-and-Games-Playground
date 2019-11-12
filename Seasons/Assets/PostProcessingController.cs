using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
public class PostProcessingController : MonoBehaviour
{
    [SerializeField] PostProcessVolume volume;
    [SerializeField] AnimationCurve bloomCurve;
    [SerializeField] AnimationCurve saturationCurve;

    Bloom bloomLayer = null;
    AmbientOcclusion ambientOcclusionLayer = null;
    ColorGrading colorGradingLayer = null;


    SeasonController season;
    // Start is called before the first frame update
    void Start()
    {
        season = GetComponent<SeasonController>();

        volume.profile.TryGetSettings(out bloomLayer);
        volume.profile.TryGetSettings(out ambientOcclusionLayer);
        volume.profile.TryGetSettings(out colorGradingLayer);


    }


    void UpdateSeason(float s)
    {
        bloomLayer.intensity.value = bloomCurve.Evaluate(s);
        colorGradingLayer.saturation.value = saturationCurve.Evaluate(s) * 50f;
    }
}
