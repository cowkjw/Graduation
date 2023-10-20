using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;

namespace Contents
{
    [Serializable]
    public class Item
    {
        public string Name { get; set; } = "emptySlot";
        public Define.ItemType ItemType;
        public int Attack = 0;
        public int Id { get; set; } = -1; // 아이템 아이디
        public int Price = 0;
        public int SellPrice = 0;
    }

    [Serializable]
    public class Stat
    {
        public int level;
        public int maxHp;
        public int maxMp;
        public int attack;
        public int defense;
        public int totalExp;
    }

    [Serializable]
    public class Player
    {
        public Stat playerStat;
        public int gold;
        public int location;
        public int equippedWeapon;
    }

    [Serializable]
    public class Enemy
    {
        public float x;
        public float y;
        public float z;
    }

    [Serializable]
    public class ExpData
    {
        public int Exp;
    }


    [Serializable]
    public class StatData : ILoader<int, Stat>
    {
        public List<Stat> stats = new List<Stat>();

        public Dictionary<int, Stat> MakeDict()
        {
            Dictionary<int, Stat> dict = new Dictionary<int, Stat>();
            foreach (Stat stat in stats)
                dict.Add(stat.level, stat);
            return dict;
        }
    }

    [Serializable]
    public class InventoryData : ILoader<int, Item>
    {
        public List<Dictionary<int, Item>> items = new List<Dictionary<int, Item>>();
        public Dictionary<int, Item> MakeDict()
        {
            return items[0];
        }
    }

    [Serializable]
    public class ItemData : ILoader<int, Item>
    {
        public List<Dictionary<int, Item>> data = new List<Dictionary<int, Item>>();
        public Dictionary<int, Item> MakeDict()
        {
            return data[0];
        }
    }

    [Serializable]
    public class EnemyData : ILoader<string, VectorConverter>
    {
        public List<Dictionary<string, VectorConverter>> enemies = new List<Dictionary<string, VectorConverter>>();
        public Dictionary<string, VectorConverter> MakeDict()
        {
            return enemies[0];
        }
    }

    [Serializable]
    public class EnemyExpData : ILoader<int, ExpData>
    {
        public List<Dictionary<string, ExpData>> EnemyExp = new List<Dictionary<string, ExpData>>();

        public Dictionary<string, ExpData> MakeList()
        {
            return EnemyExp[0];
        }

    }
}
