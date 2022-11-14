using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScene : MonoBehaviour
{
    
    protected Vector3 _playerPos;
    protected GameObject _player;

    void Awake()
    {
        Init();
    }

    public virtual void Init()
    {
        Managers.Clear(); // 备刀沁带芭 null贸府
    }
}
