using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public GameObject[] Slots;
    int _idx;
    Dictionary<int, string> _inventory = Managers.Data.Inventory;


    void Awake()
    {
        for (int idx = 0; idx < Managers.Data.InventoryCount; idx++) // 시작전에 DataManager에 있는 숫자만큼 인벤토리를 채움
        {
            if(_inventory.ContainsKey(idx))
            {
                GameObject slot = Slots[idx].transform.GetChild(0).gameObject; // 슬롯 가져옴
                slot.SetActive(true); // 활성화
                slot.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Items/{_inventory[idx]}"); // 해당 슬롯에 이미지를 바꿈
            }
        }      
    }

    public void AchiveItem(string itemName)
    {
        _idx = Managers.Data.InventoryCount;
        if (_idx < 0)
            _idx = 0;
        if (_idx > 15) // 인벤토리 꽉참
            return;
        //  Slots[_idx].gameObject.GetComponent<Slot>().SetItemInfo(_idx, $"Item{_idx}"); // 해당 슬롯에 아이템 정보 전달

        Managers.Data.InventoryDataChange(_idx, itemName); // DataManager에 있는 인벤토리에 Data 변경
        GameObject slot = Slots[_idx].transform.GetChild(0).gameObject;

        slot.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Items/{itemName}");
        slot.SetActive(true); // 이미지 활성화
        _idx++;

    }
}
