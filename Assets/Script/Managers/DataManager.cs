using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}




public class DataManager
{
  
    public Dictionary<int, Contents.Stat> StatDict = new Dictionary<int, Contents.Stat>();
    public Dictionary<int, Contents.Item> InvenDict = new Dictionary<int, Contents.Item>();
    public Hashtable hashtable = new Hashtable();
    Dictionary<int, Item> _inventoryDict;

    public Dictionary<int, Item> Inventory
    {
        get
        {
            if (_inventoryDict == null)
                _inventoryDict = new Dictionary<int, Item>();
            return _inventoryDict;
        }
    } //인벤토리 프로퍼티

    public int InventoryCount
    {
        get
        {
            if (_inventoryDict == null)
                _inventoryDict = new Dictionary<int, Item>();
            return _inventoryDict.Count;
        }
    }

    public PlayerStat PlayerStat { get { return Managers.Game._Player.gameObject.GetComponent<PlayerStat>(); } }

    int _gold;
    public int Gold { get { return _gold; } set { _gold = value; } }

    private void Awake()
    {
        _gold = 0;
        _inventoryDict = new Dictionary<int, Item>();
    }
    public void Init()
    {
        StatDict = LoadJson<Contents.StatData, int, Contents.Stat>("StatData").MakeDict();
        InvenDict = LoadJson<Contents.InventoryData, int, Contents.Item>("InventoryData").MakeDict();

    }

    //public void InventoryDataChange(int idx, Item item, bool Input = true) // 기본적으로 아이템을 넣는 bool값 
    //{

    //    Contents.Item tempItem = new Contents.Item();
    //    if(!InvenDict.ContainsKey(idx))
    //    {
    //        tempItem.Id = item.Id;
    //        tempItem.ItemType = item.ItemType;
    //        tempItem.Name = item.Name;
    //        InvenDict.Add(idx, tempItem);
    //    }
    //    if (!_inventoryDict.ContainsKey(idx) && Input)
    //    {
    //        _inventoryDict.Add(idx, item); // 인벤토리에 아이템 추가
    //    }
    //    else
    //    {
    //        _inventoryDict.Remove(idx);
    //    }

    //  //  InvenDict = WriteToJson<Dictionary<int, Contents.Item>>(InvenDict, "InventoryData");
    //}


    public void InventoryDataChange(int idx, Contents.Item item, bool Input = true) // 기본적으로 아이템을 넣는 bool값 
    {

        if (InvenDict.Count >= 15)
            return;
        if (InvenDict.ContainsKey(item.Id))
            return;
        InvenDict.Add(item.Id, item);
        //if (!InvenDict.ContainsKey(idx))
        //{
        //    tempItem.Id = item.Id;
        //    tempItem.ItemType = item.ItemType;
        //    tempItem.Name = item.Name;
        //    InvenDict.Add(idx, tempItem);
        //}
        //if (!_inventoryDict.ContainsKey(idx) && Input)
        //{
        //    _inventoryDict.Add(idx, item); // 인벤토리에 아이템 추가
        //}
        //else
        //{
        //    _inventoryDict.Remove(idx);
        //}

        InvenDict = WriteToJson<Dictionary<int, Contents.Item>>(InvenDict, "InventoryData");
    }



    T LoadJson<T, Key, Value>(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>($"Data/{path}");
        return JsonConvert.DeserializeObject<T>(textAsset.text);
    }

    T WriteToJson<T>(Dictionary<int, Contents.Item> dict,string path)
    {
        List<Contents.Item> items = dict.Values.ToList();
        string json = JsonConvert.SerializeObject(new { items }, Formatting.Indented);
        string filePath = Path.Combine(Application.dataPath, $"Resources/Data/{path}.json");
        File.WriteAllText(filePath, json);

        //TextAsset textAsset = Resources.Load<TextAsset>($"Data/{path}");
        //return JsonConvert.DeserializeObject<T>(textAsset.text);
        try
        {
            TextAsset textAsset = Resources.Load<TextAsset>($"Data/{path}");
            return JsonConvert.DeserializeObject<T>(textAsset.text);
        }
        catch (System.FormatException e)
        {
            Debug.LogError("Error deserializing JSON data: " + e.Message);
            return default(T);
        }
    }

}
