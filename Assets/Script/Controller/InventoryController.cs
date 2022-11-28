using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public GameObject[] Slots;
    int _idx;
    // public int Idx { get { return _idx; } private set { _idx = value; } }

    private List<int> _inventoryList = new List<int>();

    
    void Start()
    {
        _idx = 0;
    }


    
    void Update()
    {
        
    }

    public void AchiveItem()
    {
        if (_idx > 15) // 인벤토리 꽉참
            return;
        _inventoryList.Add(_idx);
        Slots[_idx++].transform.GetChild(0).gameObject.SetActive(true);
     
    }
}
