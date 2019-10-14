using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameController : MonoBehaviour
{
    [SerializeField] int winIndex = 3;
    [SerializeField] int loseIndex = 2;
    ShipObjectPlacementScript ship;
    OxygenTimer o2Timer;
    LoadLevel levelLoader;
    // Start is called before the first frame update
    void Start()
    {
        ship = FindObjectOfType<ShipObjectPlacementScript>();
        o2Timer = FindObjectOfType<OxygenTimer>();
        levelLoader = GetComponent<LoadLevel>();
    }

    // Update is called once per frame
    void Update()
    {
        if(ship.returnedItems.Count == 3)
        {
            Win();
        }

        if(o2Timer.gameTime <= 0)
        {
            Lose();
        }
    }

    private void Lose()
    {
        levelLoader.ActivateLevelLoadByIndex(loseIndex);
    }

    private void Win()
    {
        levelLoader.ActivateLevelLoadByIndex(winIndex);
    }
}
