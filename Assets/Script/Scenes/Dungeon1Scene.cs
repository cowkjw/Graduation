using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dungeon1Scene : BaseScene  // @Scene�� Add
{


    public List<GameObject> enemy;

    Slider _hpBar;
    Stat _objStat;
    Text _hpValue;
    Text _objNameText;
    string _objName;

    public Slider HpBar { get { return _hpBar; } }
    public Stat ObjStat { set { _objStat = value; } }
    public string ObjName { set { _objName= value; } }
    public Text ObjNameText { get { return _objNameText; } }
    public override void Init()
    {
        base.Init();

        Managers.Pool.Init(); // PoolManager �ʱ�ȭ

         _playerPos = new Vector3(-7, 1.4f, 31);
        _player = Managers.Game.SpawnPlayer(_playerPos);

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
        _hpValue.text = _objStat.Hp.ToString() + "/" + _objStat.MaxHp.ToString();
    }
   void SetHpRatio()
    {
        _hpBar.value = _objStat.Hp / (float)_objStat.MaxHp;
    }

    void SetObjNamePrint()
    {
        _objNameText.text = _objName;
    }

   

}
