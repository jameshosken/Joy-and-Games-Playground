using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeasonController : MonoBehaviour
{
    [SerializeField] float speed = 0.0033f;
    [Range(0,1)]
    public float season;
    float prevSeason = 0;
    // Start is called before the first frame update

    private void Update()
    {
        season = (season + speed * Time.deltaTime) % 1;


        if(season != prevSeason)
        {
            prevSeason = season;
            BroadcastMessage("UpdateSeason", season);
            //Debug.Log("Broadcasting");
        }
    }

    //public float GetSeason()
    //{
    //    return season;
    //}
}
