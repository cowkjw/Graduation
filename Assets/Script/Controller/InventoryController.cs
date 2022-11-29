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
        for (int idx = 0; idx < Managers.Data.InventoryCount; idx++) // �������� DataManager�� �ִ� ���ڸ�ŭ �κ��丮�� ä��
        {
            if(_inventory.ContainsKey(idx))
            {
                GameObject slot = Slots[idx].transform.GetChild(0).gameObject; // ���� ������
                slot.SetActive(true); // Ȱ��ȭ
                slot.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Items/{_inventory[idx]}"); // �ش� ���Կ� �̹����� �ٲ�
            }
        }      
    }

    public void AchiveItem(string itemName)
    {
        _idx = Managers.Data.InventoryCount;
        if (_idx < 0)
            _idx = 0;
        if (_idx > 15) // �κ��丮 ����
            return;
        //  Slots[_idx].gameObject.GetComponent<Slot>().SetItemInfo(_idx, $"Item{_idx}"); // �ش� ���Կ� ������ ���� ����

        Managers.Data.InventoryDataChange(_idx, itemName); // DataManager�� �ִ� �κ��丮�� Data ����
        GameObject slot = Slots[_idx].transform.GetChild(0).gameObject;

        slot.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Items/{itemName}");
        slot.SetActive(true); // �̹��� Ȱ��ȭ
        _idx++;

    }
}
