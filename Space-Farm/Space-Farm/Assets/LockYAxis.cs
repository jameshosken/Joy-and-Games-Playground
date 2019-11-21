using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockYAxis : MonoBehaviour
{
    Transform parent;
    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 tempRot = transform.localRotation.eulerAngles;

        transform.eulerAngles = parent.eulerAngles;

    }
}
