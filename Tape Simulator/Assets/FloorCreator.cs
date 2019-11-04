using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCreator : MonoBehaviour
{

    [SerializeField] GameObject tile;
    [SerializeField] int size;
    // Start is called before the first frame update
    void Start()
    {
        for(int x = -size; x < size; x++)
        {
            for (int z = -size; z < size; z++)
            {

                GameObject t = Instantiate(tile, new Vector3(x, transform.position.y, z), Quaternion.identity);
                t.transform.parent = transform.parent;

            }
        }
    }


}
