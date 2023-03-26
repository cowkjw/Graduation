using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : Stat
{


    [SerializeField]
    int _totalExp;


    int _level;

    public int Level { get => _level; }

    LevelUpUI levelUpUI;
    public int Exp
    {
        set // �����͸� �ҷ��� �ش� �������� ��� ������ �ϵ��� ���� 
        {
            _totalExp += value; // ���ݱ��� ���� ���� ���� ������Ʈ 

            if (Managers.Data.StatDict.TryGetValue(_level + 1, out Contents.Stat stat))
            {
                if (_totalExp >= stat.totalExp)
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
        levelUpUI = FindObjectOfType<LevelUpUI>();
        _level = Managers.Data.playerData.playerStat.level;
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
        levelUpUI?.InstantiateLevelUpText(); // ���� �ƴ϶��(�ٵ� �̰� �ǹ̰� ������ �𸣰���)
        gameObject.GetComponent<PlayerController>().LevelUpEffect();
        Managers.Data.PlayerDataChange();
    }

}
