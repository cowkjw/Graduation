using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BaseScene : MonoBehaviour
{

    protected Vector3 playerPos;
    protected GameObject _player = null;
    protected GameObject Ui = null;
    protected GameObject inventory = null; //inventory UI
    protected GameObject npcUI = null; // NPC UI 오브젝트   
    protected Slider _playerHpBar;
    protected Stat _playerStat;
    protected TextMeshProUGUI _playerLevelText;


    void Awake()
    {
        Init();
    }

    void Start()
    {
        _playerStat = Managers.Game.GetPlayer().GetComponent<PlayerStat>();
        inventory = GameObject.Find("UI").transform.Find("Inventory").gameObject;
        _playerHpBar = GameObject.FindGameObjectWithTag("PlayerUI").transform.GetChild(1).GetComponent<Slider>();
        _playerLevelText = GameObject.FindGameObjectWithTag("LevelUI").GetComponent<TextMeshProUGUI>();
        Managers.Input.KeyboardAction -= InputUIHotKey;
        Managers.Input.KeyboardAction += InputUIHotKey;
    }

    protected virtual void Update() // 플레이어의 HPUI 업데이트를 위함
    {
        _playerHpBar.value = _playerStat.Hp / (float)_playerStat.MaxHp;
        _playerLevelText.text = $"LELVEL {Managers.Data.PlayerStat.Level}";
    }

    public virtual void Init()
    {
        Managers.Clear(); // 구독했던거 null처리
        Ui = Instantiate(Resources.Load<GameObject>("Prefabs/UI_Prefab/UI"));
        Ui.name = "UI";
    }

    void InputUIHotKey(Define.UI uiType)
    {
        if (uiType == Define.UI.Inventory)
        {
            if (inventory.activeSelf) // 인벤토리가 켜져있다면
            {
                if (GameObject.FindGameObjectWithTag("NPC")) // 해당 맵에 상점 NPC가 있다면
                {
                    if (npcUI?.activeSelf == false) // NPC 널체크 상점이 안켜져있을 때 인벤토리 비활성화
                    {
                        inventory.SetActive(false);
                    }
                }
                else
                {
                    inventory.SetActive(false);
                }
            }
            else
            {
                inventory.SetActive(true);
            }
        }
    }
    protected virtual void ClickNPC(Define.MouseState evt) { }
    public virtual void SetPlayerHp() { }
}
