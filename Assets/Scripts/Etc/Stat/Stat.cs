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
    }


    public void SetStat(int level = 1) // ������ �����ϴ� �Լ�
    {
        if (!Managers.Data.StatDict.TryGetValue(level, out Contents.Stat stat)) // ���� StatDict�� ���� �ִٸ�
        {
            return;
        }

        if (this.GetComponent<Stat>() is PlayerStat) // �÷��̾��� 
        {
            _hp = _maxHp = stat.maxHp;
            _mp = _maxMp = stat.maxMp;
            _attack = stat.attack + (Managers.Data.ItemDict[Managers.Data.PlayerData.equippedWeapon].Attack); // ���� ���� ���� ���ݷ� �ɼ� ������
            _defense = stat.defense;
        }
        else
        {

            EnemyController enemyController = this.GetComponent<EnemyController>();
            BossAIController bossController = this.GetComponent<BossAIController>();
            Define.EnemyType enemyType = enemyController != null ? enemyController.EnemyType : bossController.EnemyType; // � ��Ʈ�ѷ����� ���� ���� Ÿ�� ���ϱ�

            switch (enemyType)
            {
                case Define.EnemyType.Skelton:
                    Hp = MaxHp = 200;
                    Mp = MaxMp = 200;
                    Attack = 15;
                    Defense = 10;
                    break;
                case Define.EnemyType.Boss:
                    Hp = MaxHp = 2000;
                    Mp = MaxMp = 2000;
                    Attack = 100;
                    Defense = 60;
                    break;
            }
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

    public void Attacked(Stat attackObject, GameObject target,int skillDamage = 0) // ��ų �������� ���ϴ°��� �⺻ �������� �Ķ���ͷ� �Ǵ�
    {

        if (target == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Target is null");
#endif
            return;
        }
        
        int damage = 0;
        if (skillDamage==0) // ��ų ������ �ƴ� ��
        {
            damage= Mathf.Max(0, attackObject.Attack - Defense);
        }
        else
        {
            damage = Mathf.Max(0, skillDamage - Defense);
        }
        Hp -= damage;

        if (Hp <= 0)
        {
            Hp = 0;
            if (attackObject is PlayerStat) // �÷��̾� ����ġ�� ����
            {
                // �÷��̾ �����ѰŶ��
                if (Managers.Data.EnemyExpDict.TryGetValue(target.gameObject.tag,
                     out Contents.ExpData tempExpData))
                {
                    Managers.Game.GetPlayer().GetComponent<PlayerStat>().Exp = tempExpData.Exp;

#if UNITY_EDITOR
                    Debug.Log(Managers.Game.GetPlayer().GetComponent<PlayerStat>().TotalExp);
#endif
                }
            }
        }
    }

}
