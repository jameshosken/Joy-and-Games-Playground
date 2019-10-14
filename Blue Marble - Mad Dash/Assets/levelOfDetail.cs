using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelOfDetail : MonoBehaviour
{
    [SerializeField] float dist = 100f;
    Transform player;
    MeshRenderer mr;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        mr = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {   
        if(mr.enabled == false && Vector3.Distance(player.position, transform.position) < dist){
            mr.enabled = true;
        }

        if (mr.enabled == true && Vector3.Distance(player.position, transform.position) > dist)
        {
            mr.enabled = false;
        }
    }
}
