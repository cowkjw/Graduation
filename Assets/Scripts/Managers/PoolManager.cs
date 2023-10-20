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
    GameObject _monsterPrefab;
    GameObject _poolManagers;

    public Queue<GameObject> MonsterPool { get; private set; }


    public void LoadTheLastPosition()
    {
        int playerScene = Managers.Data.PlayerData.location;
        if (playerScene == (int)Define.Scene.BossDungeon)
        {
            SceneManager.LoadScene((int)Define.Scene.Dungeon);
        }
        else
        {
            SceneManager.LoadScene(playerScene);
        }
    }

    public void Init()
    {
        if (GameObject.FindObjectOfType<DungeonScene>() is BossDungeonScene) // 던전이 보스 던전이라면
        {
            return;
        }
        _monsterPrefab = Resources.Load<GameObject>("Prefabs/Skelton");
        _poolManagers = new GameObject { name = "@PoolManagers" };
        MonsterPool = new Queue<GameObject>();
        foreach (var data in Managers.Data.EnemyDict)
        {
            if (_monsterPrefab == null)
            {
#if UNITY_EDITOR
                Debug.LogError("몬스터 프리팹 NULL");
#endif
                return;
            }
            GameObject monster = GameObject.Instantiate(_monsterPrefab, data.Value.ToVecotr3(), Quaternion.identity);
            monster.name = data.Key;
            monster.SetActive(false);
            MonsterPool.Enqueue(monster);
            monster.transform.SetParent(_poolManagers.transform);
        }

    }
}
