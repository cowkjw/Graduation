using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossDungeonScene : DungeonScene
{
    public override void Init()
    {
        base.Init();
        PlayerPos = new Vector3(2.5f, -4.1f,-27f); // 던전은 고정
        Player.transform.position = PlayerPos;
        Camera.main.gameObject.GetComponent<CameraController>().SetPlayer(Player);
    }
}
