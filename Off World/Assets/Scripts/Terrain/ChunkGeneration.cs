using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGeneration : MonoBehaviour
{
    [Header("Chunk Properties")]
    public Vector2 chunks; // how many x by y chunks
    public Vector2 chunkResolution; // points in each chunk. Ex: x = 4, z = 4. Each chunk will have 16 faces (technically 25 bc the +1 to each).
    private Vector2 chunkMiddle;
    private List<GameObject> chunkList = new List<GameObject>();
    private bool goodSeed = false;

    [Header("Terrain Properties")]
    public Material terrainMaterial;
    // water details
    public GameObject water; // flat plane for water
    public float waterLevel; // y level water should be instantiated at
    // tree details
    public GameObject[] trees; // list of my tree prefabs
    public float treeThreshold;
    // spawner details
    public GameObject spawner;
    public float spawnerThreshold; 
    // ship details
    public GameObject ship;
    public GameObject currentShip;
    public bool shipSpawned = false;

    public GameObject player;
    public GameObject currentPlayer;
    public bool playerSpawned = false;

    public int chunksChunkLoaded;

    public float landThreshold;
    public float totalLandCount = 0;
    public float totalWaterCount = 0;

    public int seed;
    public bool useRandomSeed;

    private void Start()
    {
        StartCoroutine(GenerateAndValidateChunks());
    }

    private IEnumerator GenerateAndValidateChunks()
    {
        while (!goodSeed)
        {
            totalLandCount = 0;
            totalWaterCount = 0;
            if (useRandomSeed)
            {
                seed = Random.Range(0, 1000000);
            }
            
            chunkMiddle = new Vector2((chunks.x / 2f) * 128, (chunks.y / 2f) * 128);
            waterLevel = Mathf.PerlinNoise(seed, seed) * 15;

            yield return StartCoroutine(GenerateChunks());

            float landRatio = totalLandCount / (totalLandCount + totalWaterCount);


            if (landRatio >= landThreshold)
            {
                goodSeed = true;
                Debug.Log("Valid seed found!");
            }
            else
            {
                Debug.LogWarning("Invalid seed. Regenerating...");
                DestroyPlayer();
                DestroyShip();
                DestroyChunks();
            }
        }

        GameObject current = Instantiate(water,
            new Vector3(((128 * chunks.x) / 2) - chunkMiddle.x, waterLevel,
            ((128 * chunks.y) / 2) - chunkMiddle.y),
            Quaternion.identity
            );
        current.transform.localScale = new Vector3(12.9f, 12.9f, 12.9f) * chunks.x;
    }

    public IEnumerator GenerateChunks()
    {
        for (int i = 0, x = 0; x < chunks.x; x++)
        {
            for (int z = 0; z < chunks.y; z++)
            {
                GameObject current = new GameObject("Terrain" + " (" + new Vector2(x, z) + ")", 
                    typeof(TerrainGeneration),
                    typeof(MeshRenderer),
                    typeof(MeshFilter),
                    typeof(MeshCollider)
                    );
                current.transform.parent = transform;
                current.transform.position = new Vector3(x * (chunkResolution.x) * (128 / chunkResolution.x) - chunkMiddle.x,
                    0f,
                    z * (chunkResolution.y) * (128 / chunkResolution.y) - chunkMiddle.y);
                chunkList.Add(current);
                i++;
                if (i == chunksChunkLoaded)
                {
                    i = 0;
                    yield return new WaitForSeconds(Time.deltaTime * 2);
                }
            }
        }
    }

    // Destroy Functions
    private void DestroyChunks()
    {
        foreach (GameObject current in chunkList)
        {
            Destroy(current);
        }
        chunkList.Clear();
    } // Destroys All Terrain Chunks
    private void DestroyPlayer()
    {
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
            playerSpawned = false;
        }
    } // Destroys Player
    private void DestroyShip()
    {
        if (currentShip != null)
        {
            Destroy(currentShip);
            shipSpawned = false;
        }
    } // Destroys The Ship
}
