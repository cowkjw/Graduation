using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    [SerializeField]
    int _hp;
    [SerializeField]
    int _maxHp;
    [SerializeField]
    int _attack;
    [SerializeField]
    int _defense;
    [SerializeField]
    int _mp;
    [SerializeField]
    int _maxMp;


    public int Hp { get { return _hp; } set { _hp = value; } }
    public int MaxHp { get { return _maxHp; } set { _maxHp = value; } }
    public int Mp { get { return _mp; } set { _mp = value; } }
    public int MaxMp { get { return _maxMp; } set { _maxMp = value; } }
    public int Attack { get { return _attack; } set { _attack = value; } }
    public int Defense { get { return _defense; } set { _defense = value;  } }

    private void Start()
    {
        _hp = 100;
        _maxHp = 100;
        _mp = 100;
        _maxMp = 100;
        _attack = 15;
        _defense = 10;
    }

}
