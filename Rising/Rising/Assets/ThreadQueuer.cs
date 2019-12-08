
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class ThreadQueuer : MonoBehaviour
{
    // Start is called before the first frame update

    List<Action> MainThreadFunctions = new List<Action>();

    private void Update()
    {
        while(MainThreadFunctions.Count > 0)
        {
            Action func = MainThreadFunctions[0];
            MainThreadFunctions.RemoveAt(0);
            try
            {
                func();
            }
            catch
            {
                Debug.Log("WARNING: Thread function did not execute. Possible chunk remaining");
            }

        }
    }

    public void StartThreadedFunction(Action func)
    {
        Thread t = new Thread(new ThreadStart(func));
        t.Start(); 
    }

    public void QueueMainThreadFunction(Action func)
    {
        MainThreadFunctions.Add(func);
    }



}
