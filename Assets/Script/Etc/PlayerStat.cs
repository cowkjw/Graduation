using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : Stat
{


    [SerializeField]
    int _totalExp;

    public int Exp
    {
        set
        {
            _totalExp += value; // ���ݱ��� ���� ���� ���� ������Ʈ 
        }
    }
    public int TotalExp { get => _totalExp; private set => _totalExp = value; }

    protected override void Init()
    {
        base.Init();
        Attack = 30;
        Hp = 500;
        MaxHp = 500;
    }

}
