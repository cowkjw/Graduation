using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class SkillController : MonoBehaviour  // 나중에 UI분리하기
{
    const float SKILL_A_COOLDOWN = 8f;
    const float SKILL_B_COOLDOWN = 30f;
    const float SKILL_C_COOLDOWN = 60f;
    const int SKILL_A_MANA_COST = 100;
    const int SKILL_B_MANA_COST = 200;
    const int SKILL_C_MANA_COST = 250;
    const int SKILL_C_RECOVERY_PERSECOND = 5;
    const int SKILL_C_RECOVERY_AMOUNT = 10;

    [SerializeField] AudioClip aSkillSoundEffect;
    [SerializeField] AudioClip bSkillSoundEffect;
    //[SerializeField] AudioClip SKillSoundEffect;

    public ParticleSystem skillAEffect;
    public ParticleSystem skillBEffect;
    public ParticleSystem skillCEffect;
    class SkillData // class로 안만들고 구조체로 만들면 pass by value로 코루틴 안에서 변경이 안된다
    {
        public bool used;
        public float time;
    }
    bool notUsedToSkill;
    SkillData aSkillData;
    SkillData bSkillData;
    SkillData cSkillData;
    Image aSkillImage;
    Image bSkillImage;
    Image cSkillImage;
    TextMeshProUGUI aSkillText;
    TextMeshProUGUI bSkillText;
    TextMeshProUGUI cSkillText;
    PlayerController player;
    PlayerStat playerStat;
    Animator animator;
    AudioSource audioSource;
    Stat stat;
    DungeonScene dungeonScene;
    Define.SkillType SkillType;

    void Start()
    {
        notUsedToSkill = true;
        dungeonScene = FindObjectOfType<DungeonScene>();
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
        player = Managers.Game.GetPlayer()?.GetComponent<PlayerController>();
        playerStat = player?.GetComponent<PlayerStat>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        stat = GetComponent<PlayerStat>();
    }

    void SkillInputKey(Enum skill)
    {
        if (!notUsedToSkill)
        {
            if (player.State != Define.State.Skill)
            {
                notUsedToSkill = true;
            }
            return;
        }

        switch ((Define.Skill)skill)
        {
            case Define.Skill.A:
                if (aSkillData.used || playerStat.Mp < SKILL_A_MANA_COST)
                {
                    return;
                }
                aSkillData.time = SKILL_A_COOLDOWN;
                audioSource.clip = aSkillSoundEffect;
                UseSkill(aSkillImage, SKILL_A_COOLDOWN, SkillACoolDown(), SKILL_A_MANA_COST, "Skill_A");
                break;
            case Define.Skill.B:
                if (bSkillData.used || playerStat.Mp < SKILL_B_MANA_COST)
                {
                    return;
                }
                bSkillData.time = SKILL_B_COOLDOWN;
                UseSkill(bSkillImage, SKILL_B_COOLDOWN, SkillBCoolDown(), SKILL_B_MANA_COST, "Skill_B");
                break;
            case Define.Skill.C:
                if (cSkillData.used || playerStat.Mp < SKILL_C_MANA_COST)
                {
                    return;
                }
                skillCEffect.Play();
                StartCoroutine(RestoreHealth(SKILL_C_RECOVERY_AMOUNT,SKILL_C_RECOVERY_PERSECOND)); // 5초에 걸쳐
                cSkillData.time = SKILL_C_COOLDOWN;
                UseSkill(cSkillImage, SKILL_C_COOLDOWN, SkillCCoolDown(), SKILL_C_MANA_COST);
                break;
        }
        notUsedToSkill = false;
    }

    void UseSkill(Image skillImage, float coolDownTime, IEnumerator coolDownCoroutine, int manaCost, string animationName = null)
    {
        skillImage.fillAmount = 0f;
        if (animationName != null)
        {
            animator.Play(animationName);
            player.State = Define.State.Skill;
        }
        stat.Mp -= manaCost;
        StartCoroutine(coolDownCoroutine);
    }

    void OnSkill_A()
    {
        skillAEffect?.Play();// 이펙트 실행
        audioSource?.Play();
        Vector3[] directions = new Vector3[] { transform.forward, transform.forward + transform.right, transform.forward - transform.right };

        HashSet<GameObject> enemies = new HashSet<GameObject>();
        foreach (Vector3 direction in directions)
        {
            int layerMask = 1 << LayerMask.NameToLayer("Enemy");
            Ray ray = new Ray(transform.position + Vector3.up, direction * 1.2f);
            RaycastHit[] hits = Physics.RaycastAll(ray, 2.8f, layerMask); // 배열로 선언
            foreach (var hit in hits)
            {
                enemies.Add(hit.transform.gameObject);
            }
        }
        foreach (GameObject enemy in enemies) // 배열을 순회하면서 각 오브젝트에 대해 처리
        {
            // 플레이어와 오브젝트 사이의 방향 벡터
            Vector3 dir = enemy.transform.position - transform.position;
            // 플레이어의 forward 벡터와의 각도
            float angle = Vector3.Angle(dir, transform.forward);
            // 각도가 160도 이하이면 전방으로 판단하고 데미지를 준다
            if (angle <= 160f)
            {
                Stat enemyStat = enemy.transform.GetComponent<Stat>();
                if (enemyStat == null) // 만약 null이면 부모에서 Stat 컴포넌트 찾기
                {
                    enemyStat = enemy.transform.root.GetComponentInParent<Stat>();
                }
                float damage = stat.Attack + (stat.Attack * 1.3f);
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

    IEnumerator RestoreHealth(int recoveryAmount, int seconds)
    {

        if (playerStat == null)
        {
            yield break;
        }

        for (int time = 1; time <= seconds; time++)
        {
            playerStat.Hp = Math.Min(playerStat.Hp + recoveryAmount, playerStat.MaxHp);
            yield return new WaitForSeconds(1f);
        }
        skillCEffect.Stop();
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
