using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class InventoryController : MonoBehaviour, IPointerDownHandler, IPointerExitHandler
{
    public GameObject[] Slots;
    int _idx;
    Dictionary<int, string> _inventory;
    // public Text toolTip;
    public ItemTooltip toolTip;
    Text _goldText;


    int sellSlotIdx;
    bool clickSlot;

    void OnEnable() // �տ��� �� �ʱ�ȭ�Ǹ� ( �̺�Ʈ ���� ���� )
    {
        clickSlot = false;

        _inventory = Managers.Data.Inventory;
        _goldText = transform.GetChild(1).GetChild(0).GetComponent<Text>();
        for (int idx = 0; idx < 15; idx++) // �������� DataManager�� �ִ� ���ڸ�ŭ �κ��丮�� ä��
        {
            if (_inventory.ContainsKey(idx))
            {

                GameObject slot = Slots[idx].transform.gameObject;// ���� ������
                slot.SetActive(true); // Ȱ��ȭ
                slot.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Items/{_inventory[idx]}"); // �ش� ���Կ� �̹����� �ٲ�
                slot.GetComponent<Slot>()._itemInfo.itemName = _inventory[idx]; // ������ �̸� ����
                slot.GetComponent<Slot>().inItem = true;
            }
        }


    }

    void Start()
    {
        Managers.Input.MouseAction -= Sell;
        Managers.Input.MouseAction += Sell;
    }

    private void Update()
    {
        _goldText.text = Managers.Data.Gold.ToString(); // ���� ����
    }

    public void AchiveItem(string itemName) // �������� �Ծ�����
    {
        _idx = Managers.Data.InventoryCount;
        for (int i = 0; i < 15; i++)
        {
            if (!Managers.Data.Inventory.ContainsKey(i))
            {
                _idx = i;
                break;
            }
        }
        //if (_idx < 0)
        //    _idx = 0;
        //if (_idx > 15) // �κ��丮 ����
        //    return;


        Managers.Data.InventoryDataChange(_idx, itemName); // DataManager�� �ִ� �κ��丮�� Data ����
                                                           //GameObject slot = Slots[_idx].transform.GetChild(0).gameObject; //������ �ڽ�(������ �̹���)

        GameObject slot = Slots[_idx].transform.gameObject;// ���� ������
        slot.SetActive(true); // Ȱ��ȭ
        slot.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Items/{itemName}"); // �ش� ���Կ� �̹����� �ٲ�
        slot.GetComponent<Slot>()._itemInfo.itemName = itemName; // ������ ������ �̸� ����
        slot.GetComponent<Slot>().inItem = true; // �������� ����
        _idx++;

    }

    public void Sell(Define.MouseState evt)
    {
        if (evt == Define.MouseState.RButtonDown && clickSlot && toolTip.gameObject.activeSelf) // �ش� evt�� ��Ŭ���̰� �����̾��ٸ�
        {
          
            clickSlot = false; // ������ Ŭ�� �������� ����

            Slot sellSlot = Slots[sellSlotIdx].transform.gameObject.GetComponent<Slot>(); // �ش� �Ǹ��� ������Ʈ
            if (sellSlot && sellSlot.inItem) // null üũ
            {
                sellSlot.GetComponent<Image>().sprite = Resources.Load<Sprite>("Etc/emptySlot"); // ��Ƽ ���� �̹����� ����
                sellSlot.GetComponent<Slot>()._itemInfo.itemName = "emptySlot";
                sellSlot.inItem = false;
                Managers.Data.Inventory.Remove(sellSlotIdx); // �ش� �ε��� ������ �����ϰ� �ش� ���� �� ���·� 
            }
            sellSlotIdx = -1;
            if (toolTip.gameObject.activeSelf)
            {
                toolTip.gameObject.SetActive(false);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // ���̾ �κ��丮�̰� Ŭ���� ���� �±װ� �����̶�� ( ���� ���Կ��� �±׸� �־���ұ�?)
        if (eventData.pointerPressRaycast.gameObject.CompareTag("Slot") &&
            eventData.pointerPressRaycast.gameObject.layer == (int)Define.UI.Inventory)
        {
            Slot tempSlot = eventData.pointerPressRaycast.gameObject.GetComponent<Slot>();
            if (tempSlot.inItem)
            {
                toolTip.sellOrPurchase.text = "��Ŭ�� �Ǹ�"; // �κ��丮 �ؽ� �Ǹŷ� ����
                toolTip.gameObject.SetActive(true); // ���� Ȱ��ȭ
                toolTip.SetItemInfo(tempSlot._itemInfo.itemName); // ������ �ش� ���� ������ ���� ����

                clickSlot = true;
                sellSlotIdx = tempSlot.transform.GetSiblingIndex();
            }

        }
    }



    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.gameObject.SetActive(false);
        clickSlot = false;
    }
}
