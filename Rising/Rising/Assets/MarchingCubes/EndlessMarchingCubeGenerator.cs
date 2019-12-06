using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class EndlessMarchingCubeGenerator : MonoBehaviour
{
    private enum WorldType { Noise, Sphere, SphereNoise }

    [Header("General")]
    [SerializeField] private GameObject chunkTemplate;

    [Space()]
    [Header("Map")]
    [SerializeField] private WorldType worldType;
    [SerializeField] private int mapDataChunkSize;      //Use this for grid calculations
    [SerializeField] float scale = 1f;
    [SerializeField] private float threshold;
    [SerializeField] private float noiseScale = 0.9f;
    [SerializeField] private float timeDelay = 0.05f;

    //[Space()]
    //[Header("Brush")]
    //[SerializeField] private float brushRadius = 3f;
    //[SerializeField] private float brushIncrement = 0.01f;
    //[SerializeField] private GameObject addBrush;
    //[SerializeField] private GameObject removeBrush;

    [Space()]
    [Header("Geometry and Materials")]
    [SerializeField] private Color zeroColour;
    [SerializeField] private Color fullColour;

    [Space()]
    [Header("Player")]
    [SerializeField] int chunksGeneratedPerFrame = 10;
    [SerializeField] private Transform player;
    [SerializeField] float preloadRadius;
    [SerializeField] private float radius = 3;
    [SerializeField] private float turnOffRadius = 5;
    [SerializeField] private float checkMoveThreshold = 3f;

    [Space()]
    [SerializeField] bool generateOnAwake = true;

    private Camera cam;
    private Vector3 prevPlayerPos;

    //Store map data with coordinates
    private Dictionary<int[], float[,,]> endlessMapDataChunksDictionary = new Dictionary<int[], float[,,]>(new MyEqualityComparer());

    //Store marching cube scripts with coordinates
    private Dictionary<int[], EndlessMarchingCubeChunk> endlessMarchingCubeChunksDictionary = new Dictionary<int[], EndlessMarchingCubeChunk>(new MyEqualityComparer());


    List<int[]> coords = new List<int[]>();
    private void Start()
    {
        float startTime = Time.realtimeSinceStartup;
        cam = Camera.main;

        Debug.Log("Scene generated in: " + (Time.realtimeSinceStartup - startTime).ToString() + " seconds");

        prevPlayerPos = -player.position;


        SetupChunkDictionary();
        StartCoroutine(GenerateMapChunksInLargeRadius());

        if (generateOnAwake)
        {
            StartCoroutine(ActivateMapChunksAroundPlayer());
        }


        generateRandomCoordOrder();
        

    }

    private IEnumerator SetupChunkDictionary()
    {
        Vector3 playerPos = player.position;
        int[] playerCoords = GetGridCoordFromWorldPosition(playerPos);

        int search = 30;

        int c = 0;
        List<int[]> coords = new List<int[]>();
        for (int x = -search; x < search; x++)
        {
            for (int y = -search; y < search; y++)
            {
                for (int z = -search; z < search; z++)
                {
                    coords.Add(new int[] { x, y, z });
                }
            }
        }

        //coords = coords.OrderBy(x => UnityEngine.Random.value).ToList();

        for (int i = 0; i < coords.Count; i++)
        {
            int x = coords[i][0];
            int y = coords[i][1];
            int z = coords[i][2];

            Debug.Log(x.ToString() + ", " + y.ToString() + ", " + z.ToString());
            c++;

            if (c % chunksGeneratedPerFrame == 0)
            {
                yield return null;
            }

            int[] offsetCoord = {
                            playerCoords[0] + x,
                            playerCoords[1] + y,
                            playerCoords[2] + z
                        };


                float[,,] mapDataChunk = GenerateMapDataChunk(offsetCoord);
                endlessMapDataChunksDictionary.Add(offsetCoord, mapDataChunk);
            
        }

        yield return null;
    }

    private void generateRandomCoordOrder()
    {
        for (int x = -(int)radius; x < radius; x++)
        {
            for (int y = -(int)radius; y < radius; y++)
            {
                for (int z = -(int)radius; z < radius; z++)
                {
                    coords.Add(new int[] { x, y, z });
                }
            }
        }

        coords = coords.OrderBy(x => UnityEngine.Random.value).ToList();
    }

    private void Update()
    {

        if (Vector3.Distance(prevPlayerPos, player.position) > checkMoveThreshold)
        {
            StopCoroutine(ActivateMapChunksAroundPlayer());
            StartCoroutine(ActivateMapChunksAroundPlayer());

            prevPlayerPos = player.position;
        }

    }


    bool IsOutOfRange(Vector3 pos)
    {
        if (Vector3.Distance(pos, player.position) > turnOffRadius * scale)
        {
            return true;
        }
        return false;
    }
    private void TurnOffChunksAroundPlayer()
    {
        List<KeyValuePair<int[], EndlessMarchingCubeChunk>> chunksToRemove = new List<KeyValuePair<int[], EndlessMarchingCubeChunk>>();

        foreach (KeyValuePair<int[], EndlessMarchingCubeChunk> chunk in endlessMarchingCubeChunksDictionary)
        {
            Vector3 chunkPos = chunk.Value.transform.position;
            if (IsOutOfRange(chunkPos))
            {
                chunk.Value.gameObject.SetActive(false);
            }

            if (Vector3.Distance(chunkPos, player.position) > turnOffRadius * scale * 1.2f)
            {
                //print("Removing");
                chunksToRemove.Add(chunk);

            }

        }

        for (int i = 0; i < chunksToRemove.Count; i++)
        {
            endlessMarchingCubeChunksDictionary.Remove(chunksToRemove[i].Key);
        }
    }

    IEnumerator GenerateMapChunksInLargeRadius()
    {
        Vector3 playerPos = player.position;
        int[] playerCoords = GetGridCoordFromWorldPosition(playerPos);

        int search = (int)preloadRadius;

        int c = 0;
        List<int[]> coords = new List<int[]>();
        for (int x = -search; x < search; x++)
        {
            for (int y = -search; y < search; y++)
            {
                for (int z = -search; z < search; z++)
                {
                    coords.Add(new int[] { x, y, z });
                }
            }
        }

        //coords = coords.OrderBy(x => UnityEngine.Random.value).ToList();

        for (int i = 0; i < coords.Count; i++)
        {
            int x = coords[i][0];
            int y = coords[i][1];
            int z = coords[i][2];

            Debug.Log(x.ToString() + ", " + y.ToString() + ", " + z.ToString());
            c++;

            if (c % chunksGeneratedPerFrame == 0)
            {
                yield return null;
            }

            int[] offsetCoord = {
                            playerCoords[0] + x,
                            playerCoords[1] + y,
                            playerCoords[2] + z
                        };

            //If data exists there,
            if (MapDataExists(endlessMapDataChunksDictionary, offsetCoord))
            {
                endlessMarchingCubeChunksDictionary[offsetCoord].gameObject.SetActive(true);
            }
            //Otherwise create more data!
            else
            {
                float[,,] mapDataChunk = GenerateMapDataChunk(offsetCoord);
                endlessMapDataChunksDictionary.Add(offsetCoord, mapDataChunk);
                GenerateMarchingCubeChunk(mapDataChunk, offsetCoord);
                if (IsOutOfRange(endlessMarchingCubeChunksDictionary[offsetCoord].transform.position))
                {
                    endlessMarchingCubeChunksDictionary[offsetCoord].gameObject.SetActive(false);
                }
            }
        }
        yield return null;
    }

    IEnumerator ActivateMapChunksAroundPlayer()
    {

        Vector3 playerPos = player.position;
        int[] playerCoords = GetGridCoordFromWorldPosition(playerPos);

        int search = (int)radius;

        int c = 0;

        for (int i = 0; i < coords.Count; i++)
        {
            int x = coords[i][0];
            int y = coords[i][1];
            int z = coords[i][2];


            //TODO check this? 
            if (Vector3.Distance(
                new Vector3(x + playerCoords[0], y + playerCoords[1], z + playerCoords[2]),
                new Vector3(playerCoords[0], playerCoords[1], playerCoords[2]))
                < radius)
            {


                int[] offsetCoord = {
                            playerCoords[0] + x,
                            playerCoords[1] + y,
                            playerCoords[2] + z
                        };

                //If data exists there,
                if (MapDataExists(endlessMapDataChunksDictionary, offsetCoord))
                {
                    //If there's already a chunk there (i.e. has not been deleted:
                    if (ChunkDataExists(endlessMarchingCubeChunksDictionary, offsetCoord))
                    {
                        if (IsOutOfRange(endlessMarchingCubeChunksDictionary[offsetCoord].transform.position))
                        {
                            endlessMarchingCubeChunksDictionary[offsetCoord].gameObject.SetActive(false);
                        }
                        else
                        {
                            endlessMarchingCubeChunksDictionary[offsetCoord].gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        GenerateMarchingCubeChunk(endlessMapDataChunksDictionary[offsetCoord], offsetCoord);

                        if (IsOutOfRange(endlessMarchingCubeChunksDictionary[offsetCoord].transform.position))
                        {
                            endlessMarchingCubeChunksDictionary[offsetCoord].gameObject.SetActive(false);
                        }
                    }
                }
                //Otherwise create more data and a chunk!
                else
                {
                    float[,,] mapDataChunk = GenerateMapDataChunk(offsetCoord);
                    endlessMapDataChunksDictionary.Add(offsetCoord, mapDataChunk);
                    GenerateMarchingCubeChunk(mapDataChunk, offsetCoord);

                    if (IsOutOfRange(endlessMarchingCubeChunksDictionary[offsetCoord].transform.position))
                    {
                        endlessMarchingCubeChunksDictionary[offsetCoord].gameObject.SetActive(false);
                    }

                    c++;
                    if (c % chunksGeneratedPerFrame == 0)
                    {
                        yield return null;
                    }
                }
            }

        }

        TurnOffChunksAroundPlayer();
        yield return null;
    }



    private float[,,] GenerateMapDataChunk(int[] offsetCoord)
    {

        float[,,] mapDataChunk = new float[mapDataChunkSize + 1, mapDataChunkSize + 1, mapDataChunkSize + 1];

        //Unlike non-endless generators, we can't use dynamic max and min vals to normalise so we have to estimate them. 
        float maxVal = 1;
        float minVal = 0;

        for (int x = 0; x < mapDataChunkSize + 1; x++)
        {
            for (int y = 0; y < mapDataChunkSize + 1; y++)
            {
                for (int z = 0; z < mapDataChunkSize + 1; z++)
                {

                    //add offsets to coords
                    float xOff = x + offsetCoord[0] * mapDataChunkSize;
                    float yOff = y + offsetCoord[1] * mapDataChunkSize;
                    float zOff = z + offsetCoord[2] * mapDataChunkSize;

                    //Generate noise based on offset

                    //float point = 0;

                    //float lacunarity = 1.6f;
                    //float persistence = 0.8f;
                    //float amp = 1;
                    //float scl = 1;

                    //yOff here creates gradient of noise
                    mapDataChunk[x, y, z] = PerlinNoise3D.Perlin3D(xOff * noiseScale + 10, yOff * noiseScale + 100, zOff * noiseScale) - yOff * 0.001f;

                    //mapDataChunk[x, y, z] = PerlinNoise3D.Perlin3D(xOff * noiseScale + 10, yOff * noiseScale + 100, zOff * noiseScale);
                }
            }
        }

        for (int x = 0; x < mapDataChunkSize + 1; x++)
        {
            for (int y = 0; y < mapDataChunkSize + 1; y++)
            {
                for (int z = 0; z < mapDataChunkSize + 1; z++)
                {
                    mapDataChunk[x, y, z] = Map(mapDataChunk[x, y, z], minVal, maxVal, 0f, 1f);
                }
            }
        }

        return mapDataChunk;
    }

    private void GenerateMarchingCubeChunk(float[,,] mapDataChunk, int[] offset)
    {

        GameObject chunk = Instantiate(chunkTemplate) as GameObject;
        chunk.transform.parent = transform;
        chunk.transform.localScale = Vector3.one * scale;

        EndlessMarchingCubeChunk chunkHandler = chunk.GetComponent<EndlessMarchingCubeChunk>();
        chunkHandler.SetChunkData();

        Vector3 offsetPosition = new Vector3(offset[0] * mapDataChunkSize, offset[1] * mapDataChunkSize, offset[2] * mapDataChunkSize) * scale;
        chunkHandler.AddMapChunk(offsetPosition, mapDataChunkSize);


        chunkHandler.UpdateMesh(mapDataChunk, threshold);

        //Add this chunk to the dictionary
        endlessMarchingCubeChunksDictionary.Add(offset, chunkHandler);
    }




    //private void HandleMousePress(int amount)
    //{
    //    Ray ray = cam.ScreenPointToRay(Input.mousePosition);
    //    RaycastHit hit;

    //    if (Physics.Raycast(ray, out hit))
    //    {
    //        removeBrush.transform.position = hit.point;
    //        addBrush.transform.position = hit.point;

    //        IncreaseDensityAroundPoint(hit.point, amount);
    //    }
    //}

    //private void IncreaseDensityAroundPoint(Vector3 point, int amount)
    //{

    //    List<int[]> chunksModified = new List<int[]>();
    //    int[] worldCoord = GetGridCoordFromWorldPosition(point);

    //    for (int x = (int)-brushRadius + (int)point.x; x <= brushRadius + (int)point.x; x++)
    //    {
    //        for (int y = (int)-brushRadius + (int)point.y; y <= brushRadius + (int)point.y; y++)
    //        {
    //            for (int z = (int)-brushRadius + (int)point.z; z <= (int)brushRadius + (int)point.z; z++)
    //            {


    //                //Make a sphere
    //                if (Vector3.Distance(new Vector3(x, y, z), point) < brushRadius)
    //                {
    //                    // No longler checking bounds here. Could cause issues with index out of bounds,
    //                    // but hopefully the buffer zone is big enough to avoid checking at edges

    //                    //Here we find the world position of the corner of this cube
    //                    int[] chunkOrigin =
    //                    {
    //                        worldCoord[0] * mapDataChunkSize,
    //                        worldCoord[1] * mapDataChunkSize,
    //                        worldCoord[2] * mapDataChunkSize
    //                    };

    //                    //Determine where brush overlaps with neighbours
    //                    bool[] overlaps =
    //                    {
    //                        x < chunkOrigin[0],
    //                        y < chunkOrigin[1],
    //                        z < chunkOrigin[2],
    //                        x >= chunkOrigin[0] + mapDataChunkSize,
    //                        y >= chunkOrigin[1] + mapDataChunkSize,
    //                        z >= chunkOrigin[2] + mapDataChunkSize
    //                    };

    //                    //Offset chunks if brush overlaps
    //                    int[] offsets = { 0, 0, 0 };

    //                    if (overlaps[0]) offsets[0] = -1;
    //                    if (overlaps[1]) offsets[1] = -1;
    //                    if (overlaps[2]) offsets[2] = -1;
    //                    if (overlaps[3]) offsets[0] = 1;
    //                    if (overlaps[4]) offsets[1] = 1;
    //                    if (overlaps[5]) offsets[2] = 1;

    //                    int[] chunkCoord = { x + offsets[0], y + offsets[1], z + offsets[2] };

    //                    if (endlessMapDataChunksDictionary.ContainsKey(chunkCoord))
    //                    {
    //                        if (!chunksModified.Contains(chunkCoord))
    //                        {
    //                            chunksModified.Add(chunkCoord);
    //                        }

    //                        // Normalise since all chunks start at 0,0,0
    //                        int[] pointToModify =
    //                        {

    //                            x % mapDataChunkSize,
    //                            y % mapDataChunkSize,
    //                            z % mapDataChunkSize
    //                        };

    //                        endlessMapDataChunksDictionary[chunkCoord][pointToModify[0], pointToModify[1], pointToModify[2]] += brushIncrement * Time.deltaTime * amount;
    //                    }


    //                }
    //            }
    //        }
    //    }
    //    for (int i = 0; i < chunksModified.Count; i++)
    //    {
    //        int[] coordKey = chunksModified[i];

    //        //Check if chunk exists at location
    //        if (endlessMarchingCubeChunksDictionary.ContainsKey(coordKey))
    //        {
    //            //If so, update with new data.
    //            endlessMarchingCubeChunksDictionary[coordKey].UpdateMesh(endlessMapDataChunksDictionary[coordKey], threshold);

    //        }

    //    }

    //}

    /// <summary>
    /// Helper Functions
    /// </summary>
    /// 
    public float Map(float val, float OldMin, float OldMax, float NewMin, float NewMax)
    {

        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((val - OldMin) * NewRange) / OldRange) + NewMin;

        return (NewValue);
    }

    private int[] GetGridCoordFromWorldPosition(Vector3 position)
    {
        int[] coord = new int[3];

        coord[0] = (int)(position.x / (float)mapDataChunkSize / scale);
        coord[1] = (int)(position.y / (float)mapDataChunkSize / scale);
        coord[2] = (int)(position.z / (float)mapDataChunkSize / scale);



        return coord;

    }
    private bool ChunkDataExists(Dictionary<int[], EndlessMarchingCubeChunk> dict, int[] offsetCoord)
    {
        MyEqualityComparer eq = new MyEqualityComparer();
        foreach (KeyValuePair<int[], EndlessMarchingCubeChunk> chunk in dict)
        {

            if (eq.Equals(chunk.Key, offsetCoord))
            {
                return true;
            }

        }
        return false;
    }
    private bool MapDataExists(Dictionary<int[], float[,,]> dict, int[] offsetCoord)
    {
        MyEqualityComparer eq = new MyEqualityComparer();
        foreach (KeyValuePair<int[], float[,,]> chunk in dict)
        {

            if (eq.Equals(chunk.Key, offsetCoord))
            {
                return true;
            }

        }
        return false;
    }

    private bool ChunkContainsKeyArray(Dictionary<int[], EndlessMarchingCubeChunk> dict, int[] offsetCoord)
    {
        MyEqualityComparer eq = new MyEqualityComparer();
        foreach (KeyValuePair<int[], EndlessMarchingCubeChunk> chunk in dict)
        {
            if (eq.Equals(chunk.Key, offsetCoord))
            {
                return true;
            }

        }
        return false;
    }

}


//https://stackoverflow.com/questions/14663168/an-integer-array-as-a-key-for-dictionary
public class MyEqualityComparer : IEqualityComparer<int[]>
{
    public bool Equals(int[] x, int[] y)
    {
        if (x.Length != y.Length)
        {
            return false;
        }
        for (int i = 0; i < x.Length; i++)
        {
            if (x[i] != y[i])
            {
                return false;
            }
        }
        return true;
    }

    public int GetHashCode(int[] obj)
    {
        int result = 17;
        for (int i = 0; i < obj.Length; i++)
        {
            unchecked
            {
                result = result * 23 + obj[i];
            }
        }
        return result;
    }
}