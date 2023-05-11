using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : Stat
{

    [SerializeField]
    int _totalExp;
    int _level;
    public int TotalExp { get => _totalExp; private set => _totalExp = value; }
    public int Level { get => _level; }


    LevelUpUI levelUpUI;
    public int Exp
    {
        set // 데이터를 불러와 해당 레벨까지 계속 레벨업 하도록 수정 
        {
            _totalExp += value; // 지금까지 얻은 값을 새로 업데이트 

            if (Managers.Data.StatDict.TryGetValue(_level + 1, out Contents.Stat stat))
            {
                if (_totalExp >= stat.totalExp)
                {
                    LevelUp();
#if UNITY_EDITOR
                    Debug.Log($"레벨 {_level}");
#endif
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
#if UNITY_EDITOR
                Debug.Log($"레벨 {_level}");
#endif
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
        levelUpUI?.InstantiateLevelUpText(); // 널이 아니라면(근데 이게 의미가 있을지 모르겠음)
        gameObject.GetComponent<PlayerController>().LevelUpEffect();
        Managers.Data.PlayerDataChange();
    }

}
