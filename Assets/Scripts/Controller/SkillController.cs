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
        skill_a_effect.Play();// 이펙트 실행
        Ray ray = new Ray(transform.position+Vector3.up, transform.forward);
        RaycastHit hit;
        // 레이가 닿은 오브젝트가 있는지 확인한다
        int layerMask = 1 << LayerMask.NameToLayer("Enemy");

        if (Physics.Raycast(ray, out hit, 4f,layerMask))
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

}
