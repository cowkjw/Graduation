using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonScene : BaseScene  // @Scene에 Add
{
    public Slider HpBar { get { return _hpBar; } }
    public Stat ObjStat { set { _objStat = value; } }
    public string ObjName { set { _objName = value; } }
    public Text ObjNameText { get { return _objNameText; } }

    Slider _hpBar;
    Stat _objStat;
    Text _hpValue;
    Text _objNameText;
    string _objName;

    public override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Dungeon;

        Managers.Pool.Init(); // PoolManager 초기화

        if (!(this is BossDungeonScene))
        { 
            GameObject spawningPool = new GameObject("SpawningPool", typeof(EnemySpawnController));
            spawningPool.transform.SetParent(transform);
        }

        PlayerPos = new Vector3(-7, 1.4f, 31); // 던전은 고정
        Player = Managers.Game.SpawnPlayer(PlayerPos);

        Camera.main.gameObject.GetComponent<CameraController>().SetPlayer(Player);

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
