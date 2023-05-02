using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Item : MonoBehaviour
{
    protected string itemName;
    protected Define.ItemType itemType;
    protected int id; // 아이템 아이디
    protected int price;
    protected int sellPrice;

    public string Name { get => itemName; }
    public Define.ItemType ItemType { get => itemType; }
    public int Id { get => id; }
    public int Price { get => price; }
    public int SellPrice { get => sellPrice; }

    public virtual void UseItem()
    {
        Debug.Log("아이템 사용");
    }

    protected virtual void Start()
    {
        Init();
    }

    void Init()
    {
        if (Managers.Data.ItemDict.TryGetValue(int.Parse(gameObject.name), out Contents.Item tempItem))
        {
            this.itemName = tempItem.Name;
            this.itemType = tempItem.ItemType;
            this.id = tempItem.Id;
            this.price = tempItem.Price;
            this.sellPrice = tempItem.SellPrice;
        }
    }
}
