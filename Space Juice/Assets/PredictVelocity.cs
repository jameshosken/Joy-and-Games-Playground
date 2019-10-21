using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredictVelocity : MonoBehaviour
{
    [SerializeField] Rigidbody player;
    [SerializeField] float offset = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localPosition = Vector3.zero;

        transform.Translate(player.velocity * offset, Space.World);
    }
}
