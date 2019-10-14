using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipObjectPlacementScript : MonoBehaviour
{


    [SerializeField] GameObject core;
    [SerializeField] GameObject engine;
    [SerializeField] GameObject fins;


    public List<Item> returnedItems = new List<Item>();

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

            Item returnedItem = inv.GetItem();

            switch (returnedItem)
            {
                case Item.EMPTY:
                    // Do nothing
                    break;
                case Item.CORE:
                    core.SetActive(true);
                    returnedItems.Add(returnedItem);
                    break;
                case Item.ENGINE:
                    engine.SetActive(true);
                    returnedItems.Add(returnedItem);
                    break;
                case Item.FINS:
                    fins.SetActive(true);
                    returnedItems.Add(returnedItem);
                    break;
                default:
                    break;
            }

            inv.SetEmpty();
        }
    }
}
