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

public interface IReader<Key, Value>  // 수정하기
{
    Dictionary<Key, Value> WriteDict();
}

public class DataManager
{
    Dictionary<int, Contents.Item> temp = new Dictionary<int, Contents.Item>();
    List<Contents.Item> items = new List<Contents.Item>();
    public Dictionary<int, Contents.Stat> StatDict = new Dictionary<int, Contents.Stat>();
    Dictionary<int, string> _inventoryDict;
    public Dictionary<int, Contents.Item> tempDict = new Dictionary<int, Contents.Item>();

    public Dictionary<int, string> Inventory
    {
        get
        {
            if (_inventoryDict == null)
                _inventoryDict = new Dictionary<int, string>();
            return _inventoryDict;
        }
    } //인벤토리 프로퍼티

    public int InventoryCount
    {
        get
        {
            if (_inventoryDict == null)
                _inventoryDict = new Dictionary<int, string>();
            return _inventoryDict.Count;
        }
    }

    public PlayerStat PlayerStat { get { return Managers.Game._Player.gameObject.GetComponent<PlayerStat>(); } }

    int _gold;
    public int Gold { get { return _gold; } set { _gold = value; } }

    private void Awake()
    {
        _gold = 0;
        _inventoryDict = new Dictionary<int, string>();
    }
    public void Init()
    {
        StatDict = LoadJson<Contents.StatData, int, Contents.Stat>("StatData").MakeDict();
        //tempDict = LoadJson<Contents.InventoryData, int, Contents.Item>("InventoryData").MakeDict();
    }

    public void InventoryDataChange(int idx, string itemName, bool Input = true) // 기본적으로 아이템을 넣는 bool값 
    {
        if (!_inventoryDict.ContainsKey(idx) && Input)
        {
            _inventoryDict.Add(idx, itemName); // 인벤토리에 아이템 추가
        }
        else
        {
            _inventoryDict.Remove(idx);
        }
        Dictionary<int, Contents.Item> a = new Dictionary<int, Contents.Item>();
        //Contents.Item b = new Contents.Item();
        //Contents.Item c = new Contents.Item();
        //b.Id = 4;
        //b.Name = "1";
        //b.ItemType = Define.ItemType.Equipment;
        //c.Name = "2";
        //c.Id = 5;
        //c.ItemType = Define.ItemType.Equipment;
        //a.Add(0, b);
        //a.Add(1, c);

        StatDict = LoadJson<Contents.StatData, int, Contents.Stat>("StatData").MakeDict();
      //  temp =  WriteJson<Contents.InventoryData,int,Contents.Item>("InventoryData").Wri;
    }



    T LoadJson<T, Key, Value>(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>($"Data/{path}");
        return JsonConvert.DeserializeObject<T>(textAsset.text);
    }

    //T WriteJson<T,Key,Value>(Dictionary<int,Item> dict, string path)
    //{
    //    List<Item> items = new List<Item>();


    //    foreach (var data in dict.Values)
    //    {
    //        items.Add(data);
    //    }


    //    File.WriteAllText(Path.Combine(Application.dataPath + $"/Resources/Data/{path}.json"), JsonConvert.SerializeObject(items, Formatting.Indented));
    //    //Newtonsoft.Json.

    //    TextAsset textAsset = Resources.Load<TextAsset>($"Data/{path}");
    //    return JsonUtility.FromJson<T>(textAsset.text);

    //}

    public T WriteJson<T>(Dictionary<int, Item> dict, string path) where T : class
    {
        List<Item> items = dict.Values.ToList();
        string json = JsonConvert.SerializeObject(items, Formatting.Indented);
        string filePath = Path.Combine(Application.dataPath, $"Resources/Data/{path}.json");
        File.WriteAllText(filePath, json);

        TextAsset textAsset = Resources.Load<TextAsset>($"Data/{path}");
        return JsonConvert.DeserializeObject<T>(textAsset.text);
    }

    //public void WriteJson(Dictionary<int, Contents.Item> dict, string path)
    //{
    //    List<Contents.Item> items = dict.Values.ToList();
    //    string json = JsonConvert.SerializeObject(items, Formatting.Indented);
    //    string filePath = Path.Combine(Application.dataPath, $"Resources/Data/{path}.json");
    //    File.WriteAllText(filePath, json);
    //}

}
