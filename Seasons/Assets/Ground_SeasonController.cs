using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground_SeasonController : MonoBehaviour
{

    float summerToFall = 0;
    float fallToWinter = 0;
    float winterToSpring = 0;
    float springToSummer = 0;
    float season;

    Material mat;

    AnimationCurve curve;
    private void Start()
    {
        mat = GetComponent<Renderer>().materials[0];

        //Random curve for transitions
        curve = AnimationCurve.EaseInOut(UnityEngine.Random.Range(0, .2f), 0, UnityEngine.Random.Range(.8f, 1f), 1);
    }


    void UpdateSeason(float s)
    {
        season = s;
        float x = Mathf.Clamp01(Remap(season, 0, 1, 0, 4));

        float y = Mathf.Clamp01(Remap(season, 0, 1, -1, 3));

        float z = Mathf.Clamp01(Remap(season, 0, 1, -2, 2));

        float w = Mathf.Clamp01(Remap(season, 0, 1, -3, 1));

        SetSeasonColours(new Vector4(x, y, z, w));
    }

    public void SetSeasonColours(Vector4 colours)
    {

        summerToFall = curve.Evaluate(colours.x);
        fallToWinter = curve.Evaluate(colours.y);
        winterToSpring = curve.Evaluate(colours.z);
        springToSummer = curve.Evaluate(colours.w);


        mat.SetFloat("_SummerFall", summerToFall);
        mat.SetFloat("_FallWinter", fallToWinter);
        mat.SetFloat("_WinterSpring", winterToSpring);
        mat.SetFloat("_SpringSummer", springToSummer);
    }

    float Remap(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}
