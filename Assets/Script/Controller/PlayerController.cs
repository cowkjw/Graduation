using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : BaseCharacterController//MonoBehaviour
{


 
    int _mask = 1 << 6 | 1 << 8 | 1 << 9; // 6 Ground 8 Enemy 9 Dungeon1 5 UI

    Dungeon1Scene _scene;
    public ParticleSystem swordEffect;
    bool _stopAttack = false;


    public override Define.State State
    {
        get { return _state; }

        set
        {
            _state = value;

            Animator anim = GetComponent<Animator>();

            switch (_state)
            {
                case Define.State.Idle:
                    anim.SetBool("Attacking", false);
                    anim.CrossFade("Idle", 0.1f);
                    break;
                case Define.State.Moving:
                    anim.SetBool("Attacking", false);
                    anim.CrossFade("Run", 0.005f);
                    break;
                case Define.State.Attack:
                    if (!anim.GetBool("Attacking"))
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

    override protected void Init()
    {
        _scene = FindObjectOfType<BaseScene>().GetComponent<Dungeon1Scene>();
        _stat = gameObject.GetComponent<PlayerStat>() as PlayerStat;
        Managers.Input.MouseAction -= MouseEvent;
        Managers.Input.MouseAction += MouseEvent;
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
                    if (evt == Define.MouseState.ButtonUp)
                        _stopAttack = true;
                }
                break;
        }
    }

    //override protected void Attacking()
    //{
    //    if (_target)
    //        if (_target != null)
    //        {
    //            float disEnemy = Vector3.Distance(_target.transform.position, transform.position);
    //            Vector3 dirEnemy = (_target.transform.position - transform.position);
    //            Quaternion lookEnemy = Quaternion.LookRotation(dirEnemy);

    //            if (disEnemy <= 0.8f)
    //            {
    //                transform.rotation = Quaternion.Slerp(transform.rotation, lookEnemy, 25 * Time.deltaTime);
    //                State = Define.State.Attack;
    //                return;
    //            }
    //        }
    //}

    override protected void Attacking()
    {

        if (_target == null || !((_target.transform.position - transform.position).sqrMagnitude <= 0.64f)) // 제곱근 연산 줄임
        {
            return;
        }

        Quaternion lookAtTarget = Quaternion.LookRotation((_target.transform.position - transform.position).normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookAtTarget, 25 * Time.deltaTime);

        State = Define.State.Attack;
        //if (_target == null)
        //{
        //    return;
        //}

        //float distanceToTarget = Vector3.Distance(_target.transform.position, transform.position);

        //if (distanceToTarget <= 0.8f)
        //{
        //    Vector3 directionToTarget = (_target.transform.position - transform.position).normalized;
        //    Quaternion lookAtTarget = Quaternion.LookRotation(directionToTarget);
        //    transform.rotation = Quaternion.Slerp(transform.rotation, lookAtTarget, 25 * Time.deltaTime);

        //    State = Define.State.Attack;
        //}
    }

    void EnemyTargetAndState(Define.MouseState evt)
    {

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool raycastHit = Physics.Raycast(ray, out hit, 100f, _mask);

        switch (evt)
        {

            case Define.MouseState.LButtonDown:
                if (raycastHit)
                {
                    _destPos = hit.point;
                    State = Define.State.Moving;
                    _stopAttack = false;

                    if (hit.collider.gameObject.layer == 8)
                    {
                        _target = hit.collider.gameObject;
                    }

                    else
                    {
                        _target = null;
                    }
                }
                break;
            case Define.MouseState.Press:
                {
                    if (_target == null && raycastHit)
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

    //protected override void Moving()
    //{
    //    Vector3 dir = _destPos - transform.position;
    //    dir.y = 0; // 몬스터 위로 이동 x

    //    Attacking();

    //    if (dir.magnitude < 0.1f)
    //    {
    //        State = Define.State.Idle;
    //    }
    //    else
    //    {
    //        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, LayerMask.GetMask("Wall")))
    //        {
    //            if (Input.GetMouseButton(0) == false)
    //                State = Define.State.Idle;
    //            return;
    //        }

    //        // float moveDist = Mathf.Clamp(5 * Time.deltaTime, 0, dir.magnitude);
    //        float moveDist = Mathf.Min(5 * Time.deltaTime, dir.magnitude);
    //        transform.position += dir.normalized * moveDist;
    //        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 25 * Time.deltaTime);
    //    }

    //}

    protected override void Moving()
    {
        Vector3 dir = _destPos - transform.position;
        dir.y = 0;

        Attacking();

        if (dir.magnitude < 0.1f)
        {
            State = Define.State.Idle;
        }
        else
        {
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, LayerMask.GetMask("Wall")))
            {
                if (!Input.GetMouseButton(0))
                {
                    State = Define.State.Idle;
                }

                return;
            }

            float moveDistance = Mathf.Min(5 * Time.deltaTime, dir.magnitude);
            transform.position += dir.normalized * moveDistance;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 25 * Time.deltaTime);
        }
    }

    protected override void Dying()
    {
        if (_stat.Hp == 0)
        {
            Destroy(gameObject);
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9) // 던전 레이어
        {
            SceneManager.LoadScene(1);
        }
        else if (other.gameObject.layer == 10)
        {
            SceneManager.LoadScene(0);
        }
    }


    void HitEvent()
    {

        if (_target == null) return;

        if (!_stopAttack)
        {
            Stat enemyStat = _target.GetComponent<Stat>();
            enemyStat.Attacked(_stat);

            _scene.ObjStat = enemyStat;
            _scene.ObjName = enemyStat.name;
            Debug.Log(enemyStat.name);
            _scene.ObjNameText.gameObject.SetActive(true);
            _scene.HpBar.gameObject.SetActive(true);
            State = Define.State.Attack;
        }
        else//(_stopAttack)
        {
            _scene.HpBar.gameObject.SetActive(false);
            _scene.ObjNameText.gameObject.SetActive(false);
            swordEffect.Stop();
            State = Define.State.Idle;
        }
    }


    void AttackEffect()
    {
        swordEffect.Play();

    }

    void ComboAttackAnim(Animator anim)
    {

        if (anim.GetBool("Attacking"))
        {
            anim.Play("Slash1");
        }
    }

}
