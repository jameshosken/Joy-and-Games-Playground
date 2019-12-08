using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarpoonHandler : MonoBehaviour
{
    [SerializeField] float lifetime = 7f;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Die", lifetime);
    }

    void Die()
    {
        
        GameObject.Destroy(gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CreatureHitDetector>())
        {
            //Hit Creature :(
            other.GetComponent<CreatureHitDetector>().Hit();
        }
    }

}
