using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raindrops : MonoBehaviour
{

    [SerializeField] GameObject drop;
    [SerializeField] float chance = 0.1f;
    [SerializeField] float bounds = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        if (UnityEngine.Random.Range(0f, 100f) < chance)
        {
            Vector3 angle = Random.insideUnitSphere * Random.Range(0, bounds);
            Vector3 pos = new Vector3(angle.x * 0.1f, angle.y, angle.z * 2);
            pos += transform.position;
            GameObject cln = Instantiate(drop, pos, Quaternion.identity);
        }
    }
}
