using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{

    [SerializeField] Text invText;
    Item hold = Item.EMPTY;

    // Start is called before the first frame update
    void Start()
    {
        invText.text = "Inventory: " + hold.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool SetItem(Item newItem)
    {
        if (hold == Item.EMPTY)
        {
            hold = newItem;
            invText.text = "Inventory: " + hold.ToString();
            return true;
        }
        else
        {
            Debug.Log("ERROR: HOLD IS FULL");   // TODO replace with UI error.
            return false;
        }

    }
    public void SetEmpty()
    {

        hold = Item.EMPTY;
        invText.text = "Inventory: " + hold.ToString();
    }
    

    public Item GetItem()
    {
        return hold;
    }
}

public enum Item { CORE, ENGINE, FINS, EMPTY}