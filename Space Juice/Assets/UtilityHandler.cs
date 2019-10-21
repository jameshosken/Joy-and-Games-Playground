using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState =  CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            if (Cursor.visible)
            {
                Cursor.visible = false;

                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {

                Cursor.visible = true;

                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

}
