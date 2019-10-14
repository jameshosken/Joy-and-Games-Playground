using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartWithRandomRotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Vector3 rot = new Vector3(
            Random.Range(-180, 180),
            Random.Range(-180, 180),
            Random.Range(-180, 180));

        transform.Rotate(rot);
        transform.localScale = Vector3.one * Random.Range(.1f, 2)/transform.parent.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
