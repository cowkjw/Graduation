using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : BaseCharacterController//MonoBehaviour
{



    int _mask = 1 << 6 | 1 << 8 | 1 << 9; // 6 Ground 8 Enemy 9 Dungeon1 5 UI

    BaseScene _scene;
    public ParticleSystem swordEffect;
    bool _stopAttack = false;

    [SerializeField]
    ParticleSystem levelUpParticle;


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
        // BaseScene�� ã�ƿ´� (� Scene���� �𸣱� ����)
        _scene = FindObjectOfType<BaseScene>();//.GetComponent<Dungeon1Scene>(); /
        //_scene = FindObjectOfType<BaseScene>().GetComponent<Dungeon1Scene>();
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

    override protected void Attacking()
    {

        if (_target == null || !((_target.transform.position - transform.position).sqrMagnitude <= 0.81f)
            || _target.GetComponent<EnemyController>().State == Define.State.Die) // ������ ���� ���� Ÿ���� ���� ���¶�� return
        {
            return;
        }

        Quaternion lookAtTarget = Quaternion.LookRotation((_target.transform.position - transform.position).normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookAtTarget, 25 * Time.deltaTime);

        State = Define.State.Attack;

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
        if (other.gameObject.layer == 9) // ���� ���̾�
        {
            SceneManager.LoadScene(1);
        }
        else if (other.gameObject.layer == 10)
        {
            SceneManager.LoadScene(0);

        }
        else if(other.gameObject.layer == 13)
        {
            SceneManager.LoadScene(2);
        }
    }



    void HitEvent()
    {

        if (_target == null) return;
        Dungeon1Scene dungeon1Scene = null; // ���� Scene�� ����
        if(_scene is Dungeon1Scene)
        {
            dungeon1Scene = _scene.GetComponent<Dungeon1Scene>(); // ���� �ش� ���� �⺻ ���� ���̸�
        }
        if (!_stopAttack)
        {
            Stat enemyStat = _target.GetComponent<Stat>();
            enemyStat.Attacked(_stat, _target);
            if(dungeon1Scene==null)
                return;

            //_scene.ObjStat = enemyStat;
            //_scene.ObjName = enemyStat.name;
            //Debug.Log(enemyStat.name);
            //_scene.ObjNameText.gameObject.SetActive(true);
            //_scene.HpBar.gameObject.SetActive(true);
            dungeon1Scene.ObjStat = enemyStat;
            dungeon1Scene.ObjName = enemyStat.name;
            Debug.Log(enemyStat.name);
            dungeon1Scene.ObjNameText.gameObject.SetActive(true);
            dungeon1Scene.HpBar.gameObject.SetActive(true);
            
            State = Define.State.Attack;
        }
        else//(_stopAttack)
        {
            //_scene.HpBar.gameObject.SetActive(false);
            //_scene.ObjNameText.gameObject.SetActive(false);   
            dungeon1Scene.HpBar.gameObject.SetActive(false);
            dungeon1Scene.ObjNameText.gameObject.SetActive(false);
            swordEffect.Stop();
            State = Define.State.Idle;
        }
    }


    void AttackEffect()
    {
        swordEffect.Play();

    }

    public void LevelUpEffect()
    {
        levelUpParticle.Play(); // ������ ��ƼŬ ����
        StartCoroutine(StopLevelUpParticle(1f)); // 1���Ŀ� ����
    }

    IEnumerator StopLevelUpParticle(float delay)
    {
        yield return new WaitForSeconds(delay);
        levelUpParticle.Stop();
    }

    void ComboAttackAnim(Animator anim)
    {
        int randomAttack = Random.Range(0, 2) == 0 ? 1 : 3; // 50���� Ȯ��

        if (anim.GetBool("Attacking"))
        {
            Debug.Log($"Slash{randomAttack}");
            anim.Play($"Slash{randomAttack}"); // ������ ������ �⺻ ���� ����
            Debug.Log($"Slash{randomAttack}");

        }
    }



}