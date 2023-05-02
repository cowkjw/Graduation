using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : BaseCharacterController//MonoBehaviour
{



    public ParticleSystem swordEffect;

    BaseScene _scene;
    int _mask = 1 << 6 | 1 << 8 | 1 << 9; // 6 Ground 8 Enemy 9 Dungeon1 5 UI
    bool _stopAttack = false;
    const float duration = 1.5f;
    [SerializeField]
    ParticleSystem levelUpParticle;

    public override Define.State State
    {
        get => _state;

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
                case Define.State.CrowdControl:
                    anim.Play("Stuned");
                    break;
            }
        }
    }

    override protected void Init()
    {
        // BaseScene을 찾아온다 (어떤 Scene일지 모르기 때문)
        _scene = FindObjectOfType<BaseScene>();
        _stat = gameObject.GetComponent<PlayerStat>() as PlayerStat;
        Managers.Input.MouseAction -= MouseEvent;
        Managers.Input.MouseAction += MouseEvent;
    }

    protected override void Update()
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
                Attacking();
                break;
            case Define.State.CrowdControl:
                StartCoroutine(Stunned(duration));
                break;
            case Define.State.Idle:
                StopCoroutine(Stunned(duration));
                break;
            case Define.State.Skill:
                EndSkillAnim();
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
                    if (evt == Define.MouseState.ButtonUp)
                        _stopAttack = true;
                }
                break;
        }
    }
   
    override protected void Attacking()
    {
        if (_target == null || !((_target.transform.position - transform.position).sqrMagnitude <= 0.81f)
            || _target.GetComponent<EnemyController>()?.State == Define.State.Die || _target.GetComponent<BossAIController>()?.State == Define.State.Die) // 제곱근 연산 줄임 타겟이 죽은 상태라면 return
        {
            return;
        }
        Quaternion lookAtTarget;
        if (_target.gameObject.name == "Boss")
        {
            lookAtTarget = Quaternion.LookRotation((_target.transform.parent.position - transform.position).normalized);
        }
        else
        {
            lookAtTarget = Quaternion.LookRotation((_target.transform.position - transform.position).normalized);
        }
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
        if (State == Define.State.CrowdControl) // 군중 제어 상태이면 리턴
            return;
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
    
    IEnumerator Stunned(float duration)
    {
        yield return new WaitForSeconds(duration);
        State = Define.State.Idle;
    }

    protected override void Dying()
    {
        if (_stat.Hp == 0)
        {
            Destroy(gameObject);
        }
    }

    void OnHitEvent()
    {

        if (_target == null) return;
        DungeonScene dungeonScene = null; // 던전 Scene을 생성
        if(_scene is DungeonScene)
        {
            dungeonScene = _scene.GetComponent<DungeonScene>(); // 만약 해당 씬이 기본 던전 맵이면
        }
        if (!_stopAttack)
        {
            Stat enemyStat = _target.GetComponentInParent<Stat>()??_target.GetComponentInParent<Stat>();// 해당 오브젝트에 없으면 부모의 오브젝트에서 찾기
            enemyStat.Attacked(_stat, _target);
            if(dungeonScene == null)
                return;

            dungeonScene.ObjStat = enemyStat;
            dungeonScene.ObjName = enemyStat.name;
            dungeonScene.ObjNameText.gameObject.SetActive(true);
            dungeonScene.HpBar.gameObject.SetActive(true);
            
            State = Define.State.Attack;
        }
        else//(_stopAttack)
        { 
            if (dungeonScene == null)
                return;
            dungeonScene.HpBar.gameObject.SetActive(false);
            dungeonScene.ObjNameText.gameObject.SetActive(false);
        //    swordEffect.Stop();
            State = Define.State.Idle;
        }
    }


    void OnAttackEffect()
    {
        //swordEffect.Play();
    }

    public void LevelUpEffect()
    {
        levelUpParticle.Play(); // 레벨업 파티클 실행
        StartCoroutine(StopLevelUpParticle(1f)); // 1초후에 멈춤
    }

    IEnumerator StopLevelUpParticle(float delay)
    {
        yield return new WaitForSeconds(delay);
        levelUpParticle.Stop();
    }

    void ComboAttackAnim(Animator anim)
    {
        int randomAttack = Random.Range(0, 2) == 0 ? 1 : 3; // 50프로 확률

        if (anim.GetBool("Attacking"))
        {
            anim.Play($"Slash{randomAttack}"); // 랜덤한 순서로 기본 공격 실행
        }
    }

    void OnParticleCollision(GameObject other)
    {
        float pushForce = 8f;

        if (other != null)
        {
            Vector3 pushDirection = transform.position - other.transform.position;
            GetComponent<Rigidbody>().AddForce(pushDirection.normalized * pushForce, ForceMode.Impulse);
            State = Define.State.CrowdControl;
        }
    }


    void EndSkillAnim() // 스킬 애니메이션이 끝났다면
    {
        AnimatorStateInfo currentAnimationState = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
        if(currentAnimationState.normalizedTime>=1.0f)
        {
            State = Define.State.Idle;
        }
    }

}
