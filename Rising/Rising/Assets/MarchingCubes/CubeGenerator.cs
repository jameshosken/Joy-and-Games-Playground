using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CubeGenerator : MonoBehaviour
{ 

    public void CreateCube(float x, float y, float z)
    {
        Vector3[] vertices = {

            new Vector3 (x+0, y+0, z+0),

            new Vector3 (x+1, y+0, z+0),

            new Vector3 (x+1, y+1, z+0),

            new Vector3 (x+0, y+1, z+0),

            new Vector3 (x+0, y+1, z+1),

            new Vector3 (x+1, y+1, z+1),

            new Vector3 (x+1, y+0, z+1),

            new Vector3 (x+0, y+0, z+1),

        };



        int[] triangles = {

            0, 2, 1, //face front

			0, 3, 2,

            2, 3, 4, //face top

			2, 4, 5,

            1, 2, 5, //face right

			1, 5, 6,

            0, 7, 4, //face left

			0, 4, 3,

            5, 4, 7, //face back

			5, 7, 6,

            0, 6, 7, //face bottom

			0, 1, 6

        };



        Mesh mesh = GetComponent<MeshFilter>().mesh;

        mesh.Clear();
        
        mesh.vertices = vertices;

        mesh.triangles = triangles;

        mesh.Optimize();

        mesh.RecalculateNormals();

    }

}