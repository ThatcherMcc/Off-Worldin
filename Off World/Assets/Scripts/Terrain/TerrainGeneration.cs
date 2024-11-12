using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Mathematics;

public class TerrainGeneration : MonoBehaviour
{
    ChunkGeneration chunkGen;

    Mesh mesh;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider collider;
    Vector2[] uv;

    private void Start()
    {
        chunkGen = GameObject.FindGameObjectWithTag("Manager").GetComponent<ChunkGeneration>();
        if (chunkGen == null )
        {
            return;
        }

        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        collider = GetComponent<MeshCollider>();

        meshRenderer.material = chunkGen.terrainMaterial;

        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        mesh = new Mesh();
        Vector3[] vertices = new Vector3[(int)((chunkGen.chunkResolution.x + 1) * (chunkGen.chunkResolution.y + 1))];
        uv = new Vector2[vertices.Length];
        int[] triangles;

        for (int i = 0, x = 0; x <= chunkGen.chunkResolution.x; x++)
        {
            for (int z = 0; z <= chunkGen.chunkResolution.y; z++)
            {
                float y = Noise(x, z, BiomeNoise(x, z));
                vertices[i] = new Vector3(x * (128 / chunkGen.chunkResolution.x),
                    y,
                    z * (128 / chunkGen.chunkResolution.y)
                    );
                float doesSpawn = Mathf.PerlinNoise(x + transform.position.x + chunkGen.seed, z + transform.position.z + chunkGen.seed);
                doesSpawn -= Mathf.PerlinNoise((x + transform.position.x) * 0.5f + chunkGen.seed, (z + transform.position.z) * 0.5f + chunkGen.seed);
                if (doesSpawn > chunkGen.treeThreshold && y > chunkGen.waterLevel + 5)
                {
                    float whatSpawns = Mathf.PerlinNoise(x + transform.position.x + (chunkGen.seed * 5), z + transform.position.z + (chunkGen.seed * 3));
                    whatSpawns = whatSpawns * chunkGen.trees.Length;
                    whatSpawns = Mathf.RoundToInt(whatSpawns);
                    float offset = UnityEngine.Random.Range(-2f, 2f);
                    offset = offset / 2;
                    int whatTree = UnityEngine.Random.Range(0, 2);
                    GameObject current = Instantiate(chunkGen.trees[whatTree], new Vector3(x * (128 / chunkGen.chunkResolution.x) + transform.position.x + offset,
                       y + transform.position.y,
                       z * (128 / chunkGen.chunkResolution.y) + transform.position.z + offset),
                       Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0)
                       );
                    current.transform.parent = transform;
                }
                i++;
            }
        }

        for (int i = 0; i < uv.Length; i++)
        {
            uv[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        triangles = new int[(int)(chunkGen.chunkResolution.x * chunkGen.chunkResolution.y * 6)];

        int tris = 0;
        int vert = 0;

        for (int x = 0; x < chunkGen.chunkResolution.y; x++)
        {
            for ( int z = 0; z < chunkGen.chunkResolution.x; z++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = (int)(vert + chunkGen.chunkResolution.x + 1);
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = (int)(vert + chunkGen.chunkResolution.x + 1);
                triangles[tris + 5] = (int)(vert + chunkGen.chunkResolution.x + 2);

                vert++;
                tris += 6;
            }

            vert++;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateBounds();
        mesh.triangles = mesh.triangles.Reverse().ToArray();
        meshFilter.mesh = mesh;
        mesh.RecalculateNormals();
        collider.sharedMesh = mesh;
    }

    float Noise(float x, float z, float biomeNoise)
    {
        // Base Plate of Noise
        float y = biomeNoise * 200f;
        // Mountains
        float multiplier = 1 + Mathf.Pow(biomeNoise, 3) * 20f;
        y = y * multiplier;
        y += (Mathf.PerlinNoise(((x * (128 / chunkGen.chunkResolution.x)) + transform.position.x + chunkGen.seed) * 0.003f,
            ((z * (128 / chunkGen.chunkResolution.y)) + transform.position.z + chunkGen.seed) * 0.003f) * 200) * biomeNoise;
        // Hills
        y += (Mathf.PerlinNoise(((x * (128 / chunkGen.chunkResolution.x)) + transform.position.x + chunkGen.seed) * 0.007f,
            ((z * (128 / chunkGen.chunkResolution.y)) + transform.position.z + chunkGen.seed) * 0.007f) * 70) * biomeNoise;
        return y;
    }

    float BiomeNoise(float x, float z)
    {
        // Base Plate of Noise. Quite Spread out but not too much
        float y = Mathf.PerlinNoise(((x * (128 / chunkGen.chunkResolution.x)) + transform.position.x + chunkGen.seed) * 0.0002f,
            ((z * (128 / chunkGen.chunkResolution.y)) + transform.position.z + chunkGen.seed) * 0.0002f);
        // Adding more noise
        y += Mathf.PerlinNoise(((x * (128 / chunkGen.chunkResolution.x)) + transform.position.x + chunkGen.seed) * 0.0007f,
            ((z * (128 / chunkGen.chunkResolution.y)) + transform.position.z + chunkGen.seed) * 0.0008f);
        // Minusing Noise. Multiply by 2 so it can stack up to the first 2 additions
        y -= Mathf.PerlinNoise(((x * (128 / chunkGen.chunkResolution.x)) + transform.position.x + chunkGen.seed) * 0.000008f,
            ((z * (128 / chunkGen.chunkResolution.y)) + transform.position.z + chunkGen.seed) * 0.000008f) * 2;
        y = Mathf.Clamp(y, -1, 1);
        return y;
    }
}
