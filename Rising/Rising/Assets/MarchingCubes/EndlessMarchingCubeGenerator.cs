using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Threading;

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

    [Space()]
    [Header("Player")]
    [SerializeField] private Transform player;

    [Space()]
    [Header("Generators")]
    [SerializeField] float chunksPerFrame = 5;
    [SerializeField] private float radius = 3;
    [SerializeField] private float turnOffRadius = 5;
    [SerializeField] private float checkMoveThreshold = 3f;

    [Space()]
    [SerializeField] bool generateOnAwake = true;

    private Camera cam;
    private Vector3 prevPlayerPos;

    //Store marching cube scripts with coordinates
    private Dictionary<int[], EndlessMarchingCubeChunk> endlessMarchingCubeChunksDictionary = new Dictionary<int[], EndlessMarchingCubeChunk>(new MyEqualityComparer());

    //Store random order list to activate chunks
    List<int[]> coords = new List<int[]>();


    private void Start()
    {
        cam = Camera.main;

        prevPlayerPos = -player.position;

        generateRandomCoordOrder();

        if (generateOnAwake)
        {
            StartCoroutine( ActivateMapChunksAroundPlayer() );
        }

        
    }

    private void Update()
    {

        if (Vector3.Distance(prevPlayerPos, player.position) > checkMoveThreshold)
        {

            float startTime = Time.realtimeSinceStartup;
            //Debug.Log("Frame Start: " + startTime);
            StopCoroutine(ActivateMapChunksAroundPlayer());
            StartCoroutine( ActivateMapChunksAroundPlayer() );
            prevPlayerPos = player.position;

        }

        //Remove out of range every 5 seconds or so:
        if (Time.frameCount % 300 == 0)
        {
            StartCoroutine(TurnOffChunksAroundPlayer());
        }

    }

    IEnumerator ActivateMapChunksAroundPlayer()
    {
        Vector3 playerPos = player.position;
        int[] playerCoords = GetGridCoordFromWorldPosition(playerPos);

        int search = (int)radius;

        int c = 0;

        //Loop through randomised list of grid coordinates
        for (int i = 0; i < coords.Count; i++)
        {
            int x = coords[i][0];
            int y = coords[i][1];
            int z = coords[i][2];

            // Check distance from grid chunk to player
            if (Vector3.Distance(
                new Vector3(x + playerCoords[0], y + playerCoords[1], z + playerCoords[2]),
                new Vector3(playerCoords[0], playerCoords[1], playerCoords[2]))
                < radius)
            {

                //Evaluade coordinate of grid chunk:
                int[] offsetCoord = {
                            playerCoords[0] + x,
                            playerCoords[1] + y,
                            playerCoords[2] + z
                        };

                //If data exists there,
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
                //Otherwise generate new chunk:
                else
                {
                    //Flag which chunks to generate:
                    GenerateMarchingCubeChunk(offsetCoord);
                    
                    if (IsOutOfRange(endlessMarchingCubeChunksDictionary[offsetCoord].transform.position))
                    {
                        endlessMarchingCubeChunksDictionary[offsetCoord].gameObject.SetActive(false);
                    }

                    c++;
                    if(c % chunksPerFrame == 0)
                    {
                        yield return null;
                    }
                }
            }
        }

    }



    private void GenerateMarchingCubeChunk(int[] offset)
    {
        //Generate empty objects:
        GameObject chunk = Instantiate(chunkTemplate) as GameObject;
        chunk.transform.parent = transform;
        chunk.transform.localScale = Vector3.one * scale;
        EndlessMarchingCubeChunk chunkHandler = chunk.GetComponent<EndlessMarchingCubeChunk>();

        //First, flag that this data now exists:
        endlessMarchingCubeChunksDictionary.Add(offset, chunkHandler);

        // MUST GENERATE MAP DATA HERE TOO

        chunkHandler.SetChunkData(offset, mapDataChunkSize, scale, noiseScale, threshold);

        chunkHandler.Generate();

    }


    ////////////
    ////////////

    bool IsOutOfRange(Vector3 pos)
    {
        if (Vector3.Distance(pos, player.position) > turnOffRadius * scale)
        {
            return true;
        }
        return false;
    }

    IEnumerator TurnOffChunksAroundPlayer()
    {

        int c = 0;
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
            c++;
            if(c % chunksPerFrame == 0)
            {
                yield return null;
            }
            
        }

        for (int i = 0; i < chunksToRemove.Count; i++)
        {
            endlessMarchingCubeChunksDictionary.Remove(chunksToRemove[i].Key);
        }
    }


    /// <summary>
    /// Helper Functions
    /// </summary>
    /// 
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

//ARCHIVE


/*
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
*/

/*
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
*/


////PREV CREATOR:
/*
IEnumerator ActivateMapChunksAroundPlayer()
    {

        Vector3 playerPos = player.position;
int[] playerCoords = GetGridCoordFromWorldPosition(playerPos);

int search = (int)radius;

int c = 0;

        //Loop through randomised list of grid coordinates
        for (int i = 0; i<coords.Count; i++)
        {
            int x = coords[i][0];
int y = coords[i][1];
int z = coords[i][2];

            // Check distance from grid chunk to player
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

                /// New Algo:
                /// Each chunk stores its own data. 
                /// Create data and mesh together.
                /// Only check against data

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
    */
//private bool MapDataExists(Dictionary<int[], float[,,]> dict, int[] offsetCoord)
//{
//    MyEqualityComparer eq = new MyEqualityComparer();
//    foreach (KeyValuePair<int[], float[,,]> chunk in dict)
//    {

//        if (eq.Equals(chunk.Key, offsetCoord))
//        {
//            return true;
//        }

//    }
//    return false;
//}