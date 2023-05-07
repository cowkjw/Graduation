using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Slot : MonoBehaviour
{

    Contents.Item _itemInfo = new Contents.Item();
    public bool inItem = false;
    public Contents.Item ItemInfo { get { return _itemInfo; } private set { _itemInfo = value; } }
    Image itemImage;
    void Start()
    {
        Init();
        itemImage.sprite = inItem ? Resources.Load<Sprite>($"Items/{_itemInfo.Id}") : Resources.Load<Sprite>("Items/emptySlot");
    }

    void Init()
    {
        itemImage = GetComponent<Image>();
        if (Managers.Data.ItemDict.TryGetValue(itemImage.sprite.name != "emptySlot" ?
            int.Parse(itemImage.sprite.name) : -1, out Contents.Item tempItem))
        {
            _itemInfo.ItemType = tempItem.ItemType;
            _itemInfo.Name = tempItem.Name;
            _itemInfo.Price = tempItem.Price;
            _itemInfo.Id = tempItem.Id;
            _itemInfo.SellPrice = tempItem.SellPrice;

            if (_itemInfo.ItemType == Define.ItemType.Equipment)
            {
                _itemInfo.Attack = tempItem.Attack;
            }

            inItem = true;
        }
        else
        {
            inItem = false;
        }
    }

    public void PutInItem(Contents.Item item)
    {
        ItemInfo = item;
        inItem = true;
        itemImage.sprite = Resources.Load<Sprite>($"Items/{ItemInfo.Id}");
    }
}
