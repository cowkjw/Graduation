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
    public class InventoryData : IReader<int, Item>
    {
        
        public List<Item> items = new List<Item>();

        public Dictionary<int, Item> WriteDict()
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

    //[Serializable]
    //public class ItemData //: IReader<int,Item> // 수정하기
    //{
    //    public List<Item> items = new List<Item>();

    //    void WriteDict()
    //    {
    //        Dictionary<int, Item> dict = new Dictionary<int, Item>();

    //        int idx = 0;

    //        foreach(Item item in items)
    //        {
    //            dict.Add(idx++, item);
    //        }
    //    }
    //}
}
