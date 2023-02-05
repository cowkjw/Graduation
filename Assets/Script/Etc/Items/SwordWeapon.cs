using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordWeapon : Item
{

    public override void UseItem()
    {
        base.UseItem();
        Debug.Log("°Ë ¹«±â");
    }
}
