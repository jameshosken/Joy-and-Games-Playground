using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapdoorTrigger : MonoBehaviour
{

    [SerializeField] Trapdoor trap1;
    [SerializeField] Trapdoor trap2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        trap1.Activate(other.gameObject);
        trap2.Activate(other.gameObject);
    }
}
