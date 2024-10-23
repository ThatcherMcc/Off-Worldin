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
                float y = Noise(x, z);
                vertices[i] = new Vector3(x * (128 / chunkGen.chunkResolution.x),
                    y,
                    z * (128 / chunkGen.chunkResolution.y));
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
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.triangles = mesh.triangles.Reverse().ToArray();
        meshFilter.mesh = mesh;
        collider.sharedMesh = mesh;
    }

    float Noise(float x, float z)
    {
        float y = Mathf.PerlinNoise(((x * (128 / chunkGen.chunkResolution.x)) + transform.position.x) * 0.02f, 
            ((z * (128 / chunkGen.chunkResolution.y)) + transform.position.z) * 0.02f) * 10f;
        y += Mathf.PerlinNoise(((x * (128 / chunkGen.chunkResolution.x)) + transform.position.x) * 0.006f,
            ((z * (128 / chunkGen.chunkResolution.y)) + transform.position.z) * 0.006f) * 70f;
        return y;
    }
}
