using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] GameObject worldChunk;

    List<string> names = new List<string>();
    
    // Start is called before the first frame update
    void Start()
    {
        GetNames();
        GenerateWorld();
    }

    void GenerateWorld()
    {
        Debug.Log("Generating World");

        for (int i = 0; i < names.Count; i++)
        {
            string name = names[i];
            Mesh chunkMesh = Resources.Load("WorldData/" + name) as Mesh;
            chunkMesh.RecalculateNormals();
            chunkMesh.RecalculateTangents();
            GameObject chunk = Instantiate(worldChunk);
            MeshFilter filter = chunk.GetComponent<MeshFilter>();
            MeshCollider col = chunk.GetComponent<MeshCollider>();
            chunk.transform.parent = gameObject.transform;
            chunk.transform.localPosition = Vector3.zero;
            chunk.transform.localScale = Vector3.one;
            filter.mesh = chunkMesh;
            col.sharedMesh = chunkMesh;
            chunk.name = name;
        }
    }

    void GetNames()
    {
        Debug.Log("Getting Names");
        DirectoryInfo exports = new DirectoryInfo("Assets/Resources/WorldData");
        FileInfo[] fileInfo = exports.GetFiles("*.asset");
        for(int i = 0; i < fileInfo.Length; i++)
        {
            FileInfo cube = fileInfo[i];
            string name = cube.Name;
            name = name.Replace(".asset", "");
            //Debug.Log(name);
            names.Add(name);

        }
    }
}
