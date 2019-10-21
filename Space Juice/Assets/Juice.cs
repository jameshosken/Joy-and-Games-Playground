using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Juice : MonoBehaviour
{

    //EXTERNAL COMPONENTS
    [SerializeField] PostProcessVolume outerPostProcess;
    [SerializeField] PostProcessVolume innerPostProcess;

    [SerializeField] Transform innerCamera;
    [SerializeField] Transform outerCamera;


    //TWEAKABLE PARAMS
    [SerializeField] float maxReverseDistortion = 0.1f;
    [SerializeField] float maxBloom = 10f;
    [SerializeField] float saturation = 5f;

    [SerializeField] AnimationCurve distortionCurve;

    [SerializeField] AnimationCurve fovCurve;

    [SerializeField] AnimationCurve scaleCurve;
    [SerializeField] AnimationCurve vignetteCurve;
    [SerializeField] AnimationCurve chromeCurve;

    //CAM SHAKE
    [SerializeField] float innerCameraShakeFrequency = .1f;
    [SerializeField] float innerCameraShakeAmplitude = 1f;

    [SerializeField] float outerCameraShakeFrequency = .5f;
    [SerializeField] float outerCameraShakeAmplitude = 1f;
    [SerializeField] AnimationCurve innerCameraShakeCurve;
    [SerializeField] AnimationCurve outerCameraShakeCurve;




    //SHIP EXTERIOR FX
    Bloom outerBloom = null;

    LensDistortion outerDistortion = null;
    ChromaticAberration outerChroma = null;

    Camera cam;
    Camera innerCam;


    //SHIP INTERIOR FX
    ChromaticAberration innerChroma = null;
    Vignette innerVignette = null;
    ColorGrading innerGrade = null;

    SpaceshipController controller;

    float innerCameraShakeNoiseSample = 0;
    float outerCameraShakeNoiseSample = 12.345f; // offset by an arbitrary amount

    void Start()
    {
        cam = outerCamera.GetComponent<Camera>();
        innerCam = innerCamera.GetComponent<Camera>();
        controller = GetComponent<SpaceshipController>();

        outerPostProcess.profile.TryGetSettings(out outerBloom);
        outerPostProcess.profile.TryGetSettings(out outerDistortion);
        outerPostProcess.profile.TryGetSettings(out outerChroma);
        innerPostProcess.profile.TryGetSettings(out innerChroma);
        innerPostProcess.profile.TryGetSettings(out innerVignette);

        innerPostProcess.profile.TryGetSettings(out innerGrade);


        outerBloom.enabled.value = true;
        outerBloom.intensity.value = 1;

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = controller.getVelocity();

        float alignmentToVelocity = Vector3.Dot(velocity.normalized, transform.forward); // 1 = fully aligned, -1 = opposite, 

        alignmentToVelocity = Mathf.Clamp(alignmentToVelocity, maxReverseDistortion, 1);

        //Map our speed to the distortion in the skybox:
        float normalisedVelocity = alignmentToVelocity * Utilitiy.Remap(velocity.magnitude, 0, controller.maxVelocity, 0, 1);

        float fovCurveEval = fovCurve.Evaluate(normalisedVelocity);

        float distortion = distortionCurve.Evaluate(normalisedVelocity) * 100;
        float FOV = Utilitiy.Remap(fovCurveEval, 0, 1, 60, 179);
        float innerFOV = Utilitiy.Remap(fovCurveEval, 0, 1, 60, 75);
        float scale = scaleCurve.Evaluate(normalisedVelocity) ;
        float vignette = vignetteCurve.Evaluate(normalisedVelocity);
        float chroma = chromeCurve.Evaluate(normalisedVelocity);


        outerBloom.intensity.value =  Utilitiy.Remap(distortionCurve.Evaluate(normalisedVelocity), 0, 1, 1, maxBloom);
        outerDistortion.intensity.value = distortion;
        outerDistortion.scale.value = scale;
        innerVignette.intensity.value = vignette;
        innerChroma.intensity.value = chroma;
        outerChroma.intensity.value = chroma;
        cam.fieldOfView = FOV;
        innerCam.fieldOfView = innerFOV;
        innerGrade.gain.value = Vector4.Lerp(new Vector4(1, 1, 1, .2f), new Vector4(.7f, .7f, 1, -.5f), fovCurveEval);

        //Use the camera shake noise curve to find the noise point to evaluate
        innerCameraShakeNoiseSample += innerCameraShakeCurve.Evaluate(normalisedVelocity) * innerCameraShakeFrequency;

        Vector3 innerCamRotation = Vector3.zero;

        //Add velocity shake:
        innerCamRotation += new Vector3(
                Mathf.PerlinNoise(innerCameraShakeNoiseSample, 0) * 2 - 1,
                Mathf.PerlinNoise(0, innerCameraShakeNoiseSample) * 2 - 1,
                Mathf.PerlinNoise(innerCameraShakeNoiseSample, innerCameraShakeNoiseSample) * 2 - 1
            ) * innerCameraShakeAmplitude;


        innerCamRotation +=
        new Vector3(
            Mathf.PerlinNoise(innerCameraShakeNoiseSample / 10, 0) * 2 - 1,
            Mathf.PerlinNoise(0, innerCameraShakeNoiseSample / 10) * 2 - 1,
            Mathf.PerlinNoise(innerCameraShakeNoiseSample / 10, innerCameraShakeNoiseSample) * 2 - 1
        ) * innerCameraShakeAmplitude / 10 * controller.getAcceleration();


        innerCamera.localEulerAngles = innerCamRotation;

        //Use the camera shake noise curve to find the noise point to evaluate
        outerCameraShakeNoiseSample += outerCameraShakeCurve.Evaluate(normalisedVelocity) * outerCameraShakeFrequency;

        Vector3 outerCamRotation = Vector3.zero;

        //Add velocity shake:
        outerCamRotation += new Vector3(
                Mathf.PerlinNoise(outerCameraShakeNoiseSample, 0) * 2 - 1,
                Mathf.PerlinNoise(0, outerCameraShakeNoiseSample) * 2 - 1,
                Mathf.PerlinNoise(outerCameraShakeNoiseSample, outerCameraShakeNoiseSample) * 2 - 1
            ) * outerCameraShakeAmplitude;


        outerCamRotation +=
        new Vector3(
            Mathf.PerlinNoise(outerCameraShakeNoiseSample / 10, 0) * 2 - 1,
            Mathf.PerlinNoise(0, outerCameraShakeNoiseSample / 10) * 2 - 1,
            Mathf.PerlinNoise(outerCameraShakeNoiseSample / 10, outerCameraShakeNoiseSample) * 2 - 1
        ) * outerCameraShakeAmplitude / 10 * controller.getAcceleration();


        outerCamera.localEulerAngles = outerCamRotation;


    }
}
