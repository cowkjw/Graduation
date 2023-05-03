using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class PoolManager
{
    GameObject monsterPrefab;
    GameObject poolManagers;

    public Queue<GameObject> monsterPool { get; private set; }


    public void LoadTheLastPosition()
    {
        int playerScene = Managers.Data.PlayerData.location;
        SceneManager.LoadScene(1);
       // SceneManager.LoadScene(playerScene);
    }

    public void Init()
    {
        if(GameObject.FindObjectOfType<DungeonScene>() is BossDungeonScene) // 던전이 보스 던전이라면
        {
            return; 
        }
        monsterPrefab = Resources.Load<GameObject>("Prefabs/Skelton");
        poolManagers = new GameObject { name = "@PoolManagers" };
        monsterPool = new Queue<GameObject>();
        foreach (var data in Managers.Data.EnemyDict)
        {
            if (monsterPrefab == null)
            {
#if UNITY_EDITOR
                Debug.LogError("몬스터 프리팹 NULL");
#endif
                return;
            }
            GameObject monster = GameObject.Instantiate(monsterPrefab, data.Value.ToVecotr3(), Quaternion.identity);
            monster.name = data.Key;
            monster.SetActive(false);
            monsterPool.Enqueue(monster);
            monster.transform.SetParent(poolManagers.transform);
        }

    }



}
