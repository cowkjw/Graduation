using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossAIController : MonoBehaviour
{

    public Projector projector;

    public Define.State State = Define.State.Idle;
    float jumpAttackTimer = 30f;
    public ParticleSystem jumpAttackEffect;
    public Define.EnemyType EnemyType;
    bool isJumping;
    bool attacking;
    bool dying;

    float jumpAttackCooldown = 30f;
    float speed = 2.5f;
    float jumpProgress = 0f;
    bool isAnimationPlayed;
    Vector3 initialPosition;
    Vector3 positionToJump;
    Transform target;
    NavMeshAgent nma;
    BehaviorTree behaviorTree;
    Stat _stat;
    Animator animator;
    AudioSource audioSource;
    [SerializeField]
    AudioClip jumpAttackSoundEffect;

    private void Awake()
    {
        Init();
    }
    void Start()
    {
        target = Managers.Game.GetPlayer().transform;
        behaviorTree = new BehaviorTree();

        // 일반 공격 노드
        Sequence attackSequence = new Sequence();
        attackSequence.AddChild(new ConditionNode(() => !isJumping));
        attackSequence.AddChild(new ConditionNode(() => (transform.position - target.position).sqrMagnitude <= nma.stoppingDistance * nma.stoppingDistance));
        attackSequence.AddChild(new ActionNode(FeetAttack));
         
        //점프 공격 관련 노드
        Sequence jumpAttackSequence = new Sequence();

        //점프 공격 Selector 노드 
        Selector jumpAttackSelector = new Selector();
        jumpAttackSequence.AddChild(jumpAttackSelector);

        // 점프 공격 중 확인 노드
        Sequence continueJumpAttackSequence = new Sequence();
        jumpAttackSelector.AddChild(continueJumpAttackSequence);
        continueJumpAttackSequence.AddChild(new ConditionNode(() => isJumping));
        continueJumpAttackSequence.AddChild(new ActionNode(JumpAttacking)); // 점프 공격 실행

        // 점프 공격 시작 체크 노드
        Sequence startJumpAttackSequence = new Sequence();
        jumpAttackSelector.AddChild(startJumpAttackSequence);
        startJumpAttackSequence.AddChild(new ConditionNode(() => jumpAttackTimer >= jumpAttackCooldown));
        startJumpAttackSequence.AddChild(new ConditionNode(() => ((float)_stat.Hp / _stat.MaxHp <= 0.6f)));
        startJumpAttackSequence.AddChild(new ActionNode(StartJumpAttack));
        startJumpAttackSequence.AddChild(new ActionNode(ResetJumpAttackTimer));

        // 점프공격 시간 초 증가 노드
        Sequence IncrementJumpAttackSequence = new Sequence();
        IncrementJumpAttackSequence.AddChild(new ActionNode(IncrementJumpAttackTimer)); // 쿨타임 증가

        // 이동 노드
        Sequence moveSequence = new Sequence();
        moveSequence.AddChild(new ConditionNode(() => (!attacking && !isJumping)));
        moveSequence.AddChild(new ConditionNode(() => (transform.position - target.position).sqrMagnitude > 16f));
        moveSequence.AddChild(new ActionNode(MovingToPlayer));
         
        // 죽었는지 판단하는 노드
        Sequence dyingSequence = new Sequence();
        dyingSequence.AddChild(new ConditionNode(()=> (float)_stat.Hp / _stat.MaxHp <= 0f));
        dyingSequence.AddChild(new ActionNode(Dying));

        // 점프 공격, 이동, 일반 공격 노드 선택 노드
        Selector rootSelector = new Selector();
        rootSelector.AddChild(dyingSequence);
        rootSelector.AddChild(jumpAttackSequence);
        rootSelector.AddChild(moveSequence);
        rootSelector.AddChild(attackSequence);

        // 루트 관련 노드
        Sequence rootSequence = new Sequence();
        rootSequence.AddChild(new ConditionNode(() => !dying));
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

    void Dying()
    {
        State = Define.State.Die;
        animator.Play("Die");
        dying = true;
    }

    void Init()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = jumpAttackSoundEffect;
        animator = GetComponent<Animator>();
        nma = GetComponent<NavMeshAgent>();
        nma.speed = speed;
        EnemyType = Define.EnemyType.Boss;
        _stat = GetComponent<Stat>();
        _stat.SetStat();
        attacking = false;
        isJumping = false;
        dying = false;
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
        if (attacking)
        {
            animator.Play("JumpAttack");
            attacking = false;
        }
        isJumping = true;
        jumpProgress = 0f;
        isAnimationPlayed = false;
        initialPosition = transform.position;
        positionToJump = target.position;
        projector.enabled = true;
        projector.transform.position = target.position+Vector3.up*3f;
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
        projector.orthographicSize = jumpProgress * 10f;
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

        if(jumpProgress>=0.4f)
        {
            audioSource.Play();
            return;
        }
    }

    void IncrementJumpAttackTimer()
    {
        if((int)jumpAttackTimer<30)
        {
            jumpAttackTimer += Time.deltaTime;
        }
    }

    void ResetJumpAttackTimer()
    {
        jumpAttackTimer = 0f;
    }

    void OnPlayerHit()
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
    void OnEndAttack() // 공격 애니메이션 종료
    {
        attacking = false;
    }
    void OnEndJumpAttack()
    {
        jumpAttackEffect.Play();
        isJumping = false;
        projector.enabled = false;
    }
}
