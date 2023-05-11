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
    protected GameObject NpcUI = null; // NPC UI 오브젝트   
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

    protected virtual void Update() // 플레이어의 HPUI 업데이트를 위함
    {
        PlayerHpBar.value = PlayerStat.Hp / (float)PlayerStat.MaxHp;
        PlayerMpBar.value = PlayerStat.Mp / (float)PlayerStat.MaxMp;
        PlayerLevelText.text = $"LELVEL {Managers.Data.PlayerStat.Level}";
    }

    public virtual void Init()
    {
        Managers.Clear(); // 구독했던거 null처리
        Ui = Instantiate(Resources.Load<GameObject>("Prefabs/UI_Prefab/UI"));
        Ui.name = "UI";
    }

    void InputUIHotKey(Enum uiType)
    {
        if ((Define.UI)uiType == Define.UI.Inventory)
        {
            if (Inventory.activeSelf) // 인벤토리가 켜져있다면
            {
                if (GameObject.FindGameObjectWithTag("NPC")) // 해당 맵에 상점 NPC가 있다면
                {
                    if (NpcUI?.activeSelf == false) // NPC 널체크 상점이 안켜져있을 때 인벤토리 비활성화
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
