using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

public interface ILoader<Key, Value> { }

public class DataManager
{

    public Dictionary<int, Contents.Item> ItemDict = new Dictionary<int, Contents.Item>();
    public Dictionary<int, Contents.Stat> StatDict = new Dictionary<int, Contents.Stat>();
    public Dictionary<int, Contents.Item> InvenDict = new Dictionary<int, Contents.Item>();
    public Dictionary<string, VectorConverter> EnemyDict = new Dictionary<string, VectorConverter>();
    public Dictionary<string, Contents.ExpData> EnemyExpDict = new Dictionary<string, Contents.ExpData>();
    public Contents.Player PlayerData = new Contents.Player();




    public PlayerStat PlayerStat { get { return Managers.Game.GetPlayer().gameObject.GetComponent<PlayerStat>(); } }

    int _gold;
    public int Gold { get { return _gold; } set { _gold = value; } }

    private void Awake()
    {
        _gold = 0;
    }
    public void Init()
    {
        StatDict = LoadJson<Contents.StatData, int, Contents.Stat>("StatData").MakeDict();
        InvenDict = LoadJson<Contents.InventoryData, int, Contents.Item>("InventoryData").MakeDict();
        ItemDict = LoadJson<Contents.ItemData, int, Contents.Item>("ItemData").MakeDict();
        EnemyDict = LoadJson<Contents.EnemyData, string, VectorConverter>("EnemyData").MakeDict();
        EnemyExpDict = LoadJson<Contents.EnemyExpData, string, Contents.ExpData>("EnemyExp").MakeDict();
        PlayerData = LoadJson<Contents.Player>("PlayerData");
        _gold = PlayerData.gold;

    }

    public void UpdateInventoryData(int idx, Contents.Item item = null, bool add = true) // 기본적으로 아이템을 넣는 bool값 
    {
        if (add)
        {
            if (!InvenDict.TryAdd(idx, item)) // 만약 이미 해당 칸에 들어가 있다면
            {
                InvenDict[idx] = item; // 아이템을 변경함
            }

        }
        else
        {
            InvenDict.Remove(idx);
        }
        WriteToJson(InvenDict, "InventoryData");
    }

    public void PlayerDataChange()
    {

        PlayerData.playerStat.level = PlayerStat.Level;
        PlayerData.playerStat.totalExp = PlayerStat.TotalExp;
        PlayerData.gold = _gold;
        Debug.Log($"{PlayerData.equippedWeapon} 변경함");

        WriteToJson(PlayerData, "PlayerData");
    }


    T LoadJson<T, Key, Value>(string path)
    {
        string filePath = Path.Combine(Application.dataPath, $"StreamingAssets/Data/{path}.json");
        string json = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<T>(json);
    }



    T LoadJson<T>(string path)
    {

        string filePath = Path.Combine(Application.dataPath, $"StreamingAssets/Data/{path}.json");
        string json = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<T>(json);
    }
    void WriteToJson<T>(T list, string path)
    {
        //if(list is Dictionary<int,Contents.Item>)

        List<T> items = new List<T>();
        items.Add(list);
        string json = JsonConvert.SerializeObject(new { items }, Formatting.Indented);
        //string filePath = Path.Combine(Application.dataPath, $"Resources/Data/{path}.json");
        string filePath = Path.Combine(Application.dataPath, $"StreamingAssets/Data/{path}.json");
        if (!File.Exists(filePath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        }
        File.WriteAllText(filePath, json);
    }

    void WriteToJson(Contents.Player data, string path)
    {
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        //string filePath = Path.Combine(Application.dataPath, $"Resources/Data/{path}.json");
        string filePath = Path.Combine(Application.dataPath, $"StreamingAssets/Data/{path}.json");
        if (!File.Exists(filePath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        }
        File.WriteAllText(filePath, json);
    }

}
