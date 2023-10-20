using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BaseScene : MonoBehaviour
{
    protected Vector3 PlayerPos;
    protected GameObject Player = null;
    protected GameObject Ui = null;
    protected GameObject Inventory = null; //inventory UI
    protected GameObject NpcUI = null; // NPC UI ������Ʈ   
    protected Slider PlayerHpBar;
    protected Slider PlayerMpBar;
    protected Stat PlayerStat;
    protected TextMeshProUGUI PlayerLevelText;

    void Awake()
    {
        Init();
    }

    void Start()
    {
        PlayerStat = Managers.Game.GetPlayer().GetComponent<PlayerStat>();
        Inventory = GameObject.Find("UI").transform.Find("Inventory").gameObject;
        GameObject playerUI = GameObject.FindGameObjectWithTag("PlayerUI");
        PlayerHpBar = playerUI.transform.GetChild(1).GetComponent<Slider>();
        PlayerMpBar = playerUI.transform.GetChild(2).GetComponent<Slider>();
        PlayerLevelText = GameObject.FindGameObjectWithTag("LevelUI").GetComponent<TextMeshProUGUI>();
        Managers.Input.KeyboardAction -= InputUIHotKey;
        Managers.Input.KeyboardAction += InputUIHotKey;
    }

    protected virtual void Update() // �÷��̾��� HPUI ������Ʈ�� ����
    {
        PlayerHpBar.value = PlayerStat.Hp / (float)PlayerStat.MaxHp;
        PlayerMpBar.value = PlayerStat.Mp / (float)PlayerStat.MaxMp;
        PlayerLevelText.text = $"LELVEL {Managers.Data.PlayerStat.Level}";
    }

    public virtual void Init()
    {
        Managers.Clear(); // �����ߴ��� nulló��
        Ui = Instantiate(Resources.Load<GameObject>("Prefabs/UI_Prefab/UI"));
        Ui.name = "UI";
    }

    void InputUIHotKey(Enum uiType)
    {
        if ((Define.UI)uiType == Define.UI.Inventory)
        {
            if (Inventory.activeSelf) // �κ��丮�� �����ִٸ�
            {
                if (GameObject.FindGameObjectWithTag("NPC")) // �ش� �ʿ� ���� NPC�� �ִٸ�
                {
                    if (NpcUI?.activeSelf == false) // NPC ��üũ ������ ���������� �� �κ��丮 ��Ȱ��ȭ
                    {
                        Inventory.SetActive(false);
                    }
                }
                else
                {
                    Inventory.SetActive(false);
                }
            }
            else
            {
                Inventory.SetActive(true);
            }
        }
    }
    protected virtual void ClickNPC(Define.MouseState evt) { }
    public virtual void SetPlayerHp() { }
}
