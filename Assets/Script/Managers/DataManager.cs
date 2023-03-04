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

    public Dictionary<int, Contents.Item> ItemDict = new Dictionary<int, Contents.Item>();
    public Dictionary<int, Contents.Stat> StatDict = new Dictionary<int, Contents.Stat>();
    public Dictionary<int, Contents.Item> InvenDict = new Dictionary<int, Contents.Item>();
    public Dictionary<string, VectorConverter> enemyDict = new Dictionary<string, VectorConverter>();
    public Dictionary<string, Contents.ExpData> enemyExpDict = new Dictionary<string, Contents.ExpData>();


    public PlayerStat PlayerStat { get { return Managers.Game.Player.gameObject.GetComponent<PlayerStat>(); } }

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
        ItemDict = LoadJson<Contents.ItemData, int, Contents.Item>("ItemData").MakeDict();
        enemyDict = LoadJson<Contents.EnemyData, string, VectorConverter>("EnemyData").MakeDict();
        enemyExpDict = LoadJson<Contents.EnemyExpData, string, Contents.ExpData>("EnemyExp").MakeDict();
    }

    public void InventoryDataChange(int idx, Contents.Item item = null, bool add = true) // 기본적으로 아이템을 넣는 bool값 
    {
        if (add)
        {
            InvenDict.TryAdd(idx, item); 
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
