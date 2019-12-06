using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MarchingCubePlanetGenerator : MonoBehaviour
{

    enum WorldType {SmoothSphere, SphereNoise}
    
    [Header("General")]
    [SerializeField] GameObject chunkTemplate;

    [Space()]
    [Header("Map")]
    [SerializeField] WorldType worldType;
    [SerializeField] private Vector3 worldbounds;
    [Tooltip("Must be int!")]
    [SerializeField] Vector3 chunkSize;
    [SerializeField] private float threshold;
    [SerializeField] private float timeDelay = 0f;

    [Space]
    [Header("Noise Parameters")]
    [SerializeField] int seed = 0;
    [SerializeField] int octaves = 3;
    [Range(1,10)]
    [SerializeField] float lacunarity;
    [Range(0, 1)]
    [SerializeField] float persistence;
    [SerializeField] private float noiseScale = 0.5f;
    [SerializeField] Vector2 remapHeight;

    [Space()]
    [Header("Brush")]
    [SerializeField] private float brushRadius = 3f;
    [SerializeField] private float brushIncrement = 0.01f;
    [SerializeField] private GameObject addBrush;
    [SerializeField] private GameObject removeBrush;

    [Space()]
    [Header("Geometry and Materials")]
    [SerializeField] Color zeroColour;
    [SerializeField] Color fullColour;


    private float[,,] cubeWorld;
    private Camera cam;

    List<MarchingCubeChunk> chunks = new List<MarchingCubeChunk>();

    MarchingCubeChunk[,,] chunkMap;

    [SerializeField] bool generateMap = false;

    private void Start()
    {
        
        cam = Camera.main;

    }



    void GenerateMap()
    {

        float startTime = Time.realtimeSinceStartup;
        cubeWorld = new float[(int)worldbounds.x, (int)worldbounds.y, (int)worldbounds.z];
        GenerateCubeWorldData((int)worldbounds.x, (int)worldbounds.y, (int)worldbounds.z, worldType);
        chunkMap = new MarchingCubeChunk[(int)worldbounds.x / (int)chunkSize.x, (int)worldbounds.y / (int)chunkSize.y, (int)worldbounds.z / (int)chunkSize.z];

        Debug.Log("Scene generated in: " + (Time.realtimeSinceStartup - startTime).ToString() + " seconds");

        addBrush.transform.localScale = Vector3.one * brushRadius * 2;
        removeBrush.transform.localScale = Vector3.one * brushRadius * 2;

        System.Random prng = new System.Random(seed); //Pseudo Random Number Generator

        StartCoroutine(GenerateMapChunks());

        generateMap = false;
    }


    IEnumerator GenerateMapChunks()
    {

        

        float startTime = Time.realtimeSinceStartup;
        for (int x = 0; x < worldbounds.x; x += (int)chunkSize.x)
        {
            for (int y = 0; y < worldbounds.y; y += (int)chunkSize.y)
            {
                for (int z = 0; z < worldbounds.z; z += (int)chunkSize.z)
                {


                    Debug.Log("Adding cube at: " + x.ToString() + ", " + y.ToString() + ", " + z.ToString() + ", ");

                    GameObject chunk = Instantiate(chunkTemplate) as GameObject;
                    chunk.transform.parent = transform;
                    //chunk.transform.position = new Vector3(x, y, z);

                    MarchingCubeChunk chunkHandler = chunk.GetComponent<MarchingCubeChunk>();
                    chunkHandler.SetChunkData();
                    chunkHandler.AddMapChunk(cubeWorld, new Vector3(x, y, z), chunkSize);
                    chunkHandler.UpdateMesh(threshold);

                    chunkMap[x / (int)chunkSize.x, y / (int)chunkSize.y, z / (int)chunkSize.z] = chunkHandler;

                }

                yield return null;
            }
            
        }

        Debug.Log("Chunk generated in: " + (Time.realtimeSinceStartup - startTime).ToString() + " seconds");
    }

    private void Update()
    {
        if (generateMap)
        {
            GenerateMap();
        }

        bool change = false;
        if (Input.GetMouseButtonDown(0))
        {
            addBrush.SetActive(true);
            
        }

        if (Input.GetMouseButton(0))
        {
            HandleMousePress(1);
            change = true;
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
            change = true;
        }


        if (Input.GetMouseButtonUp(1))
        {
            removeBrush.SetActive(false);
        }

        if (change)
        {
            //MarchingCubes.ConstructMeshFromVoxelMap(mesh, cubeWorld, threshold, 0, Vector3.zero);
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
                        if (x < worldbounds.x && y < worldbounds.y && z < worldbounds.z && x >= 0 && y >= 0 && z >= 0)
                        {
                            cubeWorld[x, y, z] += brushIncrement * Time.deltaTime * amount;

                            
                        }
                    }
                }
            }
        }

        //Update chunks near point:

        //First find which chunk point is in:

        int[] chunkCoord = new int[] {
            (int) (point.x / chunkSize.x),
            (int) (point.y / chunkSize.y),
            (int) (point.z / chunkSize.z) };


        //This could be optimised by only updating neighbours when necessary
        for (int x = -1; x <= 1; x++)
        {
            //Check Bounds
            if (x + chunkCoord[0] < 0 || x + chunkCoord[0] >= chunkMap.GetLength(0))
            {
                continue;
            }
            for (int y = -1; y <= 1; y++)
            {
                //Check Bounds
                if (y + chunkCoord[1] < 0 || y + chunkCoord[1] >= chunkMap.GetLength(1))
                {
                    continue;
                }
                for (int z = -1; z <= 1; z++)
                {
                    //Check Bounds
                    if (z + chunkCoord[2] < 0 || z + chunkCoord[2] >= chunkMap.GetLength(2))
                    {
                        continue;
                    }
                    MarchingCubeChunk chunk = chunkMap[x + chunkCoord[0], y + chunkCoord[1], z + chunkCoord[2]];
                    chunk.UpdateMapChunk(cubeWorld);
                    chunk.UpdateMesh(threshold);
                }
            }
        }



    }


    private void GenerateCubeWorldData(int sizeX, int sizeY, int sizeZ, WorldType type)
    {

        if (type == WorldType.SmoothSphere)
        {
            GenerateSphereWorld(sizeX, sizeY, sizeZ);
        }
        if (type == WorldType.SphereNoise)
        {
            GenerateNoiseSphereWorld(sizeX, sizeY, sizeZ);
        }


    }


    void GenerateSphereWorld(int sizeX, int sizeY, int sizeZ)
    {
        Vector3 midpoint = new Vector3(sizeX / 2, sizeY / 2, sizeY / 2);

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    float dist = Vector3.Distance(midpoint, new Vector3(x, y, z));
                    float distNormalised = dist / midpoint.x;

                    cubeWorld[x, y, z] = 1 - distNormalised;    //For some reason tris are flipped with regular distNormalised. 'One minus' corrects for this.
                }
            }
        }
    }

    void GenerateNoiseSphereWorld(int sizeX, int sizeY, int sizeZ)
    {

        float minVal = float.MaxValue;
        float maxVal = float.MinValue;
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    cubeWorld[x, y, z] = GetPlanetNoiseValue(x, y, z);

                    //For normalisation
                    if (cubeWorld[x, y, z] < minVal)
                    {
                        minVal = cubeWorld[x, y, z];
                    }
                    if (cubeWorld[x, y, z] > maxVal)
                    {
                        maxVal = cubeWorld[x, y, z];
                    }
                }
            }
        }


        //Normalise

        Vector3 midpoint = new Vector3(sizeX / 2, sizeY / 2, sizeY / 2);

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    float dist = Vector3.Distance(midpoint, new Vector3(x, y, z));
                    float distNormalised = dist / midpoint.x;

                    distNormalised = 1 - distNormalised;

                    cubeWorld[x, y, z] = Map(cubeWorld[x, y, z], minVal, maxVal, remapHeight.x, remapHeight.y) * distNormalised;
                }
            }
        }
    }

    private float GetPlanetNoiseValue(int x, int y, int z)
    {
        float value = 0;

        //PerlinNoise3D.Perlin3D(x * noiseScale, y * noiseScale, z * noiseScale);

        //Clamp scale above 0
        


        float amplitude = 1;
        float freq = 1;
        float noiseHeight = 0;

        for (int i = 0; i < octaves; i++)
        {
            float sampleX = (x) / noiseScale * freq;   //Heigher the freq, the further apart the sample points
            float sampleY = (y) / noiseScale * freq;
            float sampleZ = (z) / noiseScale * freq;
            float perlinValue = PerlinNoise3D.Perlin3D(sampleX, sampleY, sampleZ)*2-1;
            //Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

            noiseHeight += perlinValue * amplitude;

            amplitude *= persistence;   // Amplitude decreases per cycle (0 < a < 1);
            freq *= lacunarity;         // Freq increases per cycles     (1 < f)
        }

        value = noiseHeight;

        return value;
    }

    public float Map(float val, float OldMin, float OldMax, float NewMin, float NewMax)
    {

        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((val - OldMin) * NewRange) / OldRange) + NewMin;

        return (NewValue);
    }

    private void OnValidate()
    {
        if (noiseScale <= 0)
        {
            noiseScale = 0.0001f;
        }

        addBrush.transform.localScale = Vector3.one * brushRadius * 2;
        removeBrush.transform.localScale = Vector3.one * brushRadius * 2;

    }

}
