using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookController : MonoBehaviour
{
    [SerializeField] float speed = 1f;
    [SerializeField] float returnSpeed = 0.01f;

    bool isHoming = false;
    Quaternion originalRotation;
    private void Start()
    {
        originalRotation = transform.localRotation;
    }

    private void LateUpdate()
    {
        if (isHoming)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, originalRotation, returnSpeed);
        }
    }
    public void RotateCamera(Vector3 rotation)
    {
        transform.Rotate(transform.parent.up * rotation.x, Space.World);
        transform.Rotate(Vector3.left * rotation.y, Space.Self);
    }

    public void SetHoming(bool b)
    {
        isHoming = b;
    }
}
