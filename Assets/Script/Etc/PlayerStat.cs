using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : Stat
{


    [SerializeField]
    int _totalExp;
  

    int _level;

    LevelUpText levelUpText;
    public int Exp
    {
        set
        {
            _totalExp += value; // ���ݱ��� ���� ���� ���� ������Ʈ 

            if (Managers.Data.StatDict.TryGetValue(_level+1, out Contents.Stat stat))
            {
                if(_totalExp>=stat.totalExp)
                {
                    LevelUp();
                    Debug.Log($"���� {_level}");
                }
            }
        }
    }
    public int TotalExp { get => _totalExp; private set => _totalExp = value; }

    protected override void Init()
    {
        base.Init();
        levelUpText = FindObjectOfType<LevelUpText>();
        _level = 1;
        if (Managers.Data.StatDict.TryGetValue(_level, out Contents.Stat stat))
        {
            _totalExp = stat.totalExp;
        }
        Attack = 30;
        Hp = 500;
        MaxHp = 500;
    }


    void LevelUp()
    {
        _level++;
        SetStat(_level);
        Attack += 10;
        Hp += 50;
        MaxHp += 50;
        levelUpText.gameObject.SetActive(true);
        gameObject.GetComponent<PlayerController>().LevelUpEffect();
    }

}
