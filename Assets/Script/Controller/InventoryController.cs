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
    public bool clickInven;

    void OnEnable() // �տ��� �� �ʱ�ȭ�Ǹ� ( �̺�Ʈ ���� ���� )
    {
        clickInven = false;
        _inventory = Managers.Data.Inventory;
        _goldText = transform.GetChild(1).GetChild(0).GetComponent<Text>();
        for (int idx = 0; idx < 16; idx++) // �������� DataManager�� �ִ� ���ڸ�ŭ �κ��丮�� ä��
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
        if (_idx > 15) // �κ��丮 ����
        {
            Debug.LogError("�κ��丮�� �� á���ϴ�");
            return;
        }


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
        if (GameObject.FindObjectOfType<TownScene>() == null) // Ÿ���� �ƴ϶�� ����
            return;
        if (evt == Define.MouseState.RButtonDown && GameObject.FindObjectOfType<TownScene>().NPCUI.activeSelf) // �ش� evt�� ��Ŭ���̰� ���� �����ְ�
        {
            if (!clickInven) return;
            Slot sellSlot = Slots[sellSlotIdx].transform.gameObject.GetComponent<Slot>(); // �ش� �Ǹ��� ������Ʈ
            
            if (sellSlot && sellSlot.inItem&&sellSlot.gameObject.layer == (int)Define.UI.Inventory) // null üũ
            {
                sellSlot.GetComponent<Image>().sprite = Resources.Load<Sprite>("Etc/emptySlot"); // �� ���� �̹����� ����
                sellSlot.GetComponent<Slot>()._itemInfo.itemName = "emptySlot";
                sellSlot.inItem = false;
                Managers.Data.Inventory.Remove(sellSlotIdx); // �ش� �ε��� ������ �����ϰ� �ش� ���� �� ���·� 
            }
           // sellSlotIdx = 0;
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
            clickInven = true;
            Slot tempSlot = eventData.pointerPressRaycast.gameObject.GetComponent<Slot>();
            if (tempSlot.inItem)
            {
                if (GameObject.FindObjectOfType<TownScene>() == null) return; // Ÿ���� �ƴ϶��

                if (GameObject.FindObjectOfType<TownScene>().NPCUI.activeSelf)
                {
                    toolTip.sellOrPurchase.text = "��Ŭ�� �Ǹ�"; // �κ��丮 �ؽ� �Ǹŷ� ����
                }
                else
                {
                    toolTip.sellOrPurchase.text = ""; // ������ ������������
                }

                toolTip.gameObject.SetActive(true); // ���� Ȱ��ȭ
                toolTip.SetItemInfo(tempSlot._itemInfo.itemName); // ������ �ش� ���� ������ ���� ����
                sellSlotIdx = tempSlot.transform.GetSiblingIndex();
            }

        }
        else
        {
            sellSlotIdx = -1;
        }
    }



    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.gameObject.SetActive(false);
        clickInven = false;
    }
}
