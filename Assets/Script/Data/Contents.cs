using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;

namespace Contents
{


    [Serializable]
    public class Item
    {
        public string Name;
        public Define.ItemType ItemType;
        public int Id; // 아이템 아이디
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

}
