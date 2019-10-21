using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGCreator : MonoBehaviour
{

    [SerializeField] GameObject cube;
    [SerializeField] int numObjects = 10;
    [SerializeField] Vector3 bounds;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < numObjects; i++)
        {
            Vector3 posOffset = Vector3.Scale(Random.insideUnitSphere, bounds);
            Instantiate(cube, transform.position + posOffset, Random.rotation, transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
