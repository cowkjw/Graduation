using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

    public GameObject[] _coin;

    Vector3 _destPos;
    Vector3 re2Pos; // 원래 위치로

    [SerializeField]
    Define.State _state = Define.State.Idle;
    NavMeshAgent nma;

    Stat _stat;
    GameObject _player; // 임시로 설정
    //Dungeon1Scene _scene;
    float _findRange = 5f;


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
                    anim.CrossFade("Die", 0.005f);
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
        if (State != Define.State.Die)
            DyingCheck();
        switch (State)
        {
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
        _stat = GetComponent<Stat>();
        _player = Managers.game.GetPlayer();
        nma = gameObject.GetComponent<NavMeshAgent>();
        nma.speed = 2.5f;// 임시로 이동속도 설정
        re2Pos = transform.position;
     //   _scene = FindObjectOfType<BaseScene>().GetComponent<Dungeon1Scene>();
    }

    void DyingCheck()
    {
        if (_stat.Hp == 0)
        {
            State = Define.State.Die;
            StartCoroutine(DropCoin());
            //dropTest();
            Destroy(gameObject, 3f);
        }
        else
            return;
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
               // State = Define.State.Attack;
            }
            else
            {
                nma.SetDestination(_player.transform.position);
                State = Define.State.Moving;
            }
        }
    }


    void PlayerHit()
    {
        if(_player!=null)
        {
            PlayerStat playerStat = _player.GetComponent<PlayerStat>(); // 플레이어 스탯 가져옴
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
