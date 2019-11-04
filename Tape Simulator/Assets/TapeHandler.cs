using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapeHandler : MonoBehaviour
{

    [SerializeField] Transform peelTransform; // Point where tape peels off roll

    [SerializeField] float tapeFidelity = 10f; //Distance travelled before new segment of tape is added
    [SerializeField] float thickness = 0.1f;

    [SerializeField] float rotationSensitivity = 3f;

    bool isDispensing = false;

    TapeSegmentCreator tapeSegmentCreator;

    List<GameObject> tapeSegments = new List<GameObject>();


    [SerializeField] MeshFilter filter;
    Mesh tapeMesh;
    List<Vector3> vertices = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        tapeMesh = new Mesh();
        filter.mesh = tapeMesh;
        tapeSegmentCreator = GetComponent<TapeSegmentCreator>();
        Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Tape"));

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButton(0))
        {
            transform.Rotate(Vector3.forward * rotationSensitivity, Space.Self);
        }
        if (Input.GetMouseButton(1))
        {
            transform.Rotate(Vector3.forward * -rotationSensitivity, Space.Self);
        }

        if (!getLastSegment()) {
            return;
        }

        float dist = Vector3.Distance(peelTransform.position, getLastSegment().transform.position);

        if (dist > tapeFidelity)
        {
            DispenseTape();
        }

        UpdateSegmentPositions();

    }

    private void DispenseTape()
    {
        GameObject newSegment = tapeSegmentCreator.CreateNewSegment(getLastSegment(), peelTransform);
        tapeSegments.Add(newSegment);
        AddSegmentToMesh(newSegment);
    }


    void UpdateSegmentPositions()
    {
        tapeMesh.GetVertices(vertices);

        Vector3 pos;
        Vector3 right;

        for (int i = 0; i < tapeSegments.Count; i++)
        {
            pos = tapeSegments[i].transform.position;
            right = tapeSegments[i].transform.right * thickness;

            vertices[2*i] = pos + right;
            vertices[2 * i + 1] = pos - right;
        }

        pos = peelTransform.position;
        right = peelTransform.right * thickness;
        vertices[vertices.Count - 1] = pos + right;
        vertices[vertices.Count - 2] = pos - right;

        tapeMesh.SetVertices(vertices);
        tapeMesh.RecalculateNormals();
    }
    private void AddSegmentToMesh(GameObject newSegment)
    {
        Debug.Log("Adding to Mesh");

        Vector3 a, b, c, d;

    
        tapeMesh.GetVertices(vertices); // store all vertices
        int numVerts = vertices.Count;

        if (tapeSegments.Count <= 1) //In case of first square:
        {
            a = peelTransform.position + peelTransform.right * thickness;
            b = peelTransform.position - peelTransform.right * thickness;
            vertices.Add(a);
            vertices.Add(b);
        }
        else
        {
            //Get last vertices loaded into mesh
            a = vertices[vertices.Count - 1];
            b = vertices[vertices.Count - 2];
        }


        int[] currentTris = tapeMesh.GetTriangles(0);
        int currentLen = currentTris.Length;
        int aIndex = numVerts - 2;                  // Current index of starting vertex

        int[] newTris = new int[currentLen + 6];    // Add 6 new tris to mesh;

        for (int i = 0; i < currentLen; i++)        // Copy old values;
        {
            newTris[i] = currentTris[i];
        }
        
        newTris[currentLen + 0] = aIndex;       //a
        newTris[currentLen + 1] = aIndex + 2;   //c
        newTris[currentLen + 2] = aIndex + 1;   //b
        newTris[currentLen + 3] = aIndex + 2;   //c
        newTris[currentLen + 4] = aIndex + 3;   //d
        newTris[currentLen + 5] = aIndex + 1;   //b


        Vector3 pos = peelTransform.position;
        Vector3 right = peelTransform.right * thickness;
        c = pos + right;
        d = pos - right;
        vertices.Add(c);
        vertices.Add(d);

        //string v = "V: ";
        //string idx = "I: " ;
        //for (int i = 0; i < vertices.Count; i++)
        //{
        //    v += vertices[i].ToString() + ", ";
        //}

        //Debug.Log(v);
        //for (int i = 0; i < newTris.Length; i++)
        //{
        //    idx += newTris[i].ToString() + ", ";
        //}

        //Debug.Log(idx);


        tapeMesh.Clear();
        
        tapeMesh.SetVertices(vertices);
        tapeMesh.SetTriangles(newTris, 0) ;
        tapeMesh.RecalculateNormals();
    }

    GameObject getLastSegment()
    {
        if (tapeSegments.Count > 0)
        {
            return tapeSegments[tapeSegments.Count - 1];
        }
        else
        {
            return null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        print("Bang");
        if (!isDispensing)
        {
            isDispensing = true;

            Vector3 collisionPoint = collision.GetContact(0).point;
            peelTransform.position = collisionPoint;
            DispenseTape();

        }




    }
}
