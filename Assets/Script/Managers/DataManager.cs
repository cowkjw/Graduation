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
    public List<Contents.Item> InvenList    = new List<Contents.Item>();

    public PlayerStat PlayerStat { get { return Managers.Game._Player.gameObject.GetComponent<PlayerStat>(); } }

    int _gold;
    public int Gold { get { return _gold; } set { _gold = value; } }

    private void Awake()
    {
        _gold = 0;
    }
    public void Init()
    {
        StatDict = LoadJson<Contents.StatData, int, Contents.Stat>("StatData").MakeDict();
        InvenList = LoadInventory<List<Contents.Item>>("InventoryData");
    }

    public void InventoryDataChange(int idx, Contents.Item item, bool add = true) // 기본적으로 아이템을 넣는 bool값 
    {

        //if (InvenList.Count >= 15)
        //    return;
        if (add)
        {
            InvenList.Add(item);
        }
        else
        {
            InvenList[idx] = item;
        }

        WriteToJson(InvenList, "InventoryData");
    }



    T LoadJson<T, Key, Value>(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>($"Data/{path}");

        return JsonConvert.DeserializeObject<T>(textAsset.text);
    }

    T LoadInventory<T>(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>($"Data/{path}");
        return JsonConvert.DeserializeObject<T>(textAsset.text);
    }
    void WriteToJson<T>(T list, string path)
    {
        T items = list;
        string json = JsonConvert.SerializeObject(list, Formatting.Indented);
        string filePath = Path.Combine(Application.dataPath, $"Resources/Data/{path}.json");
        File.WriteAllText(filePath, json);
    }
}
