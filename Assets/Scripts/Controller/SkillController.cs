using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillController : MonoBehaviour  // ���߿� UI�и��ϱ�
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
        skillAEffect.Play();// ����Ʈ ����

        Vector3[] directions = new Vector3[] { transform.forward, transform.forward + transform.right, transform.forward - transform.right };

        foreach (Vector3 direction in directions)
        {
            Ray ray = new Ray(transform.position + Vector3.up, direction * 1.2f);
            RaycastHit[] hits; // �迭�� ����
            int layerMask = 1 << LayerMask.NameToLayer("Enemy");

            hits = Physics.RaycastAll(ray, 3f, layerMask);
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
    }

    IEnumerator SkillACoolDown()
    {
        useSkillA = false;

        // ��Ÿ�� �ð� ���� �ݺ��ϱ�
        while (skillAImage.fillAmount < 1f)
        {
            // Fill Amount�� �Բ� �Ǽ��� ������ ������Ű��
            skillAImage.fillAmount += Time.deltaTime / SKILL_A_COOLDOWN;
            if ((int)skillATime != 0) // 0ǥ�� ���ϱ�
            {
                skillAText.text = $"{(int)skillATime}";
            }
            skillATime -= Time.deltaTime;
            // ���� �����ӱ��� ����ϱ�
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

        // ��Ÿ�� �ð� ���� �ݺ��ϱ�
        while (skillBImage.fillAmount < 1f)
        {
            // Fill Amount�� �Բ� �Ǽ��� ������ ������Ű��
            skillBImage.fillAmount += Time.deltaTime / SKILL_B_COOLDOWN;
            if ((int)skillBTime != 0) // 0ǥ�� ���ϱ�
            {
                skillBText.text = $"{(int)skillBTime}";
            }
            skillBTime -= Time.deltaTime;
            // ���� �����ӱ��� ����ϱ�
            yield return null;
        }

        // ��ų ��� ���� ���·� �����
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

        // ��Ÿ�� �ð� ���� �ݺ��ϱ�
        while (skillCImage.fillAmount < 1f)
        {
            // Fill Amount�� �Բ� �Ǽ��� ������ ������Ű��
            skillCImage.fillAmount += Time.deltaTime / SKILL_C_COOLDOWN;
            if ((int)skillATime != 0) // 0ǥ�� ���ϱ�
            {
                skillCText.text = $"{(int)skillCTime}";
            }
            skillCTime -= Time.deltaTime;
            // ���� �����ӱ��� ����ϱ�
            yield return null;
        }

        // ��ų ��� ���� ���·� �����
        useSkillC = true;

        if (skillCTime <= 0f)
        {
            skillCText.text = "";
            yield break;
        }
    }


    void OnEndSkillAnim() // ��ų �ִϸ��̼��� �����ٸ� 
    {
        notUsedToSkill = true;
        player.State = Define.State.Idle;
    } 

}
