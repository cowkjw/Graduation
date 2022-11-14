using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerExt : MonoBehaviour
{
    GameObject _player;

    public GameObject SpawnPlayer(Vector3 spawnPos)
    {

        _player = Instantiate(Resources.Load<GameObject>("Prefabs/Player"));
        _player.transform.position = spawnPos;

        if (_player != null)
        {
            return _player;
        }
        else
        {
            return null;
        }
    }

    public GameObject GetPlayer()
    {
        if (_player != null)
        {
            return _player;
        }
        else
        {
            return null;
        }

    }
}
