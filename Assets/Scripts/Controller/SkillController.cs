using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillController : MonoBehaviour  // 나중에 UI분리하기
{
    Image skillAImage;
    Image skillBImage;
    Image skillCImage;
    TextMeshProUGUI skillAText;
    TextMeshProUGUI skillBText;
    TextMeshProUGUI skillCText;

    public ParticleSystem skillAEffect;
    public ParticleSystem skillBEffect;
    public ParticleSystem skillCEffect;

    const float SKILL_A_COOLDOWN = 8f;
    const float SKILL_B_COOLDOWN = 20f;
    const float SKILL_C_COOLDOWN = 60f;
    bool useSkillA;
    bool useSkillB;
    bool useSkillC;

    bool notUsedToSkill;

    float skillATime;
    float skillBTime;
    float skillCTime;

    PlayerController player;
    Animator animator;
    Stat stat;

    void Start()
    {
        notUsedToSkill = true;
        useSkillA = true;
        useSkillB = true;
        useSkillC = true;
        skillATime = 8f;
        skillBTime = 20f;
        skillCTime = 60f;

        skillAImage = GameObject.Find("Skill1_Cool").GetComponent<Image>();
        skillAText = GameObject.Find("Skill1_Cool_Text").GetComponent<TextMeshProUGUI>();
        skillBImage = GameObject.Find("Skill2_Cool").GetComponent<Image>();
        skillBText = GameObject.Find("Skill2_Cool_Text").GetComponent<TextMeshProUGUI>();
        skillCImage = GameObject.Find("Skill3_Cool").GetComponent<Image>();
        skillCText = GameObject.Find("Skill3_Cool_Text").GetComponent<TextMeshProUGUI>();

        Managers.Input.KeyboardAction -= SkillInputKey;
        Managers.Input.KeyboardAction += SkillInputKey;
        player = Managers.Game.GetPlayer().GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        stat = GetComponent<PlayerStat>();
    }

    void SkillInputKey(Enum skill)
    {
        if (!notUsedToSkill)
        {
            return;
        }
        
        switch ((Define.Skill)skill)
        {
            case Define.Skill.A:
                if (!useSkillA) return;
                skillAImage.fillAmount = 0f;
                skillATime = 8f;
                animator.Play("Skill_A");
                stat.Mp -= 100;
                player.State = Define.State.Skill;
                StartCoroutine(SkillACoolDown());
                break;
            case Define.Skill.B:
                skillBImage.fillAmount = 0f;
                skillBTime = 20f;
                animator.Play("Skill_B");
                player.State = Define.State.Skill;
                StartCoroutine(SkillBCoolDown());
                break;
            case Define.Skill.C:
                skillCImage.fillAmount = 0f;
                skillCTime = 60f;
                skillCEffect.Play();
                StartCoroutine(SkillCCoolDown());
                break;
        }
        notUsedToSkill = false;
    }

    void OnSkill_A()
    {
        skillAEffect.Play();// 이펙트 실행

        Vector3[] directions = new Vector3[] { transform.forward, transform.forward + transform.right, transform.forward - transform.right };

        foreach (Vector3 direction in directions)
        {
            Ray ray = new Ray(transform.position + Vector3.up, direction * 1.2f);
            RaycastHit[] hits; // 배열로 선언
            int layerMask = 1 << LayerMask.NameToLayer("Enemy");

            hits = Physics.RaycastAll(ray, 3f, layerMask);
            foreach (RaycastHit hit in hits) // 배열을 순회하면서 각 오브젝트에 대해 처리
            {
                GameObject enemy = hit.collider.gameObject;
                // 플레이어와 오브젝트 사이의 방향 벡터
                Vector3 dir = enemy.transform.position - transform.position;
                // 플레이어의 forward 벡터와의 각도
                float angle = Vector3.Angle(dir, transform.forward);
                // 각도가 160도 이하이면 전방으로 판단하고 데미지를 준다
                if (angle <= 160f)
                {
                    DungeonScene dungeonScene = FindObjectOfType<DungeonScene>(); // 던전 씬
                    Stat enemyStat = enemy.transform.GetComponent<Stat>();
                    if (enemyStat == null) // 만약 null이면 부모에서 Stat 컴포넌트 찾기
                    {
                        enemyStat = enemy.transform.root.GetComponentInParent<Stat>();
                    }
                    float damage = stat.Attack + (stat.Attack * 1.3f);
                    Debug.Log(damage);
                    enemyStat.Attacked(stat, enemy, (int)damage);
                    if (dungeonScene != null) // 던전 씬이 null이 아니라면 HP UI 켜기
                    {
                        dungeonScene.ObjStat = enemyStat;
                        dungeonScene.ObjName = enemyStat.name;
                        dungeonScene.ObjNameText.gameObject.SetActive(true);
                        dungeonScene.HpBar.gameObject.SetActive(true);
                    }
                }
            }
        }

    }

    void OnSkill_B()
    {
        skillBEffect.Play();
    }

    IEnumerator SkillACoolDown()
    {
        useSkillA = false;

        // 쿨타임 시간 동안 반복하기
        while (skillAImage.fillAmount < 1f)
        {
            // Fill Amount와 함께 실수형 변수를 증가시키기
            skillAImage.fillAmount += Time.deltaTime / SKILL_A_COOLDOWN;
            if ((int)skillATime != 0) // 0표시 안하기
            {
                skillAText.text = $"{(int)skillATime}";
            }
            skillATime -= Time.deltaTime;
            // 다음 프레임까지 대기하기
            yield return null;
        }

        if (skillATime <= 0f)
        {
            skillAText.text = "";
            useSkillA = true;
            yield break;
        }
    }

    IEnumerator SkillBCoolDown()
    {
        useSkillB = false;

        // 쿨타임 시간 동안 반복하기
        while (skillBImage.fillAmount < 1f)
        {
            // Fill Amount와 함께 실수형 변수를 증가시키기
            skillBImage.fillAmount += Time.deltaTime / SKILL_B_COOLDOWN;
            if ((int)skillBTime != 0) // 0표시 안하기
            {
                skillBText.text = $"{(int)skillBTime}";
            }
            skillBTime -= Time.deltaTime;
            // 다음 프레임까지 대기하기
            yield return null;
        }

        // 스킬 사용 가능 상태로 만들기
        useSkillB = true;

        if (skillBTime <= 0f)
        {
            skillBText.text = "";
            yield break;
        }
    }

    IEnumerator SkillCCoolDown()
    {
        useSkillC = false;

        // 쿨타임 시간 동안 반복하기
        while (skillCImage.fillAmount < 1f)
        {
            // Fill Amount와 함께 실수형 변수를 증가시키기
            skillCImage.fillAmount += Time.deltaTime / SKILL_C_COOLDOWN;
            if ((int)skillATime != 0) // 0표시 안하기
            {
                skillCText.text = $"{(int)skillCTime}";
            }
            skillCTime -= Time.deltaTime;
            // 다음 프레임까지 대기하기
            yield return null;
        }

        // 스킬 사용 가능 상태로 만들기
        useSkillC = true;

        if (skillCTime <= 0f)
        {
            skillCText.text = "";
            yield break;
        }
    }


    void OnEndSkillAnim() // 스킬 애니메이션이 끝났다면 
    {
        notUsedToSkill = true;
        player.State = Define.State.Idle;
    } 

}
