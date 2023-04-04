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
        SetStat();
        //_hp = 100;
        //_maxHp = 100;
        //_mp = 100;
        //_maxMp = 100;
        //_attack = 15;
        //_defense = 10;
    }


    protected void SetStat(int level = 1) // ������ �����ϴ� �Լ�
    {
        if (!Managers.Data.StatDict.TryGetValue(level, out Contents.Stat stat)) // ���� StatDict�� ���� �ִٸ�
        {
            return;
        }

        if(this.GetComponent<Stat>() is PlayerStat) // �÷��̾��� 
        {
            _maxHp = stat.maxHp;
            _hp = _maxHp;
            _maxMp = stat.maxMp;
            _mp = _maxMp;
            _attack = stat.attack + (Managers.Data.ItemDict[Managers.Data.PlayerData.equippedWeapon].Attack);
            _defense = stat.defense;
        }
        else
        {
            Hp = 200;
            MaxHp = 200;
            Mp = 200;
            MaxMp = 200;
            Attack = 15;
            Defense = 10;
        }
    }

    public void ResetStat() // �ϵ��ڵ� �ص� ������ �ٲٱ�
    {
        Hp = 200;
        MaxHp = 200;
        Mp = 200;
        MaxMp = 200;
    }

    private void Awake() // ���� ����ÿ� hp = 0���� ���� �Ǵ� ��찡 �߻��ؼ� start���� ���� ����
    {
        Init();
    }

    public void Attacked(Stat attackObject, GameObject target)
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
                // �÷��̾ �����ѰŶ��
                if (Managers.Data.EnemyExpDict.TryGetValue(target.gameObject.tag,
                     out Contents.ExpData tempExpData))
                {
                    Managers.Game.GetPlayer().GetComponent<PlayerStat>().Exp = tempExpData.Exp;

                    Debug.Log(Managers.Game.GetPlayer().GetComponent<PlayerStat>().TotalExp);
                }
            }
        }
    }

}
