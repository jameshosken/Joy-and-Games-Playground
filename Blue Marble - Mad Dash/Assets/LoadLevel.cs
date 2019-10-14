using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    [SerializeField] int levelToLoad; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateLevelLoad()
    {
        SceneManager.LoadScene(levelToLoad);
    }

    public void ActivateLevelLoadByIndex(int i)
    {
        SceneManager.LoadScene(i);
    }


}
