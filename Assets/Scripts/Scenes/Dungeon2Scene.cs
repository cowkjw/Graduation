using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dungeon2Scene : BaseScene
{

    Slider _hpBar;
    Stat _objStat;
    Text _hpValue;
    Text _objNameText;
    string _objName;

    public Slider HpBar { get { return _hpBar; } }
    public Stat ObjStat { set { _objStat = value; } }
    public string ObjName { set { _objName = value; } }
    public Text ObjNameText { get { return _objNameText; } }
    public override void Init()
    {
        base.Init();

        playerPos = new Vector3(1, 1.4f,1); // ������ ����
        _player = Managers.Game.SpawnPlayer(playerPos);

        Camera.main.gameObject.GetComponent<CameraController>().SetPlayer(_player);

        GameObject Ui = GameObject.Find("UI");
        _hpBar = Ui.transform.GetChild(1).GetComponent<Slider>();
        _hpValue = _hpBar.transform.GetChild(3).GetComponent<Text>();
        _objNameText = _hpBar.transform.GetChild(4).GetComponent<Text>();
    }

    protected override void Update()
    {
        base.Update();
        if (_objStat != null)
        {
            SetHpRatio();
            SetHpPrint();
            SetObjNamePrint();
        }
        SetPlayerHp();
    }

    public Slider GetHpBar()
    {
        return _hpBar;
    }

    void SetHpPrint()
    {
        _hpValue.text = $"{_objStat.Hp}/{_objStat.MaxHp}";
    }
    void SetHpRatio()
    {
        _hpBar.value = (float)_objStat.Hp / _objStat.MaxHp;
    }

    void SetObjNamePrint()
    {
        _objNameText.text = _objName;
    }
}