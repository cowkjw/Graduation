using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : Stat
{


    [SerializeField]
    int _totalExp;
  

    int _level;

    LevelUpUI levelUpUI;
    public int Exp
    {
        set
        {
            _totalExp += value; // 지금까지 얻은 값을 새로 업데이트 

            if (Managers.Data.StatDict.TryGetValue(_level+1, out Contents.Stat stat))
            {
                if(_totalExp>=stat.totalExp)
                {
                    LevelUp();
                    Debug.Log($"레벨 {_level}");
                }
            }
        }
    }
    public int TotalExp { get => _totalExp; private set => _totalExp = value; }

    protected override void Init()
    {
        base.Init();
        levelUpUI = FindObjectOfType<LevelUpUI>();
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
        levelUpUI?.InstantiateLevelUpText(); // 널이 아니라면(근데 이게 의미가 있을지 모르겠음)
        gameObject.GetComponent<PlayerController>().LevelUpEffect();
    }

}
