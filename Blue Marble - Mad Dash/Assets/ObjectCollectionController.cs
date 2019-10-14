using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCollectionController : MonoBehaviour
{
    [SerializeField] Item itemType;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Inventory inv = other.GetComponent<Inventory>();
            if (inv.SetItem(itemType))
            {
                GameObject.Destroy(gameObject);
            }
            else
            {
                //Object not added.
            }
        }
    }
}
