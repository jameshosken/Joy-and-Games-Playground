using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HyperspaceIndicator : MonoBehaviour
{
    SpaceshipController controller;
    Text text;
    // Start is called before the first frame update
    void Start()
    {
        controller = FindObjectOfType<SpaceshipController>();
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.enabled = controller.isReadyToPunch;
    }
}
