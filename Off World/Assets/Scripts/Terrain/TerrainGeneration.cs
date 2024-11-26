using System.Collections;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;
using Cinemachine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class TerrainGeneration : MonoBehaviour
{
    ChunkGeneration chunkGen;
    GameObject player;
    private bool canSpawnPlayer = false;
    private GameObject spawner;
    private bool canSpawnSpawner = true;
    private GameObject ship;
    private bool canSpawnShip = false;
    
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
        spawner = chunkGen.spawner;
        player = chunkGen.player;
        ship = chunkGen.ship;
        

        string name = transform.gameObject.name;
        string[] names = name.Split(',');
        float firstNum = float.Parse(Regex.Replace(names[0], @"[^\d.]", ""));
        //Debug.Log(firstNum);
        float secondNum = float.Parse(Regex.Replace(names[1], @"[^\d.]", ""));
        //Debug.Log(secondNum);
        if (chunkGen.chunks.x / 2f - 3 < firstNum && 
            firstNum < chunkGen.chunks.x / 2f + 3 &&
            chunkGen.chunks.y / 2f - 3 < secondNum &&
            secondNum < chunkGen.chunks.y / 2f + 3)
        {
            canSpawnPlayer = true;
            canSpawnShip = true;
            //Debug.Log("CANSPAWN");
        }

        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        collider = GetComponent<MeshCollider>();

        meshRenderer.material = chunkGen.terrainMaterial;

        GenerateTerrain();
        Invoke("SetSpawnerPlayer", 6f);
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
                
                // player spawning
                if (canSpawnPlayer && !chunkGen.playerSpawned && y > chunkGen.waterLevel + 15 && y < chunkGen.waterLevel + 40)
                {
                    GameObject currentPlayer = Instantiate(chunkGen.player, new Vector3(x * (128 / chunkGen.chunkResolution.x) + transform.position.x,
                       y + transform.position.y + 1f,
                       z * (128 / chunkGen.chunkResolution.y) + transform.position.z),
                       Quaternion.Euler(0, Random.Range(0, 360), 0));

                    CinemachineVirtualCamera virtualCam = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
                    virtualCam.Follow = currentPlayer.transform.Find("OrientationCam").transform;
                    currentPlayer.GetComponent<PlayerHealth>().healthbar = GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(1).GetChild(0).GetComponent<Healthbar>();
                    chunkGen.playerSpawned = true;
                    player = currentPlayer;
                    chunkGen.player = currentPlayer;
                    i++;
                    continue;
                    //Debug.Log("SPAWNED");
                }
                if (canSpawnShip && !chunkGen.shipSpawned && chunkGen.playerSpawned && y > chunkGen.waterLevel + 15 && y < chunkGen.waterLevel + 40)
                {
                    GameObject currentShip = Instantiate(chunkGen.ship, new Vector3(x * (128 / chunkGen.chunkResolution.x) + transform.position.x,
                       y + transform.position.y + 1.5f,
                       z * (128 / chunkGen.chunkResolution.y) + transform.position.z),
                       Quaternion.Euler(0, Random.Range(0, 360), 0));

                    currentShip.transform.GetChild(0).GetChild(0).GetComponent<TeleportDoor>().player = player;
                    currentShip.transform.GetChild(0).GetChild(1).GetComponent<TeleportDoor>().player = player;
                    chunkGen.shipSpawned = true;
                    ship = currentShip;
                    chunkGen.ship = currentShip;
                    i++;
                    continue;
                }

                // tree spawning
                if (doesSpawn > chunkGen.treeThreshold && y > chunkGen.waterLevel + 20)
                {
                    float whatSpawns = Mathf.PerlinNoise(x + transform.position.x + (chunkGen.seed * 5), z + transform.position.z + (chunkGen.seed * 3));
                    whatSpawns = whatSpawns * chunkGen.trees.Length;
                    whatSpawns = Mathf.RoundToInt(whatSpawns);

                    float offset = Random.Range(-2f, 2f);
                    offset = offset / 2;

                    int whatTree = Random.Range(0, 2);

                    GameObject current = Instantiate(chunkGen.trees[whatTree], new Vector3(x * (128 / chunkGen.chunkResolution.x) + transform.position.x + offset,
                       y + transform.position.y - 0.85f,
                       z * (128 / chunkGen.chunkResolution.y) + transform.position.z + offset),
                       Quaternion.Euler(0, Random.Range(0, 360), 0));

                    current.transform.parent = transform;
                }

                doesSpawn += Mathf.PerlinNoise((x + transform.position.x) * 0.03f + chunkGen.seed, (z + transform.position.z) * 0.03f + chunkGen.seed) * 0.35f;
                // spawner spawning
                if (canSpawnSpawner && doesSpawn > chunkGen.spawnerThreshold && y < chunkGen.waterLevel + 10 && y > chunkGen.waterLevel)
                {
                    GameObject current = Instantiate(spawner, new Vector3(x * (128 / chunkGen.chunkResolution.x) + transform.position.x,
                       y + transform.position.y + 1f,
                       z * (128 / chunkGen.chunkResolution.y) + transform.position.z),
                       Quaternion.identity);
                    current.transform.parent = transform;

                    spawner = current;
                    canSpawnSpawner = false;

                    Debug.Log("Spawner made and added");
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

    private bool ValidateSeed()
    {
        int landCount = 0;
        int waterCount = 0;

        // Test the terrain across a sample grid
        for (int x = 0; x < chunkGen.chunkResolution.x; x += 10) // Sample every 10 units for efficiency
        {
            for (int z = 0; z < chunkGen.chunkResolution.y; z += 10)
            {
                float y = Noise(x, z, BiomeNoise(x, z));
                if (y > chunkGen.waterLevel) landCount++;
                else waterCount++;
            }
        }

        float landRatio = (float)landCount / (landCount + waterCount);
        Debug.Log($"Land ratio for seed {chunkGen.seed}: {landRatio}");

        // Return true if enough land is available
        return landRatio >= 0.5f; // Adjust this threshold as needed
    }

    float Noise(float x, float z, float biomeNoise)
    {
        Vector2 noiseVector = new Vector2((x * (128 / chunkGen.chunkResolution.x)) + transform.position.x + chunkGen.seed,
            (z * (128 / chunkGen.chunkResolution.y)) + transform.position.z + chunkGen.seed);
        // Base Plate of Noise
        float y = biomeNoise * 60f;

        // Mountains
        float multiplier = 1 + Mathf.Pow(biomeNoise, 10f) * 1.2f;
        y *= multiplier;
        y += (Mathf.PerlinNoise(noiseVector.x * 0.0002f, noiseVector.y * 0.0002f) * 55) * biomeNoise;
        // Hills
        y -= (Mathf.PerlinNoise(noiseVector.x * 0.01f, noiseVector.y * 0.01f) * 25) * biomeNoise;
        y -= (Mathf.PerlinNoise(noiseVector.x * 0.00016f, noiseVector.y * 0.00016f) * 60) * biomeNoise;

        return y;
    }

    float BiomeNoise(float x, float z)
    {
        Vector2 noiseVector = new Vector2((x * (128 / chunkGen.chunkResolution.x)) + transform.position.x + chunkGen.seed,
            (z * (128 / chunkGen.chunkResolution.y)) + transform.position.z + chunkGen.seed);
        // Base Plate of Noise. Quite Spread out but not too much
        float y = Mathf.PerlinNoise(noiseVector.x * 0.0016f, noiseVector.y * 0.0016f);
        // Adding more noise
        y += Mathf.PerlinNoise(noiseVector.x * 0.0003f, noiseVector.y * 0.0003f);
        // Minusing Noise (Water). Multiply by 2 so it can stack up to the first 2 additions
        y -= Mathf.PerlinNoise(noiseVector.x * 0.0018f, noiseVector.y * 0.0018f) * 1.9f;
        y = Mathf.Clamp(y, -1, 1);
        return y;
        //26213
    }

    private void SetSpawnerPlayer()
    {
        if (player != null)
        {
            Debug.Log("terrain player Not null");
            player = chunkGen.player;
            spawner.GetComponent<Spawner>().player = player;
            Debug.Log(spawner.name + " Spawner assigned??");
            
        }
        
    }
    Vector3 GetTerrainNormal(int x, int z)
    {
        // Get the neighboring vertices to calculate the slope
        float yL = Noise(x - 4, z, BiomeNoise(x - 4, z));
        float yR = Noise(x + 4, z, BiomeNoise(x + 4, z));
        float yD = Noise(x, z - 4, BiomeNoise(x, z - 4));
        float yU = Noise(x, z + 4, BiomeNoise(x, z + 4));

        // Calculate vectors from the neighbors
        Vector3 leftRight = new Vector3(2, yR - yL, 0);  // Left to Right vector
        Vector3 downUp = new Vector3(0, yU - yD, 2);     // Down to Up vector

        // Compute the cross product to get the normal
        Vector3 normal = Vector3.Cross(leftRight, downUp).normalized;

        return normal;
    }

}
