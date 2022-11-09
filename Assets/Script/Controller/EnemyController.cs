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

    public GameObject _player; // �ӽ÷� ����

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
        nma.speed = 2.5f;// �ӽ÷� �̵��ӵ� ����
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

        // dir.y = 0;// �÷��̾� ����

        if (dir.magnitude <= nma.stoppingDistance) // �������� �Ǵ�
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
            Vector3 checkRange = re2Pos - transform.position; // ���� ��ġ�� ���ϱ� ���� ����
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
