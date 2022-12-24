using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{

    Dictionary<int, string> _inventoryDict = new Dictionary<int, string>();

    public Dictionary<int,string> Inventory { get { return _inventoryDict; } } //�κ��丮 ������Ƽ

    public int InventoryCount { get { return _inventoryDict.Count; } }

    public PlayerStat PlayerStat { get { return Managers.game._Player.gameObject.GetComponent<PlayerStat>(); } }

    int _gold = 0;
    public int Gold { get { return _gold; } set { _gold = value; } }

    public void InventoryDataChange(int idx, string itemName, bool Input=true) // �⺻������ �������� �ִ� bool�� 
    {
        if (!_inventoryDict.ContainsKey(idx)&&Input)
        {
            _inventoryDict.Add(idx, itemName); // �κ��丮�� ������ �߰�
        }
        else
        {
            _inventoryDict.Remove(idx);
        }
    }
}