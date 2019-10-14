using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookController : MonoBehaviour
{

    [SerializeField] float speed = 0.1f;
    Quaternion originial;
    // Start is called before the first frame update
    void Start()
    {
        originial = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            float v = Input.GetAxis("Mouse Y") * -1f;

            float h = Input.GetAxis("Mouse X") ;

            transform.Rotate(Vector3.right, v, Space.Self);

            transform.Rotate(Vector3.up, h, Space.Self);
        }
        else
        {
            if(transform.localRotation != originial)
            {
                transform.localRotation = Quaternion.Slerp(transform.localRotation, originial, speed);
            }
        }
    }
}
