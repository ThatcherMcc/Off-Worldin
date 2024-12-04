using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] mobs;
    private GameObject mob;
    private int numMobs;
    private List<GameObject> mobsSpawned = new List<GameObject>();
    private float count = 0;

    public GameObject player;
    private float distanceToSpawn = 200;
    private bool spawned = false;

    public MobType mobType;
    public enum MobType
    {
        Frog,
        Wolf
    }


    // Start is called before the first frame update
    void Start()
    {
        mob = GetMobByType(mobType);
        //Debug.Log(mob);
    }

    private void Update()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            if (!spawned && distance < distanceToSpawn && count >= 10)
            {
                SpawnMob();
            } else if (distance > distanceToSpawn + 20 && spawned)
            {
                ClearMobs();
            }
            count += Time.deltaTime;
        }
        
    }

    private GameObject GetMobByType(MobType type)
    {
        switch (type)
        {
            case MobType.Frog:
                numMobs = 1;
                return mobs.Length > 0 ? mobs[0] : null;
            case MobType.Wolf:
                numMobs = 1;
                return mobs.Length > 0 ? mobs[1] : null;
            default: 
                return null;
        }
    }

    private void SpawnMob()
    { 
        spawned = true;
        count = 0;
        for (int i = 0; i < numMobs; i++) 
        {
            float angle = Random.Range(0, 360);
            Vector3 randomDirection = new Vector3(Mathf.Cos(angle),0 , Mathf.Sin(angle)).normalized;
            GameObject current = Instantiate(mob, transform.position + (randomDirection * Random.Range(0, 5)), Quaternion.identity);
            current.GetComponent<FrogAI>().player = player.transform;
            mobsSpawned.Add(current);
            //Debug.Log("Spawning Mobs");
        }
        //Debug.Log("Spawned = True");
    }

    private void ClearMobs()
    {
        foreach (GameObject mob in mobsSpawned)
        {
            if (mob != null)
            {
                Destroy(mob);
            }
        }
        mobsSpawned.Clear();
        spawned = false;
    }

   
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, distanceToSpawn);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanceToSpawn + 20);
    }
}
