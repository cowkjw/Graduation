using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownScene : BaseScene
{

    public override void Init()
    {
        base.Init();

        _playerPos = new Vector3(0, -0.7f, 0);
        _player = Managers.game.SpawnPlayer(_playerPos);

        Camera.main.gameObject.GetComponent<CameraController>().SetPlayer(_player);
    }


}
