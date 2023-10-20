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


    public void SetStat(int level = 1) // 스탯을 셋팅하는 함수
    {
        if (!Managers.Data.StatDict.TryGetValue(level, out Contents.Stat stat)) // 만약 StatDict에 값이 있다면
        {
            return;
        }

        if (this.GetComponent<Stat>() is PlayerStat) // 플레이어라면 
        {
            _hp = _maxHp = stat.maxHp;
            _mp = _maxMp = stat.maxMp;
            _attack = stat.attack + (Managers.Data.ItemDict[Managers.Data.PlayerData.equippedWeapon].Attack); // 현재 장착 무기 공격력 옵션 더해줌
            _defense = stat.defense;
        }
        else
        {

            EnemyController enemyController = this.GetComponent<EnemyController>();
            BossAIController bossController = this.GetComponent<BossAIController>();
            Define.EnemyType enemyType = enemyController != null ? enemyController.EnemyType : bossController.EnemyType; // 어떤 컨트롤러인지 따라서 몬스터 타입 정하기

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

    public void ResetStat() // 하드코딩 해둠 다음에 바꾸기
    {
        Hp = 200;
        MaxHp = 200;
        Mp = 200;
        MaxMp = 200;
    }

    private void Awake() // 실제 빌드시에 hp = 0으로 설정 되는 경우가 발생해서 start보다 먼저 실행
    {
        Init();
    }

    public void Attacked(Stat attackObject, GameObject target,int skillDamage = 0) // 스킬 공격으로 당하는건지 기본 공격인지 파라미터로 판단
    {

        if (target == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Target is null");
#endif
            return;
        }
        
        int damage = 0;
        if (skillDamage==0) // 스킬 공격이 아닐 때
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
            if (attackObject is PlayerStat) // 플레이어 경험치를 위한
            {
                // 플레이어가 공격한거라면
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
