using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MarchingCubes
{

    private static float NORMAL_SMOOTHNESS = 1.0f;
    
    // vertices is list of vertices for mesh
    // map is 3D array of floats from 0-1
    // threshold is point at which a vertex is added
    // lod is how detailed to be (0 == most detailed)
    // offset is offset of chunk in space (for larger/endless maps)

    

    public static void ConstructMeshFromVoxelMap(Mesh mesh, float[,,] map, float threshold, int lod, Vector3 offset)
    {
        List<Vector3> vertices = new List<Vector3>();

        int mul = (int)Mathf.Pow(2, lod);

        Vector3[] corners = new Vector3[8];
        Vector3[] vertList = new Vector3[12];
        float[] cornerValues = new float[8];


        //Try +1 to add the edges between chunks


        int[] mapSize = { map.GetLength(0), map.GetLength(1), map.GetLength(2) };

        for (int i = 0; i < corners.Length; i++)
        {
            corners[i] = new Vector3();
        }

        //Loop through each dimension of the map
        for (int x = 0; x < mapSize[0] - 1; x += mul)
        {
            for (int y = 0; y < mapSize[1] - 1; y += mul)
            {
                for (int z = 0; z < mapSize[2] - 1; z += mul)
                {
                    int cubeindex = 0;

                    corners[0] = new Vector3(x, y, z);
                    corners[1] = new Vector3(x + mul, y, z);
                    corners[2] = new Vector3(x + mul, y, z + mul);
                    corners[3] = new Vector3(x, y, z + mul);
                    corners[4] = new Vector3(x, y + mul, z);
                    corners[5] = new Vector3(x + mul, y + mul, z);
                    corners[6] = new Vector3(x + mul, y + mul, z + mul);
                    corners[7] = new Vector3(x, y + mul, z + mul);

                    for (int i = 0; i < corners.Length; i++)
                    {
                        cornerValues[i] = map[(int)corners[i].x, (int)corners[i].y, (int)corners[i].z];
                    }

                    //Add bitwise value to cubeIndex (for lookup)
                    if (cornerValues[0] < threshold) cubeindex |= 1;
                    if (cornerValues[1] < threshold) cubeindex |= 2;
                    if (cornerValues[2] < threshold) cubeindex |= 4;
                    if (cornerValues[3] < threshold) cubeindex |= 8;
                    if (cornerValues[4] < threshold) cubeindex |= 16;
                    if (cornerValues[5] < threshold) cubeindex |= 32;
                    if (cornerValues[6] < threshold) cubeindex |= 64;
                    if (cornerValues[7] < threshold) cubeindex |= 128;

                    // no triangles if it is surrounded by air or surrounded by blocks
                    if (MarchTable.edges[cubeindex] == 0 || MarchTable.edges[cubeindex] == 255) continue;

                    // Find the vertices where the surface intersects the cube
                    if ((MarchTable.edges[cubeindex] & 1) == 1)
                        vertList[0] = vertexInterpolation(threshold, corners[0], corners[1], cornerValues[0], cornerValues[1]);
                    if ((MarchTable.edges[cubeindex] & 2) == 2)
                        vertList[1] = vertexInterpolation(threshold, corners[1], corners[2], cornerValues[1], cornerValues[2]);
                    if ((MarchTable.edges[cubeindex] & 4) == 4)
                        vertList[2] = vertexInterpolation(threshold, corners[2], corners[3], cornerValues[2], cornerValues[3]);
                    if ((MarchTable.edges[cubeindex] & 8) == 8)
                        vertList[3] = vertexInterpolation(threshold, corners[3], corners[0], cornerValues[3], cornerValues[0]);
                    if ((MarchTable.edges[cubeindex] & 16) == 16)
                        vertList[4] = vertexInterpolation(threshold, corners[4], corners[5], cornerValues[4], cornerValues[5]);
                    if ((MarchTable.edges[cubeindex] & 32) == 32)
                        vertList[5] = vertexInterpolation(threshold, corners[5], corners[6], cornerValues[5], cornerValues[6]);
                    if ((MarchTable.edges[cubeindex] & 64) == 64)
                        vertList[6] = vertexInterpolation(threshold, corners[6], corners[7], cornerValues[6], cornerValues[7]);
                    if ((MarchTable.edges[cubeindex] & 128) == 128)
                        vertList[7] = vertexInterpolation(threshold, corners[7], corners[4], cornerValues[7], cornerValues[4]);
                    if ((MarchTable.edges[cubeindex] & 256) == 256)
                        vertList[8] = vertexInterpolation(threshold, corners[0], corners[4], cornerValues[0], cornerValues[4]);
                    if ((MarchTable.edges[cubeindex] & 512) == 512)
                        vertList[9] = vertexInterpolation(threshold, corners[1], corners[5], cornerValues[1], cornerValues[5]);
                    if ((MarchTable.edges[cubeindex] & 1024) == 1024)
                        vertList[10] = vertexInterpolation(threshold, corners[2], corners[6], cornerValues[2], cornerValues[6]);
                    if ((MarchTable.edges[cubeindex] & 2048) == 2048)
                        vertList[11] = vertexInterpolation(threshold, corners[3], corners[7], cornerValues[3], cornerValues[7]);

                    for (int i = 0; MarchTable.triangles[cubeindex][i] != -1; i += 3)
                    {
                        //vertices.Add(createVertex(chunk, mul, vertList[MarchTable.triangles[cubeindex][i + 2]]));
                        //vertices.Add(createVertex(chunk, mul, vertList[MarchTable.triangles[cubeindex][i + 1]]));
                        //vertices.Add(createVertex(chunk, mul, vertList[MarchTable.triangles[cubeindex][i]]));

                        vertices.Add(createVertex(mapSize, offset, vertList[MarchTable.triangles[cubeindex][i + 2]]));
                        vertices.Add(createVertex(mapSize, offset, vertList[MarchTable.triangles[cubeindex][i + 1]]));
                        vertices.Add(createVertex(mapSize, offset, vertList[MarchTable.triangles[cubeindex][i]]));

                    }


                }
            }
        }


        int[] tris = new int[vertices.Count];
        for (int i = 0; i < vertices.Count; i++)
        {
            tris[i] = i;
        }

        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateNormals();


    }

    public static Vector3 createVertex(int[] mapSize, Vector3 offset, Vector3 position)
    {

        Vector3 vertex = new Vector3(
            position.x + offset.x,
            position.y + offset.y,
            position.z + offset.z);
        return vertex;
    }



    public static Vector3 vertexInterpolation(float threshold, Vector3 p1, Vector3 p2, float valp1, float valp2)
    {

        Vector3 p = new Vector3();
        float mu = (threshold - valp1) / (valp2 - valp1);
        p.x = p1.x + mu * (float)(p2.x - p1.x);
        p.y = p1.y + mu * (float)(p2.y - p1.y);
        p.z = p1.z + mu * (float)(p2.z - p1.z);

        return p;
    }

}



