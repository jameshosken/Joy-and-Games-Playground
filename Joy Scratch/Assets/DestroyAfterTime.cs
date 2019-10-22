using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] float delay = 5f;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Kill", delay);
    }

    void Kill()
    {
        GameObject.Destroy(gameObject);
    }

}