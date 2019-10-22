using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereManager : MonoBehaviour
{

    [SerializeField] GameObject spherePrefab;
    [SerializeField] float interval = 1f;
    [SerializeField] float radius;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("CreateSphere", 0, interval);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateSphere()
    {
        GameObject clone = Instantiate(spherePrefab, transform.position + UnityEngine.Random.insideUnitSphere * radius, Quaternion.identity);
        clone.name = "Clone + " + Time.time;
    }
}
