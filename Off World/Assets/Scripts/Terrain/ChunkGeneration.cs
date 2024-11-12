using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGeneration : MonoBehaviour
{
    public Vector2 chunks;
    public Vector2 chunkResolution;

    public Material terrainMaterial;

    public GameObject water;
    public float waterLevel;
    public GameObject[] trees;
    public float treeRandomness;
    public float treeThreshold;

    public Transform player;

    public int chunksChunkLoaded;

    public int seed;

    private void Start()
    {
        seed = Random.Range(1, 10000);
        waterLevel = Mathf.PerlinNoise(seed, seed) * 15;
        StartCoroutine(GenerateChunks());
        GameObject current = Instantiate(water, new Vector3((128 * chunks.x) / 2, waterLevel, (128 * chunks.y) / 2 ), Quaternion.identity);
        current.transform.localScale = new Vector3(12.9f, 12.9f, 12.9f) * chunks.x;
    }

    public IEnumerator GenerateChunks()
    {
        for (int i = 0, x = 0; x < chunks.x; x++)
        {
            for (int z = 0; z < chunks.y; z++)
            {
                GameObject current = new GameObject("Terrain" + " (" + new Vector2(x * 128, z * 128) + ")", 
                    typeof(TerrainGeneration),
                    typeof(MeshRenderer),
                    typeof(MeshFilter),
                    typeof(MeshCollider)
                    );
                current.transform.parent = transform;
                current.transform.position = new Vector3(x * (chunkResolution.x) * (128 / chunkResolution.x),
                    0f,
                    z * (chunkResolution.y) * (128 / chunkResolution.y));
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
