using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGeneration : MonoBehaviour
{
    // chunk details 
    public Vector2 chunks; // how many x by y chunks
    public Vector2 chunkResolution; // points in each chunk. Ex: x = 4, z = 4. Each chunk will have 16 faces (technically 25 bc the +1 to each).
    private Vector2 chunkMiddle;

    // material for the terrain
    public Material terrainMaterial;
    // water details
    public GameObject water; // flat plane for water
    public float waterLevel; // y level water should be instantiated at
    // tree details
    public GameObject[] trees; // list of my tree prefabs
    public float treeThreshold; // 

    public GameObject spawner;
    public float spawnerThreshold; // 

    public GameObject ship;
    public bool shipSpawned = false;

    public GameObject player;
    public bool playerSpawned = false;

    public int chunksChunkLoaded;

    public int seed;

    private void Start()
    {
        seed = Random.Range(1, 100000);
        chunkMiddle = new Vector2((chunks.x / 2f) * 128, (chunks.y / 2f) * 128);
        waterLevel = Mathf.PerlinNoise(seed, seed) * 15;
        StartCoroutine(GenerateChunks());
        GameObject current = Instantiate(water, new Vector3(((128 * chunks.x) / 2) - chunkMiddle.x, waterLevel, ((128 * chunks.y) / 2) - chunkMiddle.y), Quaternion.identity);
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
                i++;
                if (i == chunksChunkLoaded)
                {
                    i = 0;
                    yield return new WaitForSeconds(Time.deltaTime * 2);
                }
            }
        }
    }
}
