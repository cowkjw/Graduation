using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseScene : MonoBehaviour
{

    protected Vector3 _playerPos;
    protected GameObject _player = null;
    protected GameObject _ui = null;
    protected GameObject inventory = null; //inventory UI
    protected GameObject npcUI = null; // NPC UI ������Ʈ

    protected Slider _playerHpBar;
    protected Stat _playerStat;


    void Awake()
    {
        Init();
    }

    private void Start()
    {
        _playerStat = Managers.Game.Player.GetComponent<PlayerStat>();
        inventory = GameObject.Find("UI").transform.Find("Inventory").gameObject;
        _playerHpBar = GameObject.FindGameObjectWithTag("PlayerUI").transform.GetChild(1).GetComponent<Slider>();
        Managers.Input.KeyboardAction -= InputUIHotKey;
        Managers.Input.KeyboardAction += InputUIHotKey;
    }
    protected virtual void Update() // �÷��̾��� HPUI ������Ʈ�� ����
    {
        _playerHpBar.value = _playerStat.Hp / (float)_playerStat.MaxHp;
    }
    public virtual void Init()
    {
        Managers.Clear(); // �����ߴ��� nulló��
        _ui = Instantiate(Resources.Load<GameObject>("Prefabs/UI_Prefab/UI"));
        _ui.name = "UI";
    }

    void InputUIHotKey(Define.UI uiType)
    {
        if (uiType == Define.UI.Inventory)
        {

            if (inventory.activeSelf == true) // �κ��丮�� �����ִٸ�
            {
                if (GameObject.FindGameObjectWithTag("NPC")) // �ش� �ʿ� ���� NPC�� �ִٸ�
                {
                    if (npcUI != null && !npcUI.activeSelf) // NPC ��üũ ������ ���������� �� �κ��丮 ��Ȱ��ȭ
                    {

                        inventory.SetActive(false);
                    }
                }
                else
                {
                    inventory.SetActive(false);
                }

            }
            else if (inventory.activeSelf == false)
            {

                inventory.SetActive(true);

            }
        }
    }

    protected virtual void ClickNPC(Define.MouseState evt) { }
    public virtual void SetPlayerHp() { }
}
