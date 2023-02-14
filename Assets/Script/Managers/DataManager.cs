using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

public interface ILoader<Key, Value>{}

public class DataManager
{

    public Dictionary<int, Contents.Stat> StatDict = new Dictionary<int, Contents.Stat>();
    public Dictionary<int, Contents.Item> InvenDict = new Dictionary<int, Contents.Item>();


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
        InvenDict = LoadJson<Contents.InventoryData, int, Contents.Stat>("InventoryData").MakeDict();

    }

    public void InventoryDataChange(int idx, Contents.Item item = null, bool add = true) // 기본적으로 아이템을 넣는 bool값 
    {
        if (add)
        {
            if (!InvenDict.ContainsKey(idx))
                InvenDict.Add(idx, item);
        }
        else
        {
            InvenDict.Remove(idx);
        }

        WriteToJson(InvenDict, "InventoryData");

    }

    T LoadJson<T, Key, Value>(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>($"Data/{path}");
        return JsonConvert.DeserializeObject<T>(textAsset.text);
    }

    void WriteToJson<T>(T list, string path)
    {
        List<T> items = new List<T>();
        items.Add(list);
        string json = JsonConvert.SerializeObject(new { items }, Formatting.Indented);
        string filePath = Path.Combine(Application.dataPath, $"Resources/Data/{path}.json");
        File.WriteAllText(filePath, json);
    }


}
