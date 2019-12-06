using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchRollIndicator : MonoBehaviour
{

    [SerializeField] Transform player;
    Quaternion localOrigin;
    // Start is called before the first frame update
    void Start()
    {
        localOrigin = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        float x = player.eulerAngles.x;
        float z = player.eulerAngles.z;

        transform.localEulerAngles = new Vector3(x, -z, 0);
    }
}
