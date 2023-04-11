using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : EnemyController
{


    public float jumpHeight = 50f;
    public float jumpTime = 10f;
    public float jumpSpeed = 0.5f;
    public GameObject areaPrefab;
    public ParticleSystem jumpAttackEffect;

    GameObject area;
    MeshRenderer meshRenderers;
    Material material;
    Color matColor;
    Color matColor1;
    Color matColor2;

    Animator animator;
    private Vector3 initialPosition;
    private bool isJumping = false;
    private Vector3 targetPosition;

    private void OnEnable()
    {
        base.Init();
        EnemyType = Define.EnemyType.Boss;
        _findRange = 6f;
    }


    protected override void Start()
    {
        animator = GetComponent<Animator>();

        _target = Managers.Game.GetPlayer();
        area = Instantiate(areaPrefab, transform.position, Quaternion.identity);
        area.SetActive(false);
    }

    protected override void Update()
    {
        if (State != Define.State.Die)
            Dying();

        switch (State)
        {
            case Define.State.Moving:
                Moving();
                break;
            case Define.State.Idle:
                Idle();
                break;
            case Define.State.Attack:
                Attacking();
                break;
            case Define.State.JumpAttack:
                if (!isJumping)
                {
                    StartJumpAttack(_target.transform.position);
                }
                JumpAttacking();
                break;
        }

    }

    void JumpAttacking()
    {
        if (isJumping)
        {
            float jumpProgress = Mathf.PingPong(Time.time * jumpSpeed, jumpTime) / jumpTime;

            if (jumpProgress >= 0.99f)
            {
                isJumping = false;
                jumpAttackEffect.Play();
                area.SetActive(false);
            }
            float jumpHeightOffset = Mathf.Sin(jumpProgress * Mathf.PI) * jumpHeight;
            transform.position = Vector3.Lerp(initialPosition, targetPosition, jumpProgress) + Vector3.up * jumpHeightOffset;
        }
        else
        {
            if (jumpAttackEffect.isPlaying)
            {
                jumpAttackEffect.Stop();
            }
            State = Define.State.Idle;
        }
    }

    public void StartJumpAttack(Vector3 playerPosition)
    {
        targetPosition = playerPosition;
        isJumping = true;
        area.transform.position = playerPosition;
        area.SetActive(true);
        animator.CrossFade("JumpAttack", 0.1f);
        initialPosition = transform.position;
    }
}
