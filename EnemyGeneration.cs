using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGeneration : MonoBehaviour
{
    public GameObject[] enemylist;
    public Transform[] spawnPoints;

    public float SPAWN_CHANCE = 0.7f;

    void Start()
    {
        foreach (Transform spawn in spawnPoints)
        {
            if (Random.Range(0.0f, 1.0f) > SPAWN_CHANCE)
            {
                continue;
            }

            GameObject enemy = Instantiate(enemylist[Random.Range(0, enemylist.Length)], spawn.position, Quaternion.identity);
            enemy.transform.parent = spawn;
        }
    }
}
