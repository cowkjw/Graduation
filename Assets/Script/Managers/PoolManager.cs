using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System;

public class PoolManager : MonoBehaviour
{
    GameObject monsterPrefab;
    Dictionary<string, Queue<GameObject>> monsterPool;

    private void Awake()
    {
        monsterPrefab = Resources.Load<GameObject>("Prefabs/Skelton");
        monsterPool = new Dictionary<string, Queue<GameObject>>();

        Queue<GameObject> pool = new Queue<GameObject>();
        foreach (var data in Managers.Data.enemyDict)
        {
            GameObject monster = Instantiate(monsterPrefab, data.Value.ToVecotr3(), Quaternion.identity);
            monster.name = data.Key;
            monster.SetActive(false);
            pool.Enqueue(monster);
            monsterPool.Add(data.Key, pool);
        }
    }


    public GameObject SpawnMonster(string name, Vector3 position)
    {
        if (monsterPool.ContainsKey(name) && monsterPool[name].Count > 0)
        {
            GameObject monster = monsterPool[name].Dequeue();
            monster.transform.position = position;
            monster.SetActive(true);
            return monster;
        }
        else
        {
            GameObject monster = Instantiate(monsterPrefab, position, Quaternion.identity);
            return monster;
        }
    }

    public void ReturnMonster(GameObject monster)
    {
        monster.SetActive(false);
        monsterPool[monster.name].Enqueue(monster);
    }

}
