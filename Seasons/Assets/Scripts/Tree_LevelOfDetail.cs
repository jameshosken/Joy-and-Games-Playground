using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree_LevelOfDetail : MonoBehaviour
{

    [SerializeField] float enableView = 50f;
    [SerializeField] float enableLeaves = 30f;
    // Start is called before the first frame update
    Renderer renderer;
    ParticleSystem leaves;
    ParticleSystemRenderer leavesRenderer;
    Camera cam;


    void Start()
    {
        renderer = GetComponent<Renderer>();
        leavesRenderer = GetComponent<ParticleSystemRenderer>();
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(cam.gameObject.transform.position, transform.position);
        // Handle exit
        if (dist > enableView && renderer.enabled)
        {
            renderer.enabled = false;
        }

        if (dist > enableLeaves && leavesRenderer.enabled)
        {
            leavesRenderer.enabled = false;
        }

        //Handle Entry
        if (dist < enableView && !renderer.enabled)
        {
            renderer.enabled = true;
        }

        if (dist < enableLeaves && !leavesRenderer.enabled)
        {
            leavesRenderer.enabled = true;
        }
    }
}
