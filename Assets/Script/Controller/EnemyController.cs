using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : BaseCharacterController
{

    public GameObject[] _coin;

    Vector3 originalPostition; // ���� ��ġ��
    NavMeshAgent nma;

    float _findRange = 5f;

    public Contents.ExpData EnemyExp { get; private set; }

    protected override void Update()
    {
        //if (!isUsing)
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
         //   Debug.Log($"{EnemyExp.Exp} "+gameObject.name);
        }

  
        _stat = GetComponent<Stat>();
        _target = Managers.Game.GetPlayer();
        nma = gameObject.GetComponent<NavMeshAgent>();
        nma.speed = 2.5f;// �ӽ÷� �̵��ӵ� ����
        originalPostition = transform.position;
        State = Define.State.Idle;
    }

    void OnEnable()
    {
        Init();
        this.GetComponent<CapsuleCollider>().isTrigger = false; // ������ �Ǹ鼭 Ʈ���� false�� �ٲ�
        StopAllCoroutines();
    }

    protected override void Dying()
    {
        if (_stat.Hp == 0)
        {
            State = Define.State.Die;
            this.GetComponent<CapsuleCollider>().isTrigger = true; // ���� ���·� �÷��̾ ���� �ʰ� �ϱ� ���� Ʈ���� on
            StartCoroutine(DropCoin());
        }
        else
            return;
    }

    void Idle()
    {
        if (_target == null)
            return;
        float dis = (_target.transform.position - transform.position).magnitude;

        if (dis <= _findRange)
        {
            State = Define.State.Moving;
            return;
        }
    }
    protected override void Moving()
    {
        if (_target == null)
            return;

        _destPos = _target.transform.position;

        Vector3 dir = _destPos - transform.position;

        dir.y = 0;// �÷��̾� ���� ����

        if (dir.magnitude <= nma.stoppingDistance) // �������� �Ǵ�
        {
            State = Define.State.Attack;
            return;
        }
        if (dir.magnitude <= _findRange)
        {

            nma.SetDestination(_destPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        }
        else
        {
            Vector3 checkRange = originalPostition - transform.position; // ���� ��ġ�� ���ϱ� ���� ����
            if (checkRange.magnitude <= 1f)
            {
                State = Define.State.Idle;
                return;
            }
            nma.SetDestination(originalPostition);
        }
    }


    protected override void Attacking()
    {
        if (_target == null)
            return;

        Vector3 dir = _target.transform.position - transform.position;

        if (dir.magnitude > _findRange)
        {
            nma.SetDestination(originalPostition);
            State = Define.State.Idle;
            return;
        }
        else if (dir.magnitude <= _findRange)
        {
            if (dir.magnitude < nma.stoppingDistance)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
            }
            else
            {
                nma.SetDestination(_target.transform.position);
                State = Define.State.Moving;
            }
        }
    }


    void PlayerHit()
    {
        if(_target!=null)
        {
            PlayerStat playerStat = _target.GetComponent<PlayerStat>(); // �÷��̾� ���� ������
            playerStat.Attacked(_stat,_target); // ������ ������ �Ѱ���
        }
    }

    void ResetStatus()
    {

        _stat.ResetStat();
        transform.position = originalPostition;
       
    }

    IEnumerator Disable()
    {
        Managers.Pool.monsterPool.Enqueue(gameObject);
        _target = null; // �÷��̾� Ÿ�� null ó�� 
        yield return new WaitForSeconds(3f);
        ResetStatus();
        this.gameObject.SetActive(false);
    }

    IEnumerator DropCoin()
    {
        int itemCnt = Random.Range(5, 10);

        for (int i = 0; i < itemCnt; i++)
        {
            int idx = Random.Range(0, 3);
            float randX = Random.Range(-0.5f, 0.5f);
            float randZ = Random.Range(-0.5f, 0.5f);

            yield return new WaitForSeconds(0.1f);
            Instantiate(_coin[idx], transform.position + new Vector3(randX, 0, randZ), Quaternion.identity);
        }
        StartCoroutine(Disable());
    }
}
