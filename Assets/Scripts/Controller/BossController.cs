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

    private Vector3 initialPosition;
    private bool isJumping = false;
    private Vector3 targetPosition;

    private void OnEnable()
    {
    }


    Animator animator;
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        initialPosition = transform.position;
        Invoke("Test", 5f);

        //Invoke("StartJumpAttack", 2f);
        // StartJumpAttack();
    }
    void Test()
    {
        StartJumpAttack(Managers.Game.GetPlayer().transform.position);
    }

    protected override void Update()
    {
        if (isJumping)
        {
            float jumpProgress = Mathf.PingPong(Time.time * jumpSpeed, jumpTime) / jumpTime;

            if (jumpProgress >= 0.99f)
            {
                isJumping = false;
                jumpAttackEffect.Play();

                Destroy(area, 1f);
                area = null;
            }
            float jumpHeightOffset = Mathf.Sin(jumpProgress * Mathf.PI) * jumpHeight;
            transform.position = Vector3.Lerp(initialPosition, targetPosition, jumpProgress) + Vector3.up * jumpHeightOffset;

        }
        else
        {
            animator.CrossFade("Idle", 0.1f);
            if (jumpAttackEffect.isPlaying)
            {
                jumpAttackEffect.Stop();
            }

        }
    }

    public void StartJumpAttack(Vector3 playerPosition)
    {
        targetPosition = playerPosition;
        isJumping = true;
        area = Instantiate(areaPrefab, playerPosition, Quaternion.identity);
        material = area.GetComponent<MeshRenderer>().material;
        matColor = material.color;
        matColor1 = new Color(matColor.r, matColor.g, matColor.b, 0f);
        matColor2 = new Color(matColor.r, matColor.g, matColor.b, 1f);
        animator.CrossFade("Attack", 0.1f);
    }
}
