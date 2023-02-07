using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Slot : MonoBehaviour
{

    public Contents.Item _itemInfo;
   // public Item itemInShop;


    [SerializeField]
    // public ItemInfo _itemInfo;
    public bool inItem = false;




    private void Start()
    {
        _itemInfo = new Contents.Item();
        //_itemInfo.Name = GetComponent<Image>().sprite.name;
        _itemInfo.ItemType = Define.ItemType.Equipment;
        _itemInfo.Id = -1;

        if (!inItem)
        {
            _itemInfo.Name = "emptySlot";
            this.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Etc/{_itemInfo.Name}");
        }
        else
        {
            _itemInfo.Name = GetComponent<Image>().sprite.name;
            this.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Items/{_itemInfo.Name}");
        }

        //if (this.gameObject.layer == 15)
        //{
        //    itemInShop = new Item(_itemInfo.Name, Define.ItemType.Equipment);
        //}
    }
}
