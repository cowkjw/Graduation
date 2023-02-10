using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class InventoryController : MonoBehaviour, IPointerDownHandler, IPointerExitHandler
{

    public Slot[] Slots;

    List<Contents.Item> testInven;
    Dictionary<int, Contents.Item> _inventory;

    public ItemTooltip toolTip;
    Text _goldText;

    int _inventorySize = 15;

    int sellSlotIdx;
    bool clickInven;



    void Start()
    {
        Managers.Input.MouseAction -= Sell;
        Managers.Input.MouseAction += Sell;
        Init();
    }

    private void Update()
    {
        _goldText.text = Managers.Data.Gold.ToString(); // ���� ����
    }


    void Init()
    {
        clickInven = false;
        _inventory = Managers.Data.InvenDict;
        _goldText = transform.GetChild(1).GetChild(0).GetComponent<Text>();

        for(int i = 0;i<Slots.Length;i++)
        {
            Slot slot = Slots[i];

            if(_inventory.ContainsKey(i))
            {
                slot.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Items/{_inventory[i].Name}"); // �ش� ���Կ� �̹����� �ٲ�
                slot._itemInfo.Name = _inventory[i].Name; // ������ �̸� ����
                slot.inItem = true;
            }
            else
            {
                slot._itemInfo.Name = "emptySlot"; // ������ �̸� ����
                slot.GetComponent<Image>().sprite = Resources.Load<Sprite>("Items/emptySlot"); // �ش� ���Կ� �̹����� �ٲ�
            }

        }
    }

    public bool AddItem(Contents.Item item) // �������� �Ծ�����
    {
        _idx = Managers.Data.InventoryCount;
        for (int i = 0; i < _inventorySize; i++)
        {
            if (checkSlot.inItem)
                idx++;
            else break;

        }
        if (_idx > _inventorySize) // �κ��丮 ����
        {
            Debug.LogError("�κ��丮�� �� á���ϴ�");

            return false;
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

                sellSlot.GetComponent<Image>().sprite = Resources.Load<Sprite>("items/emptySlot"); // �� ���� �̹����� ����
                sellSlot.inItem = false;
                //sellSlot._itemInfo.Name = "emptySlot";
                Managers.Data.InventoryDataChange(sellSlotIdx, default,false); // �ش� �ε��� ������ �����ϰ� �ش� ���� �� ���·� 
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

        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        GameObject obj = eventData.pointerPressRaycast.gameObject;

        if (!obj.CompareTag("Slot") ||obj.layer != (int)Define.UI.Inventory)
            return;

        clickInven = true;

        Slot tempSlot = obj.GetComponent<Slot>();
        if (!tempSlot.inItem)
            return;

        TownScene townScene = GameObject.FindObjectOfType<TownScene>();
        if (townScene == null)

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
