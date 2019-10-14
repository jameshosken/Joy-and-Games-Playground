using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassPopulator : MonoBehaviour
{
    [SerializeField] GameObject[] grassTamplates;
    [SerializeField] int grassNum;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 center = transform.position;
        float radius = transform.localScale.x-0.01f;
        for (int i = 0; i < grassNum; i++)
        {
            Vector3 pos = Random.onUnitSphere * radius + center;
            Quaternion rot = Quaternion.LookRotation(pos - center);

            GameObject choice = grassTamplates[Mathf.FloorToInt(Random.Range(0, grassTamplates.Length))];
            GameObject grass = Instantiate(choice, pos, rot);
            grass.transform.parent = transform;
        }   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
