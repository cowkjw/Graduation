using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    int _mask = 1 << 6 | 1 << 8 | 1 << 9; // 6 Ground 8 Enemy 9 Dungeon1
    Vector3 _destPos;

    GameObject _enemyTarget;

    PlayerStat _stat;

    Define.State _state = Define.State.Idle;

    Dungeon1Scene _scene;

    bool _stopAttack = false;


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
                    anim.SetBool("Attacking", false); // 나머지에는 false처리 
                    anim.CrossFade("Idle", 0.1f);
                    break;
                case Define.State.Moving:
                    anim.SetBool("Attacking", false);
                    anim.CrossFade("Run", 0.005f);
                    break;
                case Define.State.Attack:
                    //  anim.CrossFade("Attack", 0.0005f);
                    //  anim.CrossFade("Slash", 0.0005f);
                    if (!anim.GetBool("Attacking")) // 만약 false라면
                    {
                        anim.SetBool("Attacking", true);
                        ComboAttackAnim(anim);

                    }
                    break;
                case Define.State.Die:
                    anim.SetBool("Attacking", false);
                    anim.CrossFade("Die", 0.1f);
                    break;

            }

        }
    }

    void Init()
    {
        _scene = FindObjectOfType<BaseScene>().GetComponent<Dungeon1Scene>();
        _stat = gameObject.GetComponent<PlayerStat>();
        Managers.Input.MouseAction -= MouseEvent;
        Managers.Input.MouseAction += MouseEvent;
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
                Dying();
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
                        _stopAttack = true;
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

            if (disEnemy <= 0.8f) // 일정거리에 들어왔는지 판단
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
                    _stopAttack = false;

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
                _stopAttack = true;
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
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, LayerMask.GetMask("Wall")))
            {
                if (Input.GetMouseButton(0) == false)
                    State = Define.State.Idle;
                return;
            }

            float moveDist = Mathf.Clamp(5 * Time.deltaTime, 0, dir.magnitude);
            transform.position += dir.normalized * moveDist;// 이동
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 25 * Time.deltaTime);
        }

    }

    void Dying()
    {
        if (_stat.Hp == 0)
        {
            Destroy(gameObject);
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9) // 던전1 
        {
            SceneManager.LoadScene(1);
        }
        else if (other.gameObject.layer == 10)
        {
            SceneManager.LoadScene(0);
        }
    }


    void HitEvent() // 애니메이션 event
    {
        if (_enemyTarget != null)
        {
            Debug.Log("타격");
            if (!_stopAttack) // 애니메이션에 event로 넣었기 때문에 stopAttack을 다시 확인해서 데미지가 들어가지 않게 함
            {
                Stat enemyStat = _enemyTarget.GetComponent<Stat>();
                enemyStat.Attacked(_stat);

                _scene.SetStat(enemyStat); // hp bar ratio를 위한 stat전달
                _scene.GetHpBar().gameObject.SetActive(true); // UI 활성화
            }
        }

        if (_stopAttack)
        {
            _scene.GetHpBar().gameObject.SetActive(false);
            State = Define.State.Idle;
        }
        else
        {
            State = Define.State.Attack;
        }

    }

    void ComboAttackAnim(Animator anim) // 콤보 애니메이션 함수
    {

        if (anim.GetBool("Attacking"))
        {
            anim.Play("Slash1");
        }
    }

}
