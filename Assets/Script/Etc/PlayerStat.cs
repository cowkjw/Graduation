using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : Stat
{

    protected override void Init()
    {
        base.Init();
        Attack = 30;
        Hp = 500;
        MaxHp = 500;
    }
   
}
