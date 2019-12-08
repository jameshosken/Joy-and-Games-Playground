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

    
    //public static List<Vector3> ConstructVerticesFromVoxelMap(Mesh mesh, float[,,] map, float threshold, int lod, Vector3 offset)
    //{

    //    //This can be done on a thread!
        
    //    //return vertices;



    //}

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



