using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseScene : MonoBehaviour
{

    protected Vector3 _playerPos;
    protected GameObject _player = null;
    protected GameObject _ui = null;

    protected Slider _playerHpBar;
    protected Stat _playerStat;


    void Awake()
    {
        Init();
    }

    private void Start()
    {
        _playerStat = Managers.game._Player.GetComponent<PlayerStat>();
        _playerHpBar = GameObject.FindGameObjectWithTag("PlayerUI").transform.GetChild(1).GetComponent<Slider>();
        Managers.Input.KeyboardAction -= InputUIHotKey;
        Managers.Input.KeyboardAction += InputUIHotKey;
    }
    protected virtual void Update() // 플레이어의 HPUI 업데이트를 위함
    {
        _playerHpBar.value = _playerStat.Hp / (float)_playerStat.MaxHp;
    }
    public virtual void Init()
    {
        Managers.Clear(); // 구독했던거 null처리
        _ui = Instantiate(Resources.Load<GameObject>("Prefabs/UI_Prefab/UI"));
        _ui.name = "UI";
    }

    void InputUIHotKey(Define.UI uiType)
    {
        if (uiType == Define.UI.Inventory)
        {
            GameObject inventory = GameObject.Find("UI").transform.Find("Inventory").gameObject;
            if (inventory.activeSelf == true)
            {
                inventory.SetActive(false);

            }
            else if (inventory.activeSelf == false)
            {
                inventory.SetActive(true);

            }
        }
    }
    public virtual void SetPlayerHp() { }
}
