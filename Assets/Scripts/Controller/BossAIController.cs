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

        // �Ϲ� ���� ���
        Sequence attackSequence = new Sequence();
        attackSequence.AddChild(new ConditionNode(() => !isJumping));
        attackSequence.AddChild(new ConditionNode(() => (transform.position - target.position).sqrMagnitude <= nma.stoppingDistance * nma.stoppingDistance));
        attackSequence.AddChild(new ActionNode(FeetAttack));
         
        //���� ���� ���� ���
        Sequence jumpAttackSequence = new Sequence();

        //���� ���� Selector ��� 
        Selector jumpAttackSelector = new Selector();
        jumpAttackSequence.AddChild(jumpAttackSelector);

        // ���� ���� �� Ȯ�� ���
        Sequence continueJumpAttackSequence = new Sequence();
        jumpAttackSelector.AddChild(continueJumpAttackSequence);
        continueJumpAttackSequence.AddChild(new ConditionNode(() => isJumping));
        continueJumpAttackSequence.AddChild(new ActionNode(JumpAttacking)); // ���� ���� ����

        // ���� ���� ���� üũ ���
        Sequence startJumpAttackSequence = new Sequence();
        jumpAttackSelector.AddChild(startJumpAttackSequence);
        startJumpAttackSequence.AddChild(new ConditionNode(() => jumpAttackTimer >= jumpAttackCooldown));
        startJumpAttackSequence.AddChild(new ConditionNode(() => ((float)_stat.Hp / _stat.MaxHp <= 0.6f)));
        startJumpAttackSequence.AddChild(new ActionNode(StartJumpAttack));
        startJumpAttackSequence.AddChild(new ActionNode(ResetJumpAttackTimer));

        // �������� �ð� �� ���� ���
        Sequence IncrementJumpAttackSequence = new Sequence();
        IncrementJumpAttackSequence.AddChild(new ActionNode(IncrementJumpAttackTimer)); // ��Ÿ�� ����

        // �̵� ���
        Sequence moveSequence = new Sequence();
        moveSequence.AddChild(new ConditionNode(() => (!attacking && !isJumping)));
        moveSequence.AddChild(new ConditionNode(() => (transform.position - target.position).sqrMagnitude > 16f));
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
        if (!attacking) // �������� �ƴҶ��� ȸ��
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        }

        AnimatorStateInfo currentAnimationState = animator.GetCurrentAnimatorStateInfo(0);
        if (currentAnimationState.normalizedTime > 1.0f && !attacking && currentAnimationState.IsName("Walk")) // �������� �ƴϸ�
        {
            animator.Play("FootAttack");
            attacking = true; // ���������� ǥ��
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
        jumpProgress += Time.deltaTime / jumpDuration;
        projector.orthographicSize = jumpProgress * 10f;
        // �ִϸ��̼��� ���� ���۵��� �ʾҰ� ���� ��ư�� ���� �� �ð��� �ִϸ��̼� ���� �ð� ���� ������, ���� ���� �ִϸ��̼��� ����մϴ�.
        if (jumpProgress < animationStartTime / jumpDuration && !isAnimationPlayed)
        {
            animator.Play("JumpAttack");
            isAnimationPlayed = true;
        }

        // �÷��̾ ���� ���� ���� �ܰ迡 �ִ� ���, ��ġ�� ������Ʈ�մϴ�.
        if (jumpProgress < fallStartTime)
        {
            // ���� ���� �ܰ��� ���� ��Ȳ�� ����մϴ�.
            float verticalJumpProgress = jumpProgress / fallStartTime;

            // ���� ������ ���� �÷��̾� ��ġ �������� ����մϴ�.
            float verticalJumpOffset = Mathf.Sin(verticalJumpProgress * Mathf.PI) * verticalJumpHeight;

            // ���� ������ ���� ���� �������� ����Ͽ� �÷��̾� ��ġ�� �����մϴ�.
            transform.position = Vector3.Lerp(initialPosition, positionToJump, verticalJumpProgress) + Vector3.up * verticalJumpOffset;
        }
        // �÷��̾ �߶��ϱ� �����ߴٸ�, ��ġ�� ������Ʈ�մϴ�.
        else
        {
            // �߶� �ܰ��� ���� ��Ȳ�� ����մϴ�.
            float fallingProgress = (jumpProgress - fallStartTime) / (1 - fallStartTime);
            // �߶� �ܰ�� ���� �÷��̾� ��ġ �������� ����մϴ�.
            float fallHeightOffset = Mathf.Sin(fallingProgress * Mathf.PI) * verticalJumpHeight;

            // ���� ������ �߶� �������� ����Ͽ� �÷��̾� ��ġ�� �����մϴ�.
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
            PlayerStat playerStat = target.GetComponent<PlayerStat>(); // �÷��̾� ���� ������
            playerStat.Attacked(_stat, target.gameObject); // ������ ������ �Ѱ���
        }
    }
    void OnEndAttack() // ���� �ִϸ��̼� ����
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
