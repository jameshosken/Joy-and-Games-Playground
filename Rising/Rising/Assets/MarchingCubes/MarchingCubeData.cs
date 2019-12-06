using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingCubeData : MonoBehaviour
{
    Renderer renderer;
    Material material;

    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    public void SetColour(Color col)
    {
        renderer = GetComponent<Renderer>();
        material = renderer.materials[0];
        material.SetColor("_BaseColor", col);
    }

    
}
