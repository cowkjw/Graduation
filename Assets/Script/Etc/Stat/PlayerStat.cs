using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : Stat
{

    [SerializeField]
    int _totalExp;
    public int TotalExp { get => _totalExp; private set => _totalExp = value; }
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

    void Start()
    {
        _totalExp = Managers.Data.PlayerData.playerStat.totalExp;
        while (Managers.Data.StatDict.TryGetValue(_level + 1, out Contents.Stat stat))
        {

            if (_totalExp >= stat.totalExp)
            {
                LevelUp();
                Debug.Log($"���� {_level}");
            }
            else
            {
                break;
            }

        }
    }

    protected override void Init()
    {
        base.Init();
        levelUpUI = FindObjectOfType<LevelUpUI>();
        _level = Managers.Data.PlayerData.playerStat.level;
        if (Managers.Data.StatDict.TryGetValue(_level, out Contents.Stat stat))
        {
            SetStat(_level);
        }
    }

    void LevelUp()
    {
        _level++;
        SetStat(_level);
        levelUpUI?.InstantiateLevelUpText(); // ���� �ƴ϶��(�ٵ� �̰� �ǹ̰� ������ �𸣰���)
        gameObject.GetComponent<PlayerController>().LevelUpEffect();
        Managers.Data.PlayerDataChange();
    }

}
