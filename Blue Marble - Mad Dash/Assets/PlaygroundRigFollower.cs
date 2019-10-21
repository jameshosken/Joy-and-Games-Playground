using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaygroundRigFollower : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float followSpeed;
    [SerializeField] float rotateSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, followSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, rotateSpeed);
    }
}
