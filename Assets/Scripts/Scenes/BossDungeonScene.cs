using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossDungeonScene : DungeonScene
{

    public override void Init()
    {
        base.Init();

        playerPos = new Vector3(2.5f, -4.1f,-27f); // 던전은 고정
        _player.transform.position = playerPos;
        Camera.main.gameObject.GetComponent<CameraController>().SetPlayer(_player);

        //GameObject Ui = GameObject.Find("UI");
        //_hpBar = Ui.transform.GetChild(1).GetComponent<Slider>();
        //_hpValue = _hpBar.transform.GetChild(3).GetComponent<Text>();
        //_objNameText = _hpBar.transform.GetChild(4).GetComponent<Text>();
    }
}
