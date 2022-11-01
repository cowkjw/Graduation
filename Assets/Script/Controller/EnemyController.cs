using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

    Vector3 _destPos;

    [SerializeField]
    Define.State _state = Define.State.Idle;


    public GameObject _player; // 임시로 설정


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
                    // anim.CrossFade("Idle", 0.1f);
                    break;
                case Define.State.Moving:
                    //   anim.CrossFade("Run", 0.005f);
                    break;
                case Define.State.Attack:
                    //    anim.CrossFade("Attack", 0.005f);
                    break;
                case Define.State.Die:
                    //  anim.CrossFade("Die", 0.1f);
                    break;

            }

        }
    }

    void Start()
    {
        
    }


    void Update()
    {
        Movig();
    }

    void Movig()
    {
        _destPos = _player.transform.position;
        Vector3 dir = _destPos - transform.position;

        dir.y = 0;// 플레이어 위로

        if (dir.magnitude < 0.5f)
        {
            State = Define.State.Idle;
        }
        else
        {
            NavMeshAgent nma = gameObject.GetComponent<NavMeshAgent>();
            nma.SetDestination(_destPos);
            nma.speed = 5f;// 임시로 이동속도 설정

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        }
    }
}
