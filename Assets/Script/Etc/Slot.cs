using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public struct ItemInfo
    {
        public string itemName;
        public Define.ItemType itemType;
    }


    Image _itemImage;
    public ItemInfo _itemInfo;

    
}
