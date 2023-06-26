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

    protected virtual void Update() // 플레이어의 HPUI 업데이트를 위함
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
        Managers.Clear(); // 구독 정리
    }

    //void InputUIHotKey(Enum uiType)
    //{
    //    if ((Define.UI)uiType == Define.UI.Inventory)
    //    {
    //        if (Inventory.activeSelf) // 인벤토리가 켜져있다면
    //        {
    //            if (GameObject.FindGameObjectWithTag("NPC")) // 해당 맵에 상점 NPC가 있다면
    //            {
    //                if (NpcUI?.activeSelf == false) // NPC 널체크 상점이 안켜져있을 때 인벤토리 비활성화
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
