using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Slot : MonoBehaviour
{

    [SerializeField]
    public ItemInfo _itemInfo;
    public bool inItem = false;
    public struct ItemInfo
    {
        public string itemName;
        public Define.ItemType itemType;
    }

    private void Start()
    {
        if(GetComponent<Image>().sprite.name!="emptySlot")
        {
            _itemInfo.itemName = GetComponent<Image>().sprite.name;
            _itemInfo.itemType = Define.ItemType.Equipment;
        }
    }
}
