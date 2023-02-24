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


    public int Hp { get { return _hp; } set { _hp = value; } }
    public int MaxHp { get { return _maxHp; } set { _maxHp = value; } }
    public int Mp { get { return _mp; } set { _mp = value; } }
    public int MaxMp { get { return _maxMp; } set { _maxMp = value; } }
    public int Attack { get { return _attack; } set { _attack = value; } }
    public int Defense { get { return _defense; } set { _defense = value;  } }

    protected virtual void Init()
    {
        _hp = 100;
        _maxHp = 100;
        _mp = 100;
        _maxMp = 100;
        _attack = 15;
        _defense = 10;
    }

    public void ResetStat()
    {
        Hp = 100;
        MaxHp = 100;
        Mp = 100;
        MaxMp = 100;
        _attack = 15;
        _defense = 10;
    }

    private void Start()
    {
        Init();
    }

    public void Attacked(Stat attackObject)
    {
        int damage = Mathf.Max(0, attackObject.Attack - Defense);
        Hp -= damage;

        if(Hp<=0)
        {
            Hp = 0;
        }
    }
}
