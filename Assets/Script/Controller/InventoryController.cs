using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public GameObject[] Slots;
    int _idx;
    Dictionary<int, string> _inventory = Managers.Data.Inventory;
    public Text toolTip;
    Text _goldText;
   
    void Awake()
    {
        _goldText = transform.GetChild(1).GetChild(0).GetComponent<Text>();
        for (int idx = 0; idx < Managers.Data.InventoryCount; idx++) // 시작전에 DataManager에 있는 숫자만큼 인벤토리를 채움
        {
            if(_inventory.ContainsKey(idx))
            {
               
                GameObject slot = Slots[idx].transform.gameObject;// 슬롯 가져옴
                slot.SetActive(true); // 활성화
                slot.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Items/{_inventory[idx]}"); // 해당 슬롯에 이미지를 바꿈
                slot.GetComponent<Slot>()._itemInfo.itemName = _inventory[idx]; // 아이템 이름 설정
                slot.GetComponent<Slot>().inItem = true;
            }
        }
      
    }

    private void Update()
    {
        _goldText.text = Managers.Data.Gold.ToString(); // 수정 예정
    }

    public void AchiveItem(string itemName) // 아이템을 먹었을때
    {
        _idx = Managers.Data.InventoryCount;
        if (_idx < 0)
            _idx = 0;
        if (_idx > 15) // 인벤토리 꽉참
            return;
       

        Managers.Data.InventoryDataChange(_idx, itemName); // DataManager에 있는 인벤토리에 Data 변경
                                                           //GameObject slot = Slots[_idx].transform.GetChild(0).gameObject; //슬롯의 자식(아이템 이미지)

        GameObject slot = Slots[_idx].transform.gameObject;// 슬롯 가져옴
        slot.SetActive(true); // 활성화
        slot.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Items/{itemName}"); // 해당 슬롯에 이미지를 바꿈
        slot.GetComponent<Slot>()._itemInfo.itemName = itemName; // 아이템 정보에 이름 전달
        slot.GetComponent<Slot>().inItem = true; // 아이템이 들어갔음
        _idx++;

    }


}
