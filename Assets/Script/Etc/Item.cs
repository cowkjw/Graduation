using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Item : MonoBehaviour
{ 
    public string Name;
    public Define.ItemType ItemType;
    public int Id; // ������ ���̵�

    public virtual void UseItem()
    {
        Debug.Log("������ ���");
    }

    void Start()
    {
        Name = gameObject.name;
        SetItemID();
    }

    public void SetItemID()
    {
        MatchCollection matches = Regex.Matches(Name, @"\d+");
        string result = "";
        foreach (Match match in matches)
        {
            result += match.Value;
        }
        if (!int.TryParse(result, out int parsedId))
        {
            Debug.LogError("Could not parse item id from name: " + Name);
            return;
        }

        Id = parsedId;
    }

    public Item(string Name, Define.ItemType ItemType)
    {
        this.Name = Name;
        this.ItemType = ItemType;

    }

}
