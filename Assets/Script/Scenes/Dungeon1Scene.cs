using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dungeon1Scene : BaseScene  // @Scene¿¡ Add
{


    Slider hpBar;
    Stat _stat;
    Text hpValue;

    public override void Init()
    {
        base.Init();

        _playerPos = new Vector3(-7, 1.4f, 31);
        _player = Managers.game.SpawnPlayer(_playerPos);

        Camera.main.gameObject.GetComponent<CameraController>().SetPlayer(_player);

        GameObject uiObj = Instantiate(Resources.Load<GameObject>("Prefabs/UI_Prefab/UI_HP"));
        hpBar = uiObj.transform.GetChild(0).GetComponent<Slider>();
        hpValue = uiObj.transform.GetChild(0).transform.GetChild(3).GetComponent<Text>();

    }

    private void Update()
    {
        if (_stat != null)
        {
            SetHpRatio();
            SetHpPrint();
        }
    }

    public Slider GetHpBar()
    {
        return hpBar;
    }
    public void SetStat(Stat stat)
    {
        this._stat = stat;
    }
    void SetHpPrint()
    {
        hpValue.text = _stat.Hp.ToString() + "/" + _stat.MaxHp.ToString();
    }
    void SetHpRatio()
    {
        hpBar.value = _stat.Hp / (float)_stat.MaxHp;
    }
}
