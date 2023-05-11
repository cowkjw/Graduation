using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossAIController : MonoBehaviour
{

    public Projector Projector;
    public ParticleSystem JumpAttackEffect;
    public Define.EnemyType EnemyType;
    public Define.State State = Define.State.Idle;

    bool _isJumping;
    bool _attacking;
    bool _dying;
    bool _isAnimationPlayed;
    float _jumpAttackTimer = 30f;
    float _jumpAttackCooldown = 30f;
    float _speed = 2.5f;
    float _jumpProgress = 0f;
    Vector3 _initialPosition;
    Vector3 _positionToJump;
    Transform _target;
    NavMeshAgent _nma;
    BehaviorTree _behaviorTree;
    Stat _stat;
    Animator _animator;
    AudioSource _audioSource;
    [SerializeField]
    AudioClip _jumpAttackSoundEffect;

    private void Awake()
    {
        Init();
    }
    void Start()
    {
        _target = Managers.Game.GetPlayer().transform;
        _behaviorTree = new BehaviorTree();

        // 일반 공격 노드
        Sequence attackSequence = new Sequence();
        attackSequence.AddChild(new ConditionNode(() => !_isJumping));
        attackSequence.AddChild(new ConditionNode(() => (transform.position - _target.position).sqrMagnitude <= _nma.stoppingDistance * _nma.stoppingDistance));
        attackSequence.AddChild(new ActionNode(FeetAttack));
         
        //점프 공격 관련 노드
        Sequence jumpAttackSequence = new Sequence();

        //점프 공격 Selector 노드 
        Selector jumpAttackSelector = new Selector();
        jumpAttackSequence.AddChild(jumpAttackSelector);

        // 점프 공격 중 확인 노드
        Sequence continueJumpAttackSequence = new Sequence();
        jumpAttackSelector.AddChild(continueJumpAttackSequence);
        continueJumpAttackSequence.AddChild(new ConditionNode(() => _isJumping));
        continueJumpAttackSequence.AddChild(new ActionNode(JumpAttacking)); // 점프 공격 실행

        // 점프 공격 시작 체크 노드
        Sequence startJumpAttackSequence = new Sequence();
        jumpAttackSelector.AddChild(startJumpAttackSequence);
        startJumpAttackSequence.AddChild(new ConditionNode(() => _jumpAttackTimer >= _jumpAttackCooldown));
        startJumpAttackSequence.AddChild(new ConditionNode(() => ((float)_stat.Hp / _stat.MaxHp <= 0.6f)));
        startJumpAttackSequence.AddChild(new ActionNode(StartJumpAttack));
        startJumpAttackSequence.AddChild(new ActionNode(ResetJumpAttackTimer));

        // 점프공격 시간 초 증가 노드
        Sequence IncrementJumpAttackSequence = new Sequence();
        IncrementJumpAttackSequence.AddChild(new ActionNode(IncrementJumpAttackTimer)); // 쿨타임 증가

        // 이동 노드
        Sequence moveSequence = new Sequence();
        moveSequence.AddChild(new ConditionNode(() => (!_attacking && !_isJumping)));
        moveSequence.AddChild(new ConditionNode(() => (transform.position - _target.position).sqrMagnitude > 16f));
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
        rootSequence.AddChild(new ConditionNode(() => !_dying));
        rootSequence.AddChild(IncrementJumpAttackSequence);
        rootSequence.AddChild(rootSelector);
        _behaviorTree.SetRootNode(rootSequence);

    }

    void Update()
    {
        _behaviorTree.Update();
        if (JumpAttackEffect != null && JumpAttackEffect.isPlaying)
        {
            JumpAttackEffect.Stop();
        }
    }

    void Dying()
    {
        State = Define.State.Die;
        _animator.Play("Die");
        _dying = true;
    }

    void Init()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _jumpAttackSoundEffect;
        _animator = GetComponent<Animator>();
        _nma = GetComponent<NavMeshAgent>();
        _nma.speed = _speed;
        EnemyType = Define.EnemyType.Boss;
        _stat = GetComponent<Stat>();
        _stat.SetStat();
        _attacking = false;
        _isJumping = false;
        _dying = false;
        _isAnimationPlayed = false;
    }

    void FeetAttack()
    {
        if (_target == null)
            return;

        Vector3 dir = _target.transform.position - transform.position;
        if (!_attacking) // 공격중이 아닐때만 회전
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        }

        AnimatorStateInfo currentAnimationState = _animator.GetCurrentAnimatorStateInfo(0);
        if (currentAnimationState.normalizedTime > 1.0f && !_attacking && currentAnimationState.IsName("Walk")) // 공격중이 아니면
        {
            _animator.Play("FootAttack");
            _attacking = true; // 공격중으로 표시
        }

    }

    void MovingToPlayer()
    {
        if (_target == null)
            return;
        _attacking = false;

        Vector3 dir = _target.transform.position - transform.position;
        _nma.SetDestination(_target.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        _animator.Play("Walk");
    }
    void StartJumpAttack()
    {
        if (_attacking)
        {
            _animator.Play("JumpAttack");
            _attacking = false;
        }
        _isJumping = true;
        _jumpProgress = 0f;
        _isAnimationPlayed = false;
        _initialPosition = transform.position;
        _positionToJump = _target.position;
        Projector.enabled = true;
        Projector.transform.position = _target.position+Vector3.up*3f;
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
        _jumpProgress += Time.deltaTime / jumpDuration;
        Projector.orthographicSize = _jumpProgress * 10f;
        // 애니메이션이 아직 시작되지 않았고 점프 버튼이 눌린 후 시간이 애니메이션 시작 시간 내에 있으면, 점프 어택 애니메이션을 재생합니다.
        if (_jumpProgress < animationStartTime / jumpDuration && !_isAnimationPlayed)
        {
            _animator.Play("JumpAttack");
            _isAnimationPlayed = true;
        }

        // 플레이어가 아직 수직 점프 단계에 있는 경우, 위치를 업데이트합니다.
        if (_jumpProgress < fallStartTime)
        {
            // 수직 점프 단계의 진행 상황을 계산합니다.
            float verticalJumpProgress = _jumpProgress / fallStartTime;

            // 수직 점프로 인한 플레이어 위치 오프셋을 계산합니다.
            float verticalJumpOffset = Mathf.Sin(verticalJumpProgress * Mathf.PI) * verticalJumpHeight;

            // 선형 보간과 수직 점프 오프셋을 사용하여 플레이어 위치를 설정합니다.
            transform.position = Vector3.Lerp(_initialPosition, _positionToJump, verticalJumpProgress) + Vector3.up * verticalJumpOffset;
        }
        // 플레이어가 추락하기 시작했다면, 위치를 업데이트합니다.
        else
        {
            // 추락 단계의 진행 상황을 계산합니다.
            float fallingProgress = (_jumpProgress - fallStartTime) / (1 - fallStartTime);
            // 추락 단계로 인한 플레이어 위치 오프셋을 계산합니다.
            float fallHeightOffset = Mathf.Sin(fallingProgress * Mathf.PI) * verticalJumpHeight;

            // 선형 보간과 추락 오프셋을 사용하여 플레이어 위치를 설정합니다.
            transform.position = Vector3.Lerp(_positionToJump, _initialPosition, fallingProgress) + Vector3.up * fallHeightOffset;
        }

        if(_jumpProgress >= 0.4f)
        {
            _audioSource.Play();
            return;
        }
    }

    void IncrementJumpAttackTimer()
    {
        if((int)_jumpAttackTimer < 30)
        {
            _jumpAttackTimer += Time.deltaTime;
        }
    }

    void ResetJumpAttackTimer()
    {
        _jumpAttackTimer = 0f;
    }

    void OnPlayerHit()
    {
        if (_target != null)
        {
            if ((transform.position - _target.position).sqrMagnitude > _nma.stoppingDistance * _nma.stoppingDistance)
            {
                return;
            }
            PlayerStat playerStat = _target.GetComponent<PlayerStat>(); // 플레이어 스탯 가져옴
            playerStat.Attacked(_stat, _target.gameObject); // 몬스터의 스탯을 넘겨줌
        }
    }
    void OnEndAttack() // 공격 애니메이션 종료
    {
        _attacking = false;
    }
    void OnEndJumpAttack()
    {
        JumpAttackEffect.Play();
        Projector.enabled = false;
        _isJumping = false;
    }
}
