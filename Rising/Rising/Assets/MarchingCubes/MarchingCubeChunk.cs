using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MarchingCubeChunk : MonoBehaviour
{
    [SerializeField] int LOD = 0;

    Mesh mesh;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;
    Vector3 worldPosition;

    float[,,] mapChunk;

    string myName;

    public void SetChunkData()
    {
        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;
        meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = meshFilter.sharedMesh;
    }
    

    public void AddMapChunk(float[,,] globalMap, Vector3 offsetPosition, Vector3 chunkSize)
    {
        Debug.Log("World Position:");
        Debug.Log(offsetPosition);
        worldPosition = offsetPosition;

        myName = "Cube-" + worldPosition.x.ToString() + "-" + worldPosition.y.ToString() + "-" + worldPosition.z.ToString();

        //Set size of chunk. 
        //Chunk is +1 from chunksize to fill in gaps between chunks.

        int sizeX = (int)chunkSize.x;

        if(offsetPosition.x + chunkSize.x < globalMap.GetLength(0))
        {
            sizeX += 1;
        }

        int sizeY = (int)chunkSize.y;

        if (offsetPosition.y + chunkSize.y < globalMap.GetLength(1))
        {
            sizeY += 1;
        }

        int sizeZ = (int)chunkSize.z;

        if (offsetPosition.z + chunkSize.z < globalMap.GetLength(2))
        {
            sizeZ += 1;
        }

        mapChunk = new float[sizeX, sizeY, sizeZ];

        for (int x = 0; x < mapChunk.GetLength(0); x++)
        {
            for (int y = 0; y < mapChunk.GetLength(1); y++)
            {
                for (int z = 0; z < mapChunk.GetLength(2); z++)
                {
                    mapChunk[x, y, z] = globalMap[
                        x + (int)worldPosition.x,
                        y + (int)worldPosition.y,
                        z + (int)worldPosition.z];
                }
            }
        }
    }

    public void UpdateMapChunk(float[,,] globalMap)
    {
        Debug.Log("World Position:");
        Debug.Log(worldPosition);

        for (int x = 0; x < mapChunk.GetLength(0); x++)
        {
            for (int y = 0; y < mapChunk.GetLength(1); y++)
            {
                for (int z = 0; z < mapChunk.GetLength(2); z++)
                {
                    mapChunk[x, y, z] = globalMap[
                        x + (int)worldPosition.x,
                        y + (int)worldPosition.y,
                        z + (int)worldPosition.z];
                }
            }
        }
    }

    public void UpdateMesh(float threshold)
    {
        //MarchingCubes.ConstructVerticesFromVoxelMap(mesh, mapChunk, threshold, LOD, worldPosition);
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = meshFilter.sharedMesh;

        AssetDatabase.CreateAsset( mesh, "Assets/MarchingCubes/Export/" + myName + ".asset" );
        AssetDatabase.SaveAssets();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
