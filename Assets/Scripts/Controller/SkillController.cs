using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;




public class SkillController : MonoBehaviour  // ���߿� UI�и��ϱ�
{ 
    struct SkillData // ���� heap�������� �Ƚᵵ �ɵ��ϴ�
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
        skillAEffect.Play();// ����Ʈ ����

        Vector3[] directions = new Vector3[] { transform.forward, transform.forward + transform.right, transform.forward - transform.right };

        foreach (Vector3 direction in directions)
        {
            Ray ray = new Ray(transform.position + Vector3.up, direction * 1.2f);
            RaycastHit[] hits; // �迭�� ����
            int layerMask = 1 << LayerMask.NameToLayer("Enemy");

            hits = Physics.RaycastAll(ray, 2.8f, layerMask);
            foreach (RaycastHit hit in hits) // �迭�� ��ȸ�ϸ鼭 �� ������Ʈ�� ���� ó��
            {
                GameObject enemy = hit.collider.gameObject;
                // �÷��̾�� ������Ʈ ������ ���� ����
                Vector3 dir = enemy.transform.position - transform.position;
                // �÷��̾��� forward ���Ϳ��� ����
                float angle = Vector3.Angle(dir, transform.forward);
                // ������ 160�� �����̸� �������� �Ǵ��ϰ� �������� �ش�
                if (angle <= 160f)
                {
                    DungeonScene dungeonScene = FindObjectOfType<DungeonScene>(); // ���� ��
                    Stat enemyStat = enemy.transform.GetComponent<Stat>();
                    if (enemyStat == null) // ���� null�̸� �θ𿡼� Stat ������Ʈ ã��
                    {
                        enemyStat = enemy.transform.root.GetComponentInParent<Stat>();
                    }
                    float damage = stat.Attack + (stat.Attack * 1.3f);
                    Debug.Log(damage);
                    enemyStat.Attacked(stat, enemy, (int)damage);
                    if (dungeonScene != null) // ���� ���� null�� �ƴ϶�� HP UI �ѱ�
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
        int layerMask = 1 << LayerMask.NameToLayer("Enemy"); // �� ���̾� ����ũ
        Collider[] enemies = Physics.OverlapSphere(transform.position, 3f, layerMask);

        foreach (var enemy in enemies)
        {
            Stat enemyStat = enemy.transform.GetComponent<Stat>();
            if (enemyStat == null) // ���� null�̸� �θ𿡼� Stat ������Ʈ ã��
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
            if ((int)skillData.time != 0) // 0ǥ�� ���ϱ�
            {
                skillText.text = $"{(int)skillData.time}";
            }
            skillData.time -= Time.deltaTime;
            yield return null;
        }

        // ��ų ��� ���� ���·� �����
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

    void OnEndSkillAnim() // ��ų �ִϸ��̼��� �����ٸ� 
    {
        notUsedToSkill = true;
        player.State = Define.State.Idle;
    } 

}
