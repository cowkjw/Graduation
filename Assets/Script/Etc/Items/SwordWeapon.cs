using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordWeapon : Item
{

    public SwordWeapon(string name, Define.ItemType itemType) : base(name, itemType) { }


    public override void UseItem()
    {
        base.UseItem();
        Debug.Log("°Ë ¹«±â");
    }
}
