using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class SkillController : MonoBehaviour  // ���߿� UI�и��ϱ�
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
    class SkillData // class�� �ȸ���� ����ü�� ����� pass by value�� �ڷ�ƾ �ȿ��� ������ �ȵȴ�
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
                StartCoroutine(RestoreHealth(SKILL_C_RECOVERY_AMOUNT,SKILL_C_RECOVERY_PERSECOND)); // 5�ʿ� ����
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
        skillAEffect?.Play();// ����Ʈ ����
        audioSource?.Play();
        Vector3[] directions = new Vector3[] { transform.forward, transform.forward + transform.right, transform.forward - transform.right };

        HashSet<GameObject> enemies = new HashSet<GameObject>();
        foreach (Vector3 direction in directions)
        {
            int layerMask = 1 << LayerMask.NameToLayer("Enemy");
            Ray ray = new Ray(transform.position + Vector3.up, direction * 1.2f);
            RaycastHit[] hits = Physics.RaycastAll(ray, 2.8f, layerMask); // �迭�� ����
            foreach (var hit in hits)
            {
                enemies.Add(hit.transform.gameObject);
            }
        }
        foreach (GameObject enemy in enemies) // �迭�� ��ȸ�ϸ鼭 �� ������Ʈ�� ���� ó��
        {
            // �÷��̾�� ������Ʈ ������ ���� ����
            Vector3 dir = enemy.transform.position - transform.position;
            // �÷��̾��� forward ���Ϳ��� ����
            float angle = Vector3.Angle(dir, transform.forward);
            // ������ 160�� �����̸� �������� �Ǵ��ϰ� �������� �ش�
            if (angle <= 160f)
            {
                Stat enemyStat = enemy.transform.GetComponent<Stat>();
                if (enemyStat == null) // ���� null�̸� �θ𿡼� Stat ������Ʈ ã��
                {
                    enemyStat = enemy.transform.root.GetComponentInParent<Stat>();
                }
                float damage = stat.Attack + (stat.Attack * 1.3f);
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
