using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : BaseCharacterController
{

    public GameObject[] Coin;
    public Define.EnemyType EnemyType { get; protected set; }
    public Contents.ExpData EnemyExp { get; private set; }

    protected NavMeshAgent Nma;
    protected float FindRange = 5f;
    protected Vector3 OriginalPostition; // ���� ��ġ��

    protected override void Update()
    {
        if (State != Define.State.Die)
            Dying();

        switch (State)
        {
            case Define.State.Moving:
                Moving();
                break;
            case Define.State.Idle:
                Idle();
                break;
            case Define.State.Attack:
                Attacking();
                break;
        }
    }

    protected override void Init()
    {

        if (Managers.Data.EnemyExpDict.TryGetValue(gameObject.tag, out Contents.ExpData tempExpData))
        {
            EnemyExp = tempExpData;

        }
        Stat = GetComponent<Stat>();
        Target = Managers.Game.GetPlayer();
        Nma = gameObject.GetComponent<NavMeshAgent>();
        Nma.speed = 2.5f;// �ӽ÷� �̵��ӵ� ����
        OriginalPostition = transform.position;
        State = Define.State.Idle;
        EnemyType = Define.EnemyType.Skelton;

    }

    void OnEnable()
    {
        Init();
        this.GetComponent<CapsuleCollider>().isTrigger = false; // ������ �Ǹ鼭 Ʈ���� false�� �ٲ�
        StopAllCoroutines();
    }

    protected override void Dying()
    {
        if (Stat.Hp <= 0)
        {
            State = Define.State.Die;
            this.GetComponent<CapsuleCollider>().isTrigger = true; // ���� ���·� �÷��̾ ���� �ʰ� �ϱ� ���� Ʈ���� on
            StartCoroutine(DropCoin());
        }
        else
            return;
    }

    protected void Idle()
    {
        if (Target == null)
            return;
        float dis = (Target.transform.position - transform.position).magnitude;

        if (dis <= FindRange)
        {
            State = Define.State.Moving;
            return;
        }
    }

    protected override void Moving()
    {
        if (Target == null)
            return;

        DestPos = Target.transform.position;

        Vector3 dir = DestPos - transform.position;

        dir.y = 0;// �÷��̾� ���� ����

        if (dir.magnitude <= Nma.stoppingDistance) // �������� �Ǵ�
        {
            State = Define.State.Attack;
            return;
        }
        if (dir.magnitude <= FindRange)
        {
            Nma.SetDestination(DestPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        }
        else
        {
            Vector3 checkRange = OriginalPostition - transform.position; // ���� ��ġ�� ���ϱ� ���� ����
            if (checkRange.magnitude <= 1f)
            {
                State = Define.State.Idle;
                return;
            }
            Nma.SetDestination(OriginalPostition);
        }
    }


    protected override void Attacking()
    {
        if (Target == null)
            return;

        Vector3 dir = Target.transform.position - transform.position;

        if (dir.magnitude > FindRange) // Ÿ���� �־�����
        {
            Nma.SetDestination(OriginalPostition); // ���� �ڸ��� 
            State = Define.State.Idle;
            return;
        }
        else if (dir.magnitude <= FindRange) // ���� �ȿ� �ִٸ�
        {
            if (dir.magnitude < Nma.stoppingDistance) // ���ߴ� �����ȿ� �ִٸ�
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
            }
            else // �ƴ϶�� Ÿ�� ��ġ���� �̵�
            {
                Nma.SetDestination(Target.transform.position);
                State = Define.State.Moving;
            }
        }
    }

    protected virtual void PlayerHit()
    {
        if (Target != null)
        {
            PlayerStat playerStat = Target.GetComponent<PlayerStat>(); // �÷��̾� ���� ������
            playerStat.Attacked(Stat, Target); // ������ ������ �Ѱ���
        }
    }

    void ResetStatus()
    {

        Stat.ResetStat();
        transform.position = OriginalPostition;

    }

    IEnumerator Disable()
    {
        Managers.Pool.MonsterPool.Enqueue(gameObject);
        Target = null; // �÷��̾� Ÿ�� null ó�� 
        yield return new WaitForSeconds(3f);
        ResetStatus();
        this.gameObject.SetActive(false);
    }

    protected virtual IEnumerator DropCoin()
    {
        int itemCnt = Random.Range(5, 10);

        for (int i = 0; i < itemCnt; i++)
        {
            int idx = Random.Range(0, 3);
            float randX = Random.Range(-0.5f, 0.5f);
            float randZ = Random.Range(-0.5f, 0.5f);

            yield return new WaitForSeconds(0.1f);
            Instantiate(Coin[idx], transform.position + new Vector3(randX, 0, randZ), Quaternion.identity);
        }
        StartCoroutine(Disable());
    }
}
