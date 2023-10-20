using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Item : MonoBehaviour
{
    public string Name { get => _itemName; }
    public Define.ItemType ItemType { get => _itemType; }
    public int Id { get => _id; }
    public int Price { get => _price; }
    public int SellPrice { get => _sellPrice; }

    string _itemName;
    Define.ItemType _itemType;
    int _id; // 아이템 아이디
    int _price;
    int _sellPrice;

    public virtual void UseItem()
    {
#if UNITY_EDITOR
        Debug.Log("아이템 사용");
#endif
    }

    protected virtual void Start()
    {
        Init();
    }

    void Init()
    {
        if (Managers.Data.ItemDict.TryGetValue(int.Parse(gameObject.name), out Contents.Item tempItem))
        {
            _itemName = tempItem.Name;
            _itemType = tempItem.ItemType;
            _id = tempItem.Id;
            _price = tempItem.Price;
            _sellPrice = tempItem.SellPrice;
        }
    }
}
