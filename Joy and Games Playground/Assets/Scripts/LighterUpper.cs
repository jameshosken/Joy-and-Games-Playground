using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LighterUpper : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.collider.gameObject;

        MeshRenderer otherRenderer = other.GetComponent<MeshRenderer>();

        Color col = otherRenderer.material.GetColor("_EmissionColor");

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material.SetColor("_EmissionColor", col);

    }
}
