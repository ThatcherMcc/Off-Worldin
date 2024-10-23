using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGeneration : MonoBehaviour
{
    public Vector2 chunks;
    public Vector2 chunkResolution;

    public Material terrainMaterial;

    private void Start()
    {
        StartCoroutine(GenerateChunks());
    }

    public IEnumerator GenerateChunks()
    {
        for (int x = 0; x < chunks.x; x++)
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
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }
    }
}
