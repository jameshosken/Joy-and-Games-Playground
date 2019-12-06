using System.Collections;
using UnityEngine;

public class VoxelGenerator : MonoBehaviour
{
    [SerializeField] private GameObject cube;
    [SerializeField] private Vector3 bounds;
    [SerializeField] private float threshold;
    [SerializeField] private float noiseScale = 0.9f;
    [SerializeField] private float timeDelay = 0.05f;
    [SerializeField] private float brushRadius = 3f;
    [SerializeField] private float brushIncrement = 0.01f;
    [SerializeField] private GameObject addBrush;
    [SerializeField] private GameObject removeBrush;

    [SerializeField] Color zeroColour;
    [SerializeField] Color fullColour;

    private float[,,] cubeWorld;
    private GameObject[,,] cubes;
    private Camera cam;
    private void Start()
    {
        cam = Camera.main;

        float startTime = Time.realtimeSinceStartup;
        cubeWorld = new float[(int)bounds.x, (int)bounds.y, (int)bounds.z];
        cubes = new GameObject[(int)bounds.x, (int)bounds.y, (int)bounds.z];

        GenerateCubeWorldData((int)bounds.x, (int)bounds.y, (int)bounds.z);
        GenerateAllCubes((int)bounds.x, (int)bounds.y, (int)bounds.z);

        float runTime = Time.realtimeSinceStartup - startTime;
        Debug.Log("Data generated in " + runTime + " seconds");

        StartCoroutine(GenerateCubeWorldInLayers());
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            addBrush.SetActive(true);
        }

        if (Input.GetMouseButton(0))
        {
            HandleMousePress(1);
        }


        if (Input.GetMouseButtonUp(0))
        {
            addBrush.SetActive(false);
        }

        if (Input.GetMouseButtonDown(1))
        {
            removeBrush.SetActive(true);
        }

        if (Input.GetMouseButton(1))
        {
            HandleMousePress(-1);
        }


        if (Input.GetMouseButtonUp(1))
        {
            removeBrush.SetActive(false);
        }
    }

    private void HandleMousePress(int amount)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            removeBrush.transform.position = hit.point;
            addBrush.transform.position = hit.point;
 
            IncreaseDensityAroundPoint(hit.point, amount);
        }
    }

    private void IncreaseDensityAroundPoint(Vector3 point, int amount)
    {
        for (int x = (int)-brushRadius + (int)point.x; x <= brushRadius + (int)point.x; x++)
        {
            for (int y = (int)-brushRadius + (int)point.y; y <= brushRadius + (int)point.y; y++)
            {
                for (int z = (int)-brushRadius + (int)point.z; z <= (int)brushRadius + (int)point.z; z++)
                {
                    //Make a sphere
                    if (Vector3.Distance(new Vector3(x, y, z), point) < brushRadius)
                    {
                        //Check Bounds
                        if (x < bounds.x && y < bounds.y && z < bounds.z && x >= 0 && y >= 0 && z >= 0)
                        {
                            cubeWorld[x, y, z] += brushIncrement * Time.deltaTime * amount;
                            UpdateCube(x, y, z);
                        }
                    }
                }
            }
        }
    }

    private IEnumerator GenerateCubeWorldInLayers()
    {
        int maxBounds = Mathf.Max(new int[] { (int)bounds.x, (int)bounds.y, (int)bounds.z });

        for (int i = 0; i < maxBounds; i++)
        {

            //XY Plane
            for (int x = 0; x <= i; x++)
            {
                for (int y = 0; y <= i; y++)
                {
                    UpdateCube(x, y, i);
                }
            }

            yield return new WaitForSeconds(timeDelay);
            //XZ Plane
            for (int x = 0; x < i; x++)
            {
                for (int z = 0; z <= i; z++)
                {
                    UpdateCube(x, i, z);
                }
            }
            yield return new WaitForSeconds(timeDelay);

            //YZ Plane
            for (int y = 0; y <= i; y++)
            {
                for (int z = 0; z < i; z++)
                {
                    UpdateCube(i, y, z);
                }
            }

            yield return new WaitForSeconds(timeDelay);



        }
    }

    private void GenerateCubeWorldData(int sizeX, int sizeY, int sizeZ)
    {


        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    cubeWorld[x, y, z] = PerlinNoise3D.Perlin3D(x * noiseScale, y * noiseScale, z * noiseScale);
                }
            }
        }
    }
    //Check Bounds
    private void AddCube(int x, int y, int z)
    {
        if (x < bounds.x && y < bounds.y && z < bounds.z)
        {
            if (cubeWorld[x, y, z] > threshold)
            { 
                cubes[x, y, z].SetActive(true);
                cubes[x, y, z].GetComponent<MarchingCubeData>().SetColour(Color.Lerp(zeroColour, fullColour, (cubeWorld[x, y, z] - threshold) * 10f));
            }
            else
            {
                cubes[x, y, z].SetActive(false);
            }
        }
    }

    //Do not check bounds
    private void UpdateCube(int x, int y, int z)
    {

        if (cubeWorld[x, y, z] > threshold)
        {
            cubes[x, y, z].SetActive(true);
            cubes[x, y, z].GetComponent<MarchingCubeData>().SetColour(Color.Lerp(zeroColour, fullColour, (cubeWorld[x, y, z] - threshold) * 10f));
        }
        else
        {
            cubes[x, y, z].SetActive(false);
        }
        
    }

    private void GenerateAllCubes(int sizeX, int sizeY, int sizeZ)
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    cubes[x, y, z] = Instantiate(cube, new Vector3(x, y, z), Quaternion.identity) as GameObject;
                    cubes[x, y, z].transform.parent = transform;
                    cubes[x, y, z].SetActive(false);
                }
            }
        }
    }





    //Combine generate and instantiate
    private void GenerateCube(int x, int y, int z)
    {
        if (x < bounds.x && y < bounds.y && z < bounds.z)
        {
            cubeWorld[x, y, z] = PerlinNoise3D.Perlin3D(x * noiseScale, y * noiseScale, z * noiseScale);

            if (cubeWorld[x, y, z] > threshold)
            {

                Instantiate(cube, new Vector3(x, y, z), Quaternion.identity);

            }
        }
    }

    private void GenerateCubeWorldAtOnce()
    {
        for (int x = 0; x < bounds.x; x++)
        {
            for (int y = 0; y < bounds.y; y++)
            {
                for (int z = 0; z < bounds.z; z++)
                {
                    cubeWorld[x, y, z] = PerlinNoise3D.Perlin3D(x * noiseScale, y * noiseScale, z * noiseScale);

                    if (cubeWorld[x, y, z] > threshold)
                    {
                        Instantiate(cube, new Vector3(x, y, z), Quaternion.identity);
                    }
                }
            }
        }
    }

    //public float Perlin3D(float x, float y, float z)
    //{
    //    Debug.Log(x + ", " + y + ", " + z);
    //    float AB = Mathf.PerlinNoise(x, y);
    //    float BC = Mathf.PerlinNoise(y, z);
    //    float AC = Mathf.PerlinNoise(x, z);

    //    float BA = Mathf.PerlinNoise(y, x);
    //    float CB = Mathf.PerlinNoise(z, y);
    //    float CA = Mathf.PerlinNoise(z, x);

    //    float ABC = AB + BC + AC + BA + CB + CA;
    //    Debug.Log(ABC);
    //    return ABC / 6f;
    //}

}
