using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillController : MonoBehaviour  // ���߿� UI�и��ϱ�
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
        skillAEffect.Play();// ����Ʈ ����


        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        RaycastHit hit;
        int layerMask = 1 << LayerMask.NameToLayer("Enemy");

        if (Physics.Raycast(ray, out hit, 4f, layerMask))
        {
            GameObject enemy = hit.collider.gameObject;
            // �÷��̾�� ������Ʈ ������ ���� ����
            Vector3 dir = enemy.transform.position - transform.position;
            // �÷��̾��� forward ���Ϳ��� ����
            float angle = Vector3.Angle(dir, transform.forward);
            // ������ 150�� �����̸� �������� �Ǵ��ϰ� �������� �ش�
            if (angle <= 150f)
            {
                DungeonScene dungeonScene = FindObjectOfType<DungeonScene>(); // ���� ��
                Stat enemyStat = enemy.transform.GetComponent<Stat>();

                if (enemyStat == null) // ���� null�̸� �θ𿡼� Stat ������Ʈ ã��
                {
                    enemyStat = enemy.transform.root.GetComponentInParent<Stat>();
                }
                enemyStat.Attacked(enemyStat, this.gameObject, 100);
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

    IEnumerator SkillACoolDown()
    {
        useSkill = false;

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

        // ��ų ��� ���� ���·� �����
        useSkill = true;

        if (skillATime <= 0f)
        {
            skillAText.text = "";
            yield break;
        }
    }

}
