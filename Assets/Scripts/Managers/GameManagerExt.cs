using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManagerExt
{
    GameObject Player { get; set; }

    public GameObject SpawnPlayer(Vector3 spawnPos)
    {
        Player = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Player"), spawnPos, Quaternion.identity);

       return Player ?? null; // 널이라면 널 리턴
    }

    public GameObject GetPlayer()
    {
        return Player ?? null;
    }

    public void SetPlayer(GameObject player) // 수정사항
    {
        Player = player;
    }

}
