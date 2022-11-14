using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon1Scene : BaseScene
{
 
    public override void Init()
    {
        base.Init();

        _playerPos = new Vector3(-7, 1.4f, 31);
        _player = Managers.game.SpawnPlayer(_playerPos);

        Camera.main.gameObject.GetComponent<CameraController>().SetPlayer(_player);
    }

}
