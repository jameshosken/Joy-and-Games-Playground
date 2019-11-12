using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forest_SeasonController : MonoBehaviour
{
    [Range(0,1)]
    float season;

    float prevSeason;
    ForestGenerator generator;


    
    void Start()
    {
        season = 0;
        generator = GetComponent<ForestGenerator>();
    }

    //void Update()
    //{
    //    if(prevSeason != season)
    //    {
    //        UpdateSeasons();
    //        prevSeason = season;
    //    }
    //    season = seasonController.GetSeason();


    //}

    void UpdateSeason(float s)
    {
        season = s;
        UpdateSeasons();
    }

    private void UpdateSeasons()
    {

        float x = Mathf.Clamp01(Remap(season, 0, 1, 0, 4));

        float y = Mathf.Clamp01(Remap(season, 0, 1, -1, 3));

        float z = Mathf.Clamp01(Remap(season, 0, 1, -2, 2));

        float w = Mathf.Clamp01(Remap(season, 0, 1, -3, 1));

        for(int i = 0; i < generator.GetTrees().Count; i++)
        {
            generator.GetTrees()[i].GetComponent<Tree_SeasonController>().SetSeasonColours(new Vector4(x, y, z, w));
        }
    }

    float Remap(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }

}
