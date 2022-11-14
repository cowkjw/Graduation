using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScene : MonoBehaviour
{
    
    protected Vector3 _playerPos;
    protected GameObject _player =null;

    void Awake()
    {
        Init();
    }

    public virtual void Init()
    {
        Managers.Clear(); // 구독했던거 null처리
    }
}
