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
        for (int idx = 0; idx < Managers.Data.InventoryCount; idx++) // �������� DataManager�� �ִ� ���ڸ�ŭ �κ��丮�� ä��
        {
            if(_inventory.ContainsKey(idx))
            {
               
                GameObject slot = Slots[idx].transform.gameObject;// ���� ������
                slot.SetActive(true); // Ȱ��ȭ
                slot.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Items/{_inventory[idx]}"); // �ش� ���Կ� �̹����� �ٲ�
                slot.GetComponent<Slot>()._itemInfo.itemName = _inventory[idx]; // ������ �̸� ����
                slot.GetComponent<Slot>().inItem = true;
            }
        }
      
    }

    private void Update()
    {
        _goldText.text = Managers.Data.Gold.ToString(); // ���� ����
    }

    public void AchiveItem(string itemName) // �������� �Ծ�����
    {
        _idx = Managers.Data.InventoryCount;
        if (_idx < 0)
            _idx = 0;
        if (_idx > 15) // �κ��丮 ����
            return;
       

        Managers.Data.InventoryDataChange(_idx, itemName); // DataManager�� �ִ� �κ��丮�� Data ����
                                                           //GameObject slot = Slots[_idx].transform.GetChild(0).gameObject; //������ �ڽ�(������ �̹���)

        GameObject slot = Slots[_idx].transform.gameObject;// ���� ������
        slot.SetActive(true); // Ȱ��ȭ
        slot.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Items/{itemName}"); // �ش� ���Կ� �̹����� �ٲ�
        slot.GetComponent<Slot>()._itemInfo.itemName = itemName; // ������ ������ �̸� ����
        slot.GetComponent<Slot>().inItem = true; // �������� ����
        _idx++;

    }


}
