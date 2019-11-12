using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree_SeasonController : MonoBehaviour
{

    float summerToFall = 0;
    float fallToWinter = 0;
    float winterToSpring = 0;
    float springToSummer = 0;


    ParticleSystem leavesSystem;
    ParticleSystem.EmitParams leafParams;
    ParticleSystemRenderer renderer;

    AnimationCurve curve;
    private void Start()
    {
        leavesSystem = GetComponent<ParticleSystem>();

        renderer = leavesSystem.GetComponent<ParticleSystemRenderer>();

        //Random curve for transitions
        curve = AnimationCurve.EaseInOut(UnityEngine.Random.Range(0, .4f), 0, UnityEngine.Random.Range(.6f, 1f), 1);
    }

    

    public void SetSeasonColours(Vector4 colours)
    {

        summerToFall = curve.Evaluate(colours.x);
        fallToWinter = curve.Evaluate(colours.y);
        winterToSpring = curve.Evaluate(colours.z);
        springToSummer = curve.Evaluate(colours.w);

        ParticleSystem.Particle[] leaves = new ParticleSystem.Particle[leavesSystem.particleCount];
        leavesSystem.GetParticles(leaves);
        renderer.material.SetFloat("_SummerFall", summerToFall);
        renderer.material.SetFloat("_FallWinter", fallToWinter);
        renderer.material.SetFloat("_WinterSpring", winterToSpring);
        renderer.material.SetFloat("_SpringSummer", springToSummer);
    }
}
