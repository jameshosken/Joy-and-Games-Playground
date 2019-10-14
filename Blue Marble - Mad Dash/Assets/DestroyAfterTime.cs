using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] float time;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Die", time);
    }

    void Die()
    {
        GameObject.Destroy(gameObject);
    }
}
