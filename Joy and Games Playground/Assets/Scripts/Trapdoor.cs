using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trapdoor : MonoBehaviour
{

    [ColorUsage(true, true)]
    [SerializeField] Color activationColour;

    [SerializeField] GameObject activationObject;
    [SerializeField] Trapdoor otherDoor;

    public bool isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isActive && otherDoor.isActive)
        {
            ActivateHinge();
        }
    }

    void ActivateHinge()
    {
        //isActive = false;

        GetComponent<Rigidbody>().isKinematic = false;
    }

    public void Activate(GameObject other)
    {
        if (other == activationObject)
        {
            if (!isActive)
            {
                MeshRenderer mesh = GetComponent<MeshRenderer>();
                Material mat = mesh.materials[0];
                Debug.Log("TRIG: Setting Mat");
                mat.SetColor("_EmissionColor", activationColour);

                isActive = true;
            }
        }
    }





}
