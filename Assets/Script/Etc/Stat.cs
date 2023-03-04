using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    [SerializeField]
    protected int _hp;
    [SerializeField]
    protected int _maxHp;
    [SerializeField]
    protected int _attack;
    [SerializeField]
    protected int _defense;
    [SerializeField]
    protected int _mp;
    [SerializeField]
    protected int _maxMp;


    public int Hp { get => _hp; set => _hp = value; }
    public int MaxHp { get => _maxHp; set => _maxHp = value; }
    public int Mp { get => _mp; set => _mp = value; }
    public int MaxMp { get => _maxMp; set => _maxMp = value; }
    public int Attack { get => _attack; set => _attack = value; }
    public int Defense { get => _defense; set => _defense = value; }

    protected virtual void Init()
    {
        _hp = 100;
        _maxHp = 100;
        _mp = 100;
        _maxMp = 100;
        _attack = 15;
        _defense = 10;
    }

    public void ResetStat() // 하드코딩 해둠 다음에 바꾸기
    {
        Hp = 100;
        MaxHp = 100;
        Mp = 100;
        MaxMp = 100;
    }

    private void Start()
    {
        Init();
    }

    public void Attacked(Stat attackObject,GameObject target)
    {

        if (target == null)
        {
            Debug.LogError("Target is null");
            return;
        }
        int damage = Mathf.Max(0, attackObject.Attack - Defense);
        Hp -= damage;

        if (Hp <= 0)
        {
            Hp = 0;
            if (attackObject is PlayerStat)
            {
                // 플레이어가 공격한거라면
                if (Managers.Data.enemyExpDict.TryGetValue(target.gameObject.tag,
                     out Contents.ExpData tempExpData))
                {
                    Managers.Game.GetPlayer().GetComponent<PlayerStat>().Exp = tempExpData.Exp;

                    Debug.Log(Managers.Game.GetPlayer().GetComponent<PlayerStat>().TotalExp);
                }
            }
        }
    }

}
