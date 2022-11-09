using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    int _mask = 1 << 6 | 1 << 8 | 1 << 9; // 6 Ground 8 Enemy
    Vector3 _destPos;

    GameObject _enemyTarget;

    Define.State _state = Define.State.Idle;

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
                    anim.CrossFade("Run", 0.005f);
                    break;
                case Define.State.Attack:
                    anim.CrossFade("Attack", 0.005f);
                    break;
                case Define.State.Die:
                    anim.CrossFade("Die", 0.1f);
                    break;

            }

        }
    }


    void Start()
    { 
        Managers.Input.MouseAction -= MouseEvent;
        Managers.Input.MouseAction += MouseEvent;
    }

    void Update()
    {

        switch (State)
        {
            case Define.State.Die:
                //UpdateDie();
                break;
            case Define.State.Moving:
                Moving();
                break;
            case Define.State.Attack:
                AttackEnemy();
                break;
        }
    }


    void MouseEvent(Define.MouseState evt)
    {
        switch (State)
        {
            case Define.State.Idle:
                EnemyTargetAndState(evt);
                break;
            case Define.State.Moving:
                EnemyTargetAndState(evt);
                break;
            case Define.State.Attack:
                {
                    if (evt == Define.MouseState.ButtonUp) // 마우스 떼면 공격 x
                        State = Define.State.Idle;
                }
                break;
        }
    }

    void AttackEnemy()
    {
        if (_enemyTarget != null)
        {
            float disEnemy = Vector3.Distance(_enemyTarget.transform.position, transform.position);
            Vector3 dirEnemy = (_enemyTarget.transform.position - transform.position);
            Quaternion lookEnemy = Quaternion.LookRotation(dirEnemy);

            if (disEnemy <= 1f) // 일정거리에 들어왔는지 판단
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, lookEnemy, 25 * Time.deltaTime);
                State = Define.State.Attack;
                return;
            }


        }
    }

    void EnemyTargetAndState(Define.MouseState evt)
    {

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool raycastHit = Physics.Raycast(ray, out hit, 100f, _mask);


        switch (evt)
        {

            case Define.MouseState.ButtonDown:
                if (raycastHit)
                {
                    _destPos = hit.point;
                    State = Define.State.Moving;

                    if (hit.collider.gameObject.layer == 8)
                    {
                        _enemyTarget = hit.collider.gameObject;

                    }
                    else
                    {
                        _enemyTarget = null;
                    }
                }
                break;
            case Define.MouseState.Press:
                {
                    if (_enemyTarget == null && raycastHit)
                    {
                        _destPos = hit.point;
                    }
                }
                break;
            case Define.MouseState.ButtonUp:
                //State = Define.State.Moving;
                break;

        }

    }

    void Moving()
    {
        Vector3 dir = _destPos - transform.position;
        dir.y = 0; // 몬스터 위로 x

        AttackEnemy();

        if (dir.magnitude < 0.1f)
        {
            State = Define.State.Idle;
        }
        else
        {
            float moveDist = Mathf.Clamp(5 * Time.deltaTime, 0, dir.magnitude);
            transform.position += dir.normalized * moveDist;// 이동
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 25 * Time.deltaTime);
        }

    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9) // 던전1 
        {
            SceneManager.LoadScene(1);
        }
        else if(other.gameObject.layer == 10)
        {
            SceneManager.LoadScene(0);
        }
    }

}
