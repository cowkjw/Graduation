using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossAIController : MonoBehaviour
{
    //public float jumpHeight = 50f;
    //public float jumpTime = 10f;
    //public float jumpSpeed = 0.5f;
    public GameObject areaPrefab;
    public ParticleSystem jumpAttackEffect;

    //GameObject area;
    //MeshRenderer meshRenderers;
    //Material material;
    //Color matColor;
    //Color matColor1;
    //Color matColor2;

    //Animator animator;
    //private Vector3 initialPosition;
    //private bool isJumping = false;
    //private Vector3 targetPosition;

    //private void OnEnable()
    //{
    //    base.Init();
    //    EnemyType = Define.EnemyType.Boss;
    //    _findRange = 6f;
    //}


    //protected override void Start()
    //{
    //    animator = GetComponent<Animator>();

    //    _target = Managers.Game.GetPlayer();
    //    area = Instantiate(areaPrefab, transform.position, Quaternion.identity);
    //    area.SetActive(false);
    //}

    //protected override void Update()
    //{
    //    if (State != Define.State.Die)
    //        Dying();

    //    switch (State)
    //    {
    //        case Define.State.Moving:
    //            Moving();
    //            break;
    //        case Define.State.Idle:
    //            Idle();
    //            break;
    //        case Define.State.Attack:
    //            Attacking();
    //            break;
    //        case Define.State.JumpAttack:
    //            if (!isJumping)
    //            {
    //                StartJumpAttack(_target.transform.position);
    //            }
    //            JumpAttacking();
    //            break;
    //    }

    //}

    //void JumpAttacking()
    //{
    //    if (isJumping)
    //    {
    //        float jumpProgress = Mathf.PingPong(Time.time * jumpSpeed, jumpTime) / jumpTime;

    //        if (jumpProgress >= 0.99f)
    //        {
    //            isJumping = false;
    //            jumpAttackEffect.Play();
    //            area.SetActive(false);
    //        }
    //        float jumpHeightOffset = Mathf.Sin(jumpProgress * Mathf.PI) * jumpHeight;
    //        transform.position = Vector3.Lerp(initialPosition, targetPosition, jumpProgress) + Vector3.up * jumpHeightOffset;
    //    }
    //    else
    //    {
    //        if (jumpAttackEffect.isPlaying)
    //        {
    //            jumpAttackEffect.Stop();
    //        }
    //        State = Define.State.Idle;
    //    }
    //}

    //public void StartJumpAttack(Vector3 playerPosition)
    //{
    //    targetPosition = playerPosition;
    //    isJumping = true;
    //    area.transform.position = playerPosition;
    //    area.SetActive(true);
    //    animator.CrossFade("JumpAttack", 0.1f);
    //    initialPosition = transform.position;
    //}



    public float attackDistance = 1f;
    public float health = 100f;
    public float speed = 5f;
    public float jumpAttackCooldown = 5f;
    public Define.State State = Define.State.Idle;
    public Define.EnemyType EnemyType;

    float findDistance = 10;
    float jumpAttackTimer = 0f;
     Transform target;
     NavMeshAgent nma;
     BehaviorTree behaviorTree;
     Stat _stat;
    Animator animator;


    private void Awake()
    {
        Init();
    }
    void Start()
    {
        target = Managers.Game.GetPlayer().transform;
        behaviorTree = new BehaviorTree();

        Sequence attackSequence = new Sequence();
        
        attackSequence.AddChild(new ConditionNode(() => (transform.position - target.position).sqrMagnitude <= nma.stoppingDistance * nma.stoppingDistance));
        attackSequence.AddChild(new ActionNode(AttackWithFistsAndFeet));

        Sequence jumpAttackSequence = new Sequence();
        jumpAttackSequence.AddChild(new ConditionNode(() => health <= 40f));
        jumpAttackSequence.AddChild(new ActionNode(IncrementJumpAttackTimer));
        jumpAttackSequence.AddChild(new ConditionNode(() => jumpAttackTimer >= jumpAttackCooldown));
        jumpAttackSequence.AddChild(new ActionNode(PerformJumpAttack));
        jumpAttackSequence.AddChild(new ActionNode(ResetJumpAttackTimer));

        Sequence moveSequence = new Sequence();
        moveSequence.AddChild(new ConditionNode(() => (transform.position - target.position).sqrMagnitude >= findDistance * findDistance));
        moveSequence.AddChild(new ActionNode(MovingToPlayer));

        Selector rootSelector = new Selector();
        rootSelector.AddChild(attackSequence);
        rootSelector.AddChild(jumpAttackSequence);
        rootSelector.AddChild(moveSequence);

        behaviorTree.SetRootNode(rootSelector);
    }

    void Update()
    {
        behaviorTree.Update();
    }
    
    void Init()
    {
        animator = GetComponent<Animator>();
        nma = GetComponent<NavMeshAgent>();
        nma.speed = speed;
        EnemyType = Define.EnemyType.Boss;
        _stat = GetComponent<Stat>();
        _stat.SetStat();
    }

    void AttackWithFistsAndFeet()
    {
        if (target == null)
            return;

        Vector3 dir = target.transform.position - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        animator.CrossFade("Attack", 0.1f);
    }

    void MovingToPlayer()
    {
        if (target == null)
            return;
        Vector3 dir = target.transform.position - transform.position;
        nma.SetDestination(target.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        animator.CrossFade("Run", 0.1f);
    }

    void IncrementJumpAttackTimer()
    {
        jumpAttackTimer += Time.deltaTime;
    }

    void PerformJumpAttack()
    {
        // Perform jump attack logic
    }

    void ResetJumpAttackTimer()
    {
        jumpAttackTimer = 0f;
    }

    void PlayerHit()
    {
        if (target != null)
        {
            PlayerStat playerStat = target.GetComponent<PlayerStat>(); // «√∑π¿ÃæÓ Ω∫≈» ∞°¡Æø»
            playerStat.Attacked(_stat, target.gameObject); // ∏ÛΩ∫≈Õ¿« Ω∫≈»¿ª ≥—∞‹¡‹
        }
    }
}
