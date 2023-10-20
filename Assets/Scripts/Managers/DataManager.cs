using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

public interface ILoader<Key, Value> {}

public class DataManager
{
    public Dictionary<int, Contents.Item> ItemDict = new Dictionary<int, Contents.Item>();
    public Dictionary<int, Contents.Stat> StatDict = new Dictionary<int, Contents.Stat>();
    public Dictionary<int, Contents.Item> InvenDict = new Dictionary<int, Contents.Item>();
    public Dictionary<string, VectorConverter> EnemyDict = new Dictionary<string, VectorConverter>();
    public Dictionary<string, Contents.ExpData> EnemyExpDict = new Dictionary<string, Contents.ExpData>();
    public PlayerStat PlayerStat { get { return Managers.Game.GetPlayer().gameObject.GetComponent<PlayerStat>(); } }
    public Contents.Player PlayerData = new Contents.Player();
    public int Gold { get { return _gold; } set { _gold = value; } }

    int _gold;

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

        EnemyExpDict = LoadJson<Contents.EnemyExpData, string, Contents.ExpData>("EnemyExp").MakeList();
        PlayerData = LoadJson<Contents.Player>("PlayerData");
        _gold = PlayerData.gold;
    }

    public void UpdateInventoryData(int idx, Contents.Item item = null, bool add = true) // �⺻������ �������� �ִ� bool�� 
    {
        if (add)
        {
            if (!InvenDict.TryAdd(idx, item)) // ���� �̹� �ش� ĭ�� �� �ִٸ�
            {
                InvenDict[idx] = item; // �������� ������
            }
        }
        else
        {
            InvenDict.Remove(idx);
        }
        WriteToJson(InvenDict, "InventoryData",true);
        PlayerDataChange();
    }

    public void PlayerDataChange()
    {
        PlayerData.playerStat.level = PlayerStat.Level;
        PlayerData.playerStat.totalExp = PlayerStat.TotalExp;
        PlayerData.gold = _gold;
#if UNITY_EDITOR
        Debug.Log("������ ������");
#endif
        WriteToJson(PlayerData, "PlayerData");
    }


    T LoadJson<T, Key, Value>(string path) // Key,Value
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

    void WriteToJson<T>(T data, string path, bool wrapInItemsObject = false)
    {
        string json;
        List<T> items = new List<T>();
        items.Add(data);
        if (wrapInItemsObject)
        {
            json = JsonConvert.SerializeObject(new { items }, Formatting.Indented);
        }
        else
        {
            json = JsonConvert.SerializeObject(data, Formatting.Indented);
        }
        string filePath = Path.Combine(Application.dataPath, $"StreamingAssets/Data/{path}.json");
        if (!File.Exists(filePath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        }
        File.WriteAllText(filePath, json);
    }
}
