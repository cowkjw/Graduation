using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : BaseCharacterController
{

    public GameObject[] _coin;

    Vector3 re2Pos; // 원래 위치로
    NavMeshAgent nma;

    float _findRange = 5f;

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
        _stat = GetComponent<Stat>();
        _target = Managers.Game.GetPlayer();
        nma = gameObject.GetComponent<NavMeshAgent>();
        nma.speed = 2.5f;// 임시로 이동속도 설정
        re2Pos = transform.position;
    }

    protected override void Dying()
    {
        if (_stat.Hp == 0)
        {
            State = Define.State.Die;
            StartCoroutine(DropCoin());
            Destroy(gameObject, 3f);
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

        dir.y = 0;// 플레이어 위로 방지

        if (dir.magnitude <= nma.stoppingDistance) // 공격할지 판단
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
            Vector3 checkRange = re2Pos - transform.position; // 원래 위치와 비교하기 위한 변수
            if (checkRange.magnitude <= 1f)
            {
                State = Define.State.Idle;
                return;
            }
            nma.SetDestination(re2Pos);
        }
    }


    protected override void Attacking()
    {
        if (_target == null)
            return;

        Vector3 dir = _target.transform.position - transform.position;

        if (dir.magnitude > _findRange)
        {
            nma.SetDestination(re2Pos);
            State = Define.State.Idle;
            return;
        }
        else if (dir.magnitude <= _findRange)
        {
            if (dir.magnitude < nma.stoppingDistance)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
               // State = Define.State.Attack;
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
            PlayerStat playerStat = _target.GetComponent<PlayerStat>(); // 플레이어 스탯 가져옴
            playerStat.Attacked(_stat); // 몬스터의 스탯을 넘겨줌
          //  _scene.SetPlayerHp(playerStat.Hp, playerStat.MaxHp);
        }
    }

    IEnumerator DropCoin()
    {
        int itemCnt = Random.Range(5, 15);

        for (int i = 0; i < itemCnt; i++)
        {
            int idx = Random.Range(0, 3);
            float randX = Random.Range(-0.5f, 0.5f);
            float randZ = Random.Range(-0.5f, 0.5f);

            yield return new WaitForSeconds(0.1f);
            Instantiate(_coin[idx], transform.position + new Vector3(randX, 0, randZ), Quaternion.identity);
        }

    }
}
