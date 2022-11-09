using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

    Vector3 _destPos;

    [SerializeField]
    Define.State _state = Define.State.Idle;
    NavMeshAgent nma;

    public GameObject _player; // 임시로 설정

    float _findRange = 5f;

    Vector3 re2Pos;

    public Define.State State
    {
        get { return _state; }

        set
        {
            _state = value;

            Animator anim = GetComponent<Animator>();

            switch (_state)
            {
                case Define.State.Idle:
                    anim.CrossFade("Idle", 0.1f);
                    break;
                case Define.State.Moving:
                    anim.CrossFade("Run", 0.1f);
                    break;
                case Define.State.Attack:
                    anim.CrossFade("Attack", 0.1f);
                    break;
                case Define.State.Die:
                    //  anim.CrossFade("Die", 0.1f);
                    break;
            }

        }
    }

    void Start()
    {
        Init();

    }


    void Update()
    {
        switch (State)
        {
            case Define.State.Die:
                //Die();
                break;
            case Define.State.Moving:
                Movig();
                break;
            case Define.State.Idle:
                Idle();
                break;
            case Define.State.Attack:
                Attacking();
                break;
        }
    }

    void Init()
    {
        nma = gameObject.GetComponent<NavMeshAgent>();
        nma.speed = 2.5f;// 임시로 이동속도 설정
        re2Pos = transform.position;
    }


    void Idle()
    {
        if (_player == null)
            return;
        float dis = (_player.transform.position - transform.position).magnitude;

        if (dis <= _findRange)
        {
            State = Define.State.Moving;
            return;
        }


    }
    void Movig()
    {
        if (_player == null)
            return;

        _destPos = _player.transform.position;

        Vector3 dir = _destPos - transform.position;

        // dir.y = 0;// 플레이어 위로

        if (dir.magnitude <= nma.stoppingDistance) // 공격할지 판단
        {
            State = Define.State.Attack;
            return;
        }
        if (dir.magnitude <= _findRange)
        {

            nma.SetDestination(_destPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);

            //Vector2 temp = new Vector2(transform.position.z, transform.position.x);
            //Vector2 st = new Vector2(nma.steeringTarget.z, nma.steeringTarget.x);

            //dir = st - temp;
            //float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            //transform.eulerAngles = Vector3.up * ang;
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


    void Attacking()
    {
        if (_player == null)
            return;

        Vector3 dir = _player.transform.position - transform.position;

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
            }
            else
            {
                nma.SetDestination(_player.transform.position);
                State = Define.State.Moving;
            }
        }
    }
}
