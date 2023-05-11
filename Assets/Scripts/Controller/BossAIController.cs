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

        // �Ϲ� ���� ���
        Sequence attackSequence = new Sequence();
        attackSequence.AddChild(new ConditionNode(() => !_isJumping));
        attackSequence.AddChild(new ConditionNode(() => (transform.position - _target.position).sqrMagnitude <= _nma.stoppingDistance * _nma.stoppingDistance));
        attackSequence.AddChild(new ActionNode(FeetAttack));
         
        //���� ���� ���� ���
        Sequence jumpAttackSequence = new Sequence();

        //���� ���� Selector ��� 
        Selector jumpAttackSelector = new Selector();
        jumpAttackSequence.AddChild(jumpAttackSelector);

        // ���� ���� �� Ȯ�� ���
        Sequence continueJumpAttackSequence = new Sequence();
        jumpAttackSelector.AddChild(continueJumpAttackSequence);
        continueJumpAttackSequence.AddChild(new ConditionNode(() => _isJumping));
        continueJumpAttackSequence.AddChild(new ActionNode(JumpAttacking)); // ���� ���� ����

        // ���� ���� ���� üũ ���
        Sequence startJumpAttackSequence = new Sequence();
        jumpAttackSelector.AddChild(startJumpAttackSequence);
        startJumpAttackSequence.AddChild(new ConditionNode(() => _jumpAttackTimer >= _jumpAttackCooldown));
        startJumpAttackSequence.AddChild(new ConditionNode(() => ((float)_stat.Hp / _stat.MaxHp <= 0.6f)));
        startJumpAttackSequence.AddChild(new ActionNode(StartJumpAttack));
        startJumpAttackSequence.AddChild(new ActionNode(ResetJumpAttackTimer));

        // �������� �ð� �� ���� ���
        Sequence IncrementJumpAttackSequence = new Sequence();
        IncrementJumpAttackSequence.AddChild(new ActionNode(IncrementJumpAttackTimer)); // ��Ÿ�� ����

        // �̵� ���
        Sequence moveSequence = new Sequence();
        moveSequence.AddChild(new ConditionNode(() => (!_attacking && !_isJumping)));
        moveSequence.AddChild(new ConditionNode(() => (transform.position - _target.position).sqrMagnitude > 16f));
        moveSequence.AddChild(new ActionNode(MovingToPlayer));
         
        // �׾����� �Ǵ��ϴ� ���
        Sequence dyingSequence = new Sequence();
        dyingSequence.AddChild(new ConditionNode(()=> (float)_stat.Hp / _stat.MaxHp <= 0f));
        dyingSequence.AddChild(new ActionNode(Dying));

        // ���� ����, �̵�, �Ϲ� ���� ��� ���� ���
        Selector rootSelector = new Selector();
        rootSelector.AddChild(dyingSequence);
        rootSelector.AddChild(jumpAttackSequence);
        rootSelector.AddChild(moveSequence);
        rootSelector.AddChild(attackSequence);

        // ��Ʈ ���� ���
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
        if (!_attacking) // �������� �ƴҶ��� ȸ��
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        }

        AnimatorStateInfo currentAnimationState = _animator.GetCurrentAnimatorStateInfo(0);
        if (currentAnimationState.normalizedTime > 1.0f && !_attacking && currentAnimationState.IsName("Walk")) // �������� �ƴϸ�
        {
            _animator.Play("FootAttack");
            _attacking = true; // ���������� ǥ��
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
        // ���� �ִϸ��̼��� �� �ð�.
        float jumpDuration = 5f;

        // ���� ��ư�� ���� �� ���� �ִϸ��̼��� �����ϴ� �� �ɸ��� �ð�.
        float animationStartTime = 1.5f;

        // ������ �ִ� ����.
        float verticalJumpHeight = 40f;

        // �ִϸ��̼��� ���� ���� �ܰ��� ���� �ð�.
        float verticalJumpDuration = jumpDuration * 0.5f;

        // �ִ� ���̿� ������ �� �÷��̾ �߶��ϱ� �����ϴ� �ð�.
        float fallStartTime = (jumpDuration - verticalJumpDuration) / jumpDuration;

        // �÷��̾� ��ġ ������ ���Ǵ� ���� �ִϸ��̼��� ���� ��Ȳ.
        _jumpProgress += Time.deltaTime / jumpDuration;
        Projector.orthographicSize = _jumpProgress * 10f;
        // �ִϸ��̼��� ���� ���۵��� �ʾҰ� ���� ��ư�� ���� �� �ð��� �ִϸ��̼� ���� �ð� ���� ������, ���� ���� �ִϸ��̼��� ����մϴ�.
        if (_jumpProgress < animationStartTime / jumpDuration && !_isAnimationPlayed)
        {
            _animator.Play("JumpAttack");
            _isAnimationPlayed = true;
        }

        // �÷��̾ ���� ���� ���� �ܰ迡 �ִ� ���, ��ġ�� ������Ʈ�մϴ�.
        if (_jumpProgress < fallStartTime)
        {
            // ���� ���� �ܰ��� ���� ��Ȳ�� ����մϴ�.
            float verticalJumpProgress = _jumpProgress / fallStartTime;

            // ���� ������ ���� �÷��̾� ��ġ �������� ����մϴ�.
            float verticalJumpOffset = Mathf.Sin(verticalJumpProgress * Mathf.PI) * verticalJumpHeight;

            // ���� ������ ���� ���� �������� ����Ͽ� �÷��̾� ��ġ�� �����մϴ�.
            transform.position = Vector3.Lerp(_initialPosition, _positionToJump, verticalJumpProgress) + Vector3.up * verticalJumpOffset;
        }
        // �÷��̾ �߶��ϱ� �����ߴٸ�, ��ġ�� ������Ʈ�մϴ�.
        else
        {
            // �߶� �ܰ��� ���� ��Ȳ�� ����մϴ�.
            float fallingProgress = (_jumpProgress - fallStartTime) / (1 - fallStartTime);
            // �߶� �ܰ�� ���� �÷��̾� ��ġ �������� ����մϴ�.
            float fallHeightOffset = Mathf.Sin(fallingProgress * Mathf.PI) * verticalJumpHeight;

            // ���� ������ �߶� �������� ����Ͽ� �÷��̾� ��ġ�� �����մϴ�.
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
            PlayerStat playerStat = _target.GetComponent<PlayerStat>(); // �÷��̾� ���� ������
            playerStat.Attacked(_stat, _target.gameObject); // ������ ������ �Ѱ���
        }
    }
    void OnEndAttack() // ���� �ִϸ��̼� ����
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
