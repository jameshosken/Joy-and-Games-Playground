using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereBehaviour : MonoBehaviour
{

    [SerializeField] Texture2D[] textures;
    Renderer renderer;

        // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
        Texture2D tex = GetRandomTexture();
        renderer.material.mainTexture = tex;
        //renderer.material.SetTexture("_BumpMap", tex);
        renderer.material.SetTexture("_HeightMap", tex);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Texture2D GetRandomTexture()
    {
        int choice = Mathf.FloorToInt(UnityEngine.Random.Range(0, textures.Length));

        return textures[choice];
    }
}
