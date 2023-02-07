using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Contents
{



    [Serializable]
    public class Item
    {
        public string Name = "emptySlot"; 
        public Define.ItemType ItemType { get; set; }
        public int Id{ get; set; } // 아이템 아이디
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

        public List<Item> items = new List<Item>();

        public Dictionary<int, Item> MakeDict()
        {
            Dictionary<int, Item> dict = new Dictionary<int, Item>();
            int idx = 0;
            foreach (Item item in items)
            {
                dict.Add(idx++, item);
            }
            return dict;
        }

    }

 

   
}
