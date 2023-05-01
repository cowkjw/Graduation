using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossAIController : MonoBehaviour
{
    public float jumpHeight = 50f;
    public float jumpTime = 10f;
    public float jumpSpeed = 2f;
    public GameObject areaPrefab;
    public ParticleSystem jumpAttackEffect;

    public bool isJumping;
    public bool attacking;
    Vector3 initialPosition;
    Vector3 positionToJump;
    float speed = 2.5f;
    public Define.State State = Define.State.Idle;
    public Define.EnemyType EnemyType;

    float attack_run_ratio = 0;
    public float jumpAttackTimer = 0f;
    float jumpAttackCooldown = 15f;
    Transform target;
    NavMeshAgent nma;
    BehaviorTree behaviorTree;
    Stat _stat;
    Animator animator;



    public float jumpProgress = 0f;
    bool isAnimationPlayed;
    private void Awake()
    {
        Init();
    }
    void Start()
    {
        attack_run_ratio = 0;
        target = Managers.Game.GetPlayer().transform;
        behaviorTree = new BehaviorTree();

        Sequence attackSequence = new Sequence();
        attackSequence.AddChild(new ConditionNode(() => !isJumping));
        attackSequence.AddChild(new ConditionNode(() => (transform.position - target.position).sqrMagnitude <= nma.stoppingDistance * nma.stoppingDistance));
        attackSequence.AddChild(new ActionNode(FeetAttack));

        Sequence jumpAttackSequence = new Sequence();

        Selector jumpAttackSelector = new Selector();
        jumpAttackSequence.AddChild(jumpAttackSelector);

        Sequence continueJumpAttackSequence = new Sequence();
        jumpAttackSelector.AddChild(continueJumpAttackSequence);
        continueJumpAttackSequence.AddChild(new ConditionNode(() => isJumping));
        continueJumpAttackSequence.AddChild(new ActionNode(JumpAttacking)); // 점프 공격 실행

        Sequence startJumpAttackSequence = new Sequence();
        jumpAttackSelector.AddChild(startJumpAttackSequence);
        startJumpAttackSequence.AddChild(new ConditionNode(() => jumpAttackTimer >= jumpAttackCooldown));
        startJumpAttackSequence.AddChild(new ConditionNode(() => ((float)_stat.Hp / _stat.MaxHp <= 1f)));
        startJumpAttackSequence.AddChild(new ActionNode(StartJumpAttack));
        startJumpAttackSequence.AddChild(new ActionNode(ResetJumpAttackTimer));

        Sequence IncrementJumpAttackSequence = new Sequence();
        IncrementJumpAttackSequence.AddChild(new ActionNode(IncrementJumpAttackTimer)); // 쿨타임 증가

        Sequence moveSequence = new Sequence();
        moveSequence.AddChild(new ConditionNode(() => (!attacking && !isJumping)));
        moveSequence.AddChild(new ConditionNode(() => (transform.position - target.position).sqrMagnitude > 16f));
        moveSequence.AddChild(new ActionNode(MovingToPlayer));


        Selector rootSelector = new Selector();
        rootSelector.AddChild(jumpAttackSequence);
        rootSelector.AddChild(moveSequence);
        rootSelector.AddChild(attackSequence);

        Sequence rootSequence = new Sequence();
        rootSequence.AddChild(IncrementJumpAttackSequence);
        rootSequence.AddChild(rootSelector);
        behaviorTree.SetRootNode(rootSequence);

    }

    void Update()
    {
        behaviorTree.Update();
        if (jumpAttackEffect != null && jumpAttackEffect.isPlaying)
        {
            jumpAttackEffect.Stop();
        }
    }

    void Init()
    {
        animator = GetComponent<Animator>();
        nma = GetComponent<NavMeshAgent>();
        nma.speed = speed;
        EnemyType = Define.EnemyType.Boss;
        _stat = GetComponent<Stat>();
        _stat.SetStat();
        attacking = false;
        isJumping = false;
        isAnimationPlayed = false;
    }

    void FeetAttack()
    {
        if (target == null)
            return;

        Vector3 dir = target.transform.position - transform.position;
        if (!attacking) // 공격중이 아닐때만 회전
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        }

        AnimatorStateInfo currentAnimationState = animator.GetCurrentAnimatorStateInfo(0);
        if (currentAnimationState.normalizedTime > 1.0f && !attacking && currentAnimationState.IsName("Walk")) // 공격중이 아니면
        { 
            animator.Play("FootAttack");
            attacking = true; // 공격중으로 표시
        }

    }

    void MovingToPlayer()
    {
        if (target == null)
            return;
        attacking = false;

        Vector3 dir = target.transform.position - transform.position;
        nma.SetDestination(target.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        animator.Play("Walk");
    }
    void StartJumpAttack()
    {
        if(attacking)
        {
            animator.Play("JumpAttack");
            attacking = false;
        }
        isJumping = true;
        areaPrefab.transform.position = target.position;
        areaPrefab.SetActive(true);
        jumpProgress = 0f;
        isAnimationPlayed = false;
        initialPosition = transform.position;
        positionToJump = target.position;
    }

    void JumpAttacking()
    {
        // 점프 애니메이션의 총 시간.
        float jumpDuration = 5f;

        // 점프 버튼을 누른 후 점프 애니메이션을 시작하는 데 걸리는 시간.
        float animationStartTime = 1.5f;

        // 점프의 최대 높이.
        float verticalJumpHeight = 40f;

        // 애니메이션의 수직 점프 단계의 지속 시간.
        float verticalJumpDuration = jumpDuration * 0.5f;

        // 최대 높이에 도달한 후 플레이어가 추락하기 시작하는 시간.
        float fallStartTime = (jumpDuration - verticalJumpDuration) / jumpDuration;

        // 플레이어 위치 보간에 사용되는 점프 애니메이션의 진행 상황.
        jumpProgress += Time.deltaTime / jumpDuration;

        // 애니메이션이 아직 시작되지 않았고 점프 버튼이 눌린 후 시간이 애니메이션 시작 시간 내에 있으면, 점프 어택 애니메이션을 재생합니다.
        if (jumpProgress < animationStartTime / jumpDuration && !isAnimationPlayed)
        {
            animator.Play("JumpAttack");
            isAnimationPlayed = true;
        }

        // 플레이어가 아직 수직 점프 단계에 있는 경우, 위치를 업데이트합니다.
        if (jumpProgress < fallStartTime)
        {
            // 수직 점프 단계의 진행 상황을 계산합니다.
            float verticalJumpProgress = jumpProgress / fallStartTime;

            // 수직 점프로 인한 플레이어 위치 오프셋을 계산합니다.
            float verticalJumpOffset = Mathf.Sin(verticalJumpProgress * Mathf.PI) * verticalJumpHeight;

            // 선형 보간과 수직 점프 오프셋을 사용하여 플레이어 위치를 설정합니다.
            transform.position = Vector3.Lerp(initialPosition, positionToJump, verticalJumpProgress) + Vector3.up * verticalJumpOffset;
        }
        // 플레이어가 추락하기 시작했다면, 위치를 업데이트합니다.
        else
        {
            // 추락 단계의 진행 상황을 계산합니다.
            float fallingProgress = (jumpProgress - fallStartTime) / (1 - fallStartTime);
            // 추락 단계로 인한 플레이어 위치 오프셋을 계산합니다.
            float fallHeightOffset = Mathf.Sin(fallingProgress * Mathf.PI) * verticalJumpHeight;

            // 선형 보간과 추락 오프셋을 사용하여 플레이어 위치를 설정합니다.
            transform.position = Vector3.Lerp(positionToJump, initialPosition, fallingProgress) + Vector3.up * fallHeightOffset;
        }
    }

    void IncrementJumpAttackTimer()
    {
        jumpAttackTimer += Time.deltaTime;
    }

    void ResetJumpAttackTimer()
    {
        jumpAttackTimer = 0f;
    }

    void PlayerHit()
    {
        if (target != null)
        {
            if ((transform.position - target.position).sqrMagnitude > nma.stoppingDistance * nma.stoppingDistance)
            {
                return;
            }
            PlayerStat playerStat = target.GetComponent<PlayerStat>(); // 플레이어 스탯 가져옴
            playerStat.Attacked(_stat, target.gameObject); // 몬스터의 스탯을 넘겨줌
        }
    }
    void EndAttack() // 공격 애니메이션 종료
    {
        attacking = false;
    }
    void EndJumpAttack()
    {
        jumpAttackEffect.Play();
        areaPrefab.SetActive(false);
        isJumping = false;
    }
}
