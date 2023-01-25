using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DataManager
{

    Dictionary<int, string> _inventoryDict;

    public Dictionary<int,string> Inventory { get {
            if (_inventoryDict == null)
                _inventoryDict = new Dictionary<int, string>();
            return _inventoryDict; } } //�κ��丮 ������Ƽ

    public int InventoryCount { get { 
            if(_inventoryDict==null)
                _inventoryDict = new Dictionary<int, string>();
            return _inventoryDict.Count; } }

    public PlayerStat PlayerStat { get { return Managers.Game._Player.gameObject.GetComponent<PlayerStat>(); } }

    int _gold;
    public int Gold { get { return _gold; } set { _gold = value; } }

    private void Awake()
    {
        _gold = 0;
        _inventoryDict = new Dictionary<int, string>();
    }


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
