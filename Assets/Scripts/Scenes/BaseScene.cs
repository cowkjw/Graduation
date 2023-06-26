using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BaseScene : MonoBehaviour
{
    public Define.Scene SceneType = Define.Scene.Base;

    protected Vector3 PlayerPos;
    protected GameObject Player = null;
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
        GameObject playerUI = GameObject.FindGameObjectWithTag("PlayerUI");
        PlayerHpBar = playerUI.transform.GetChild(1).GetComponent<Slider>();
        PlayerMpBar = playerUI.transform.GetChild(2).GetComponent<Slider>();
        PlayerLevelText = GameObject.FindGameObjectWithTag("LevelUI").GetComponent<TextMeshProUGUI>();
    }

    protected virtual void Update() // �÷��̾��� HPUI ������Ʈ�� ����
    {
        if(Managers.UI.PlayerHpBar==null)
        {
            Managers.UI.BindUI();
        }
        Managers.UI.PlayerHpBar.value = PlayerStat.Hp / (float)PlayerStat.MaxHp;
        Managers.UI.PlayerMpBar.value = PlayerStat.Mp / (float)PlayerStat.MaxMp;
        PlayerLevelText.text = $"LELVEL {Managers.Data.PlayerStat.Level}";
    }

    public virtual void Init()
    {
        Managers.Clear(); // ���� ����
    }

    //void InputUIHotKey(Enum uiType)
    //{
    //    if ((Define.UI)uiType == Define.UI.Inventory)
    //    {
    //        if (Inventory.activeSelf) // �κ��丮�� �����ִٸ�
    //        {
    //            if (GameObject.FindGameObjectWithTag("NPC")) // �ش� �ʿ� ���� NPC�� �ִٸ�
    //            {
    //                if (NpcUI?.activeSelf == false) // NPC ��üũ ������ ���������� �� �κ��丮 ��Ȱ��ȭ
    //                {
    //                    Inventory.SetActive(false);
    //                }
    //            }
    //            else
    //            {
    //                Inventory.SetActive(false);
    //            }
    //        }
    //        else
    //        {
    //            Inventory.SetActive(true);
    //        }
    //    }
    //}
    protected virtual void ClickNPC(Define.MouseState evt) { }
    public virtual void SetPlayerHp() { }
}
