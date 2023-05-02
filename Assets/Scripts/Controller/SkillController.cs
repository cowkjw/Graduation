using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour
{

    public ParticleSystem skill_a_effect;
    public ParticleSystem skill_b_effect;
    public ParticleSystem skill_c_effect;

    PlayerController player;
    Animator animator;
    
    void Start()
    {
        Managers.Input.KeyboardAction -= SkillInputKey;
        Managers.Input.KeyboardAction += SkillInputKey;
        player = Managers.Game.GetPlayer().GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
    }

    void SkillInputKey(Enum skill)
    {
        switch ((Define.Skill)skill)
        {
            case Define.Skill.A:
                player.State = Define.State.Skill;
                animator.Play("Skill_A");
                break;
            case Define.Skill.B:
                skill_b_effect.Play();
                break;
            case Define.Skill.C:
                skill_c_effect.Play();
                break;
        }
    }

    void OnSkill_A()
    {
        skill_a_effect.Play();// ����Ʈ ����
        Ray ray = new Ray(transform.position+Vector3.up, transform.forward);
        RaycastHit hit;
        // ���̰� ���� ������Ʈ�� �ִ��� Ȯ���Ѵ�
        int layerMask = 1 << LayerMask.NameToLayer("Enemy");

        if (Physics.Raycast(ray, out hit, 4f,layerMask))
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

}
