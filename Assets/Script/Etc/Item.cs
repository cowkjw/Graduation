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

    public string Name { get { return itemName; } }
    public Define.ItemType ItemType { get { return itemType; } }
    public int Id { get { return id; } }
    public int Price { get { return price; } }

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
        }
    }

    //public void SetItemID()
    //{
    //    MatchCollection matches = Regex.Matches(Name, @"\d+");
    //    string result = "";
    //    foreach (Match match in matches)
    //    {
    //        result += match.Value;
    //    }
    //    if (!int.TryParse(result, out int parsedId))
    //    {
    //        Debug.LogError("Could not parse item id from name: " + Name);
    //        return;
    //    }

    //   // Id = parsedId;
    //}


}
