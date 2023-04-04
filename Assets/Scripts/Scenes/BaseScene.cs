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
    protected GameObject npcUI = null; // NPC UI ������Ʈ   
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

    protected virtual void Update() // �÷��̾��� HPUI ������Ʈ�� ����
    {
        _playerHpBar.value = _playerStat.Hp / (float)_playerStat.MaxHp;
        _playerLevelText.text = $"LELVEL {Managers.Data.PlayerStat.Level}";
    }

    public virtual void Init()
    {
        Managers.Clear(); // �����ߴ��� nulló��
        Ui = Instantiate(Resources.Load<GameObject>("Prefabs/UI_Prefab/UI"));
        Ui.name = "UI";
    }

    void InputUIHotKey(Define.UI uiType)
    {
        if (uiType == Define.UI.Inventory)
        {
            if (inventory.activeSelf) // �κ��丮�� �����ִٸ�
            {
                if (GameObject.FindGameObjectWithTag("NPC")) // �ش� �ʿ� ���� NPC�� �ִٸ�
                {
                    if (npcUI?.activeSelf == false) // NPC ��üũ ������ ���������� �� �κ��丮 ��Ȱ��ȭ
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
