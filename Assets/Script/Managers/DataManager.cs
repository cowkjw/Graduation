using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{

    Dictionary<int, string> _inventoryDict = new Dictionary<int, string>();

    public Dictionary<int,string> Inventory { get { return _inventoryDict; } }

    public int InventoryCount { get { return _inventoryDict.Count; } }

    int _gold = 0;
    public int Gold { get { return _gold; } set { _gold = value; } }

    public void InventoryDataChange(int idx, string itemName, bool Input=true) // 기본적으로 아이템을 넣는 bool값 
    {
        if (!_inventoryDict.ContainsKey(idx)&&Input)
        {
            _inventoryDict.Add(idx, itemName); // 인벤토리에 아이템 추가
        }
        else
        {
            _inventoryDict.Remove(idx);
        }
    }
}
