using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;




public class SkillController : MonoBehaviour  // 나중에 UI분리하기
{ 
    struct SkillData // 굳이 heap영역까지 안써도 될듯하다
    {
        public bool used;
        public float time;
    }

    Image aSkillImage;
    Image bSkillImage;
    Image cSkillImage;
    TextMeshProUGUI aSkillText;
    TextMeshProUGUI bSkillText;
    TextMeshProUGUI cSkillText;

    public ParticleSystem skillAEffect;
    public ParticleSystem skillBEffect;
    public ParticleSystem skillCEffect;

    const float SKILL_A_COOLDOWN = 8f;
    const float SKILL_B_COOLDOWN = 30f;
    const float SKILL_C_COOLDOWN = 60f;

    bool notUsedToSkill;

    SkillData aSkillData;
    SkillData bSkillData;
    SkillData cSkillData;
    PlayerController player;
    Animator animator;
    Stat stat;

    void Start()
    {
        notUsedToSkill = true;

        aSkillData = new SkillData();
        aSkillData.time = SKILL_A_COOLDOWN;
        aSkillData.used = false;
        aSkillImage = GameObject.Find("Skill1_Cool").GetComponent<Image>();
        aSkillText = GameObject.Find("Skill1_Cool_Text").GetComponent<TextMeshProUGUI>();

        bSkillData = new SkillData();
        bSkillData.time = SKILL_B_COOLDOWN;
        bSkillData.used = false;
        bSkillImage = GameObject.Find("Skill2_Cool").GetComponent<Image>();
        bSkillText = GameObject.Find("Skill2_Cool_Text").GetComponent<TextMeshProUGUI>();

        cSkillData = new SkillData();
        cSkillData.time = SKILL_C_COOLDOWN;
        cSkillData.used = false;
        cSkillImage = GameObject.Find("Skill3_Cool").GetComponent<Image>();
        cSkillText = GameObject.Find("Skill3_Cool_Text").GetComponent<TextMeshProUGUI>();

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
                if (aSkillData.used) return;
                aSkillData.time = SKILL_A_COOLDOWN;
                UseSkill(aSkillImage, SKILL_A_COOLDOWN, "Skill_A", SkillACoolDown(),100);
                break;
            case Define.Skill.B:
                if (bSkillData.used) return;
                bSkillData.time = SKILL_B_COOLDOWN;
                UseSkill(bSkillImage, SKILL_B_COOLDOWN, "Skill_B", SkillBCoolDown(),100);
                break;
            case Define.Skill.C:
                if (cSkillData.used) return;
                cSkillData.time = SKILL_C_COOLDOWN;
                skillCEffect.Play();
                UseSkill(cSkillImage, SKILL_C_COOLDOWN, "Skill_C", SkillCCoolDown(),100);
                break;
        }
        notUsedToSkill = false;
    }

    void UseSkill(Image skillImage, float coolDownTime, string animationName, IEnumerator coolDownCoroutine,int manaCost)
    {
        skillImage.fillAmount = 0f;
        animator.Play(animationName);
        player.State = Define.State.Skill;
        stat.Mp -= manaCost;
        StartCoroutine(coolDownCoroutine);
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

            hits = Physics.RaycastAll(ray, 2.8f, layerMask);
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
        int layerMask = 1 << LayerMask.NameToLayer("Enemy"); // 적 레이어 마스크
        Collider[] enemies = Physics.OverlapSphere(transform.position, 3f, layerMask);

        foreach (var enemy in enemies)
        {
            Stat enemyStat = enemy.transform.GetComponent<Stat>();
            if (enemyStat == null) // 만약 null이면 부모에서 Stat 컴포넌트 찾기
            {
                enemyStat = enemy.transform.root.GetComponentInParent<Stat>();
            }
            float damage = stat.Attack + (stat.Attack * 1.3f);
            enemyStat.Attacked(stat, enemy.gameObject, (int)damage);
        }
    }


    IEnumerator UpdateCoolDown(Image skillImage, TextMeshProUGUI skillText, float coolDownTime, SkillData skillData)//, bool skillUsed, float skillTime)
    {
        skillData.used = true;

        while (skillImage.fillAmount < 1f)
        {
            skillImage.fillAmount += Time.deltaTime / coolDownTime;
            if ((int)skillData.time != 0) // 0표시 안하기
            {
                skillText.text = $"{(int)skillData.time}";
            }
            skillData.time -= Time.deltaTime;
            yield return null;
        }

        // 스킬 사용 가능 상태로 만들기
        if (skillData.time <= 0f)
        {
            skillText.text = "";
            skillData.used = false;
            yield break;
        }
    }

    IEnumerator SkillACoolDown()
    {
        yield return StartCoroutine(UpdateCoolDown(aSkillImage, aSkillText, SKILL_A_COOLDOWN, aSkillData));
    }

    IEnumerator SkillBCoolDown()
    {
        yield return StartCoroutine(UpdateCoolDown(bSkillImage, bSkillText, SKILL_B_COOLDOWN, bSkillData));
    }

    IEnumerator SkillCCoolDown()
    {
        yield return StartCoroutine(UpdateCoolDown(cSkillImage, cSkillText, SKILL_C_COOLDOWN, cSkillData));
    }

    void OnEndSkillAnim() // 스킬 애니메이션이 끝났다면 
    {
        notUsedToSkill = true;
        player.State = Define.State.Idle;
    } 

}
