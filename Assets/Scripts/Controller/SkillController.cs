using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillController : MonoBehaviour  // 나중에 UI분리하기
{
    Image skillAImage;
    TextMeshProUGUI skillAText;

    public ParticleSystem skillAEffect;
    public ParticleSystem skillBEffect;
    public ParticleSystem skillCEffect;

    const float SKILL_A_COOLDOWN = 8f;
    bool useSkill;

    float skillATime;

    PlayerController player;
    Animator animator;

    void Start()
    {
        useSkill = true;
        skillATime = 8f;
        skillAImage = GameObject.Find("Skill1_Cool").GetComponent<Image>();
        skillAText = GameObject.Find("Skill1_Cool_Text").GetComponent<TextMeshProUGUI>();
        Managers.Input.KeyboardAction -= SkillInputKey;
        Managers.Input.KeyboardAction += SkillInputKey;
        player = Managers.Game.GetPlayer().GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
    }

    void SkillInputKey(Enum skill)
    {
        if (!useSkill)
        {
            return;
        }
        player.State = Define.State.Skill;
        switch ((Define.Skill)skill)
        {
            case Define.Skill.A:
                skillAImage.fillAmount = 0f;
                skillATime = 8f;
                StartCoroutine(SkillACoolDown());
                animator.Play("Skill_A");
                break;
            case Define.Skill.B:
                skillBEffect.Play();
                break;
            case Define.Skill.C:
                skillCEffect.Play();
                break;
        }
    }

    void OnSkill_A()
    {
        skillAEffect.Play();// 이펙트 실행


        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        RaycastHit hit;
        int layerMask = 1 << LayerMask.NameToLayer("Enemy");

        if (Physics.Raycast(ray, out hit, 4f, layerMask))
        {
            GameObject enemy = hit.collider.gameObject;
            // 플레이어와 오브젝트 사이의 방향 벡터
            Vector3 dir = enemy.transform.position - transform.position;
            // 플레이어의 forward 벡터와의 각도
            float angle = Vector3.Angle(dir, transform.forward);
            // 각도가 150도 이하이면 전방으로 판단하고 데미지를 준다
            if (angle <= 150f)
            {
                DungeonScene dungeonScene = FindObjectOfType<DungeonScene>(); // 던전 씬
                Stat enemyStat = enemy.transform.GetComponent<Stat>();

                if (enemyStat == null) // 만약 null이면 부모에서 Stat 컴포넌트 찾기
                {
                    enemyStat = enemy.transform.root.GetComponentInParent<Stat>();
                }
                enemyStat.Attacked(enemyStat, this.gameObject, 100);
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

    IEnumerator SkillACoolDown()
    {
        useSkill = false;

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

        // 스킬 사용 가능 상태로 만들기
        useSkill = true;

        if (skillATime <= 0f)
        {
            skillAText.text = "";
            yield break;
        }
    }

}
