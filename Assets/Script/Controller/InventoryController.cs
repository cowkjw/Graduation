using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class InventoryController : MonoBehaviour, IPointerDownHandler, IPointerExitHandler
{
    public Slot[] Slots;
    int _idx;

    List<Contents.Item> testInven = new List<Contents.Item>();
    Dictionary<int, Item> _inventory;
    public ItemTooltip toolTip;
    Text _goldText;

    int _inventorySize = 15;

    int sellSlotIdx;
    public bool clickInven;

    void OnEnable() // �տ��� �� �ʱ�ȭ�Ǹ� ( �̺�Ʈ ���� ���� )
    {
        clickInven = false;
        _inventory = Managers.Data.Inventory;
        _goldText = transform.GetChild(1).GetChild(0).GetComponent<Text>();

        for (int idx = 0; idx < _inventorySize + 1; idx++) // �������� DataManager�� �ִ� ���ڸ�ŭ �κ��丮�� ä��
        {
            if (_inventory.ContainsKey(idx))
            {
                Slot slot = Slots[idx];// ���� ������

                slot.gameObject.SetActive(true); // Ȱ��ȭ
                slot.inItem = true;
                slot.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Items/{_inventory[idx].Name}"); // �ش� ���Կ� �̹����� �ٲ�
                slot._itemInfo.Name = _inventory[idx].Name; // ������ �̸� ����
            }
        }


        //for (int idx = 0; idx < _inventorySize+1; idx++) // �������� DataManager�� �ִ� ���ڸ�ŭ �κ��丮�� ä��
        //{
        //    if (_inventory.ContainsKey(idx))
        //    {
        //        Slot slot = Slots[idx];// ���� ������

        //        slot.gameObject.SetActive(true); // Ȱ��ȭ
        //        slot.inItem = true;
        //        slot.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Items/{_inventory[idx].Name}"); // �ش� ���Կ� �̹����� �ٲ�
        //        slot._itemInfo.Name = _inventory[idx].Name; // ������ �̸� ����
        //    }
        //}
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


    public void AchiveItem(Contents.Item item) // �������� �Ծ�����
    {
        int idx = 0;
        foreach(var checkSlot in Slots)
        {
            if (checkSlot.inItem)
                idx++;
            else break;
        }
        Managers.Data.InventoryDataChange(_idx, item);
      //  _idx = Managers.Data.InvenDict.Count;
        //for (int i = 0; i < _inventorySize; i++)
        //{
        //    if (!Managers.Data.Inventory.ContainsKey(i))
        //    {
        //        _idx = i;
        //        break;
        //    }

        //}
        //if (_idx > _inventorySize) // �κ��丮 ����
        //{
        //    Debug.LogError("�κ��丮�� �� á���ϴ�");
        //    return;
        //}



        Slot slot = Slots[_idx];// ���� ������
        slot.gameObject.SetActive(true); // Ȱ��ȭ
        slot.inItem = true; // �������� ����
        slot.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Items/{item.Name}"); // �ش� ���Կ� �̹����� �ٲ�
        slot._itemInfo.Name = item.Name; // ������ ������ �̸� ����
        //_idx++;

    }

    //public void AchiveItem(Item item) // �������� �Ծ�����
    //{
    //    _idx = Managers.Data.InventoryCount;
    //    for (int i = 0; i < _inventorySize; i++)
    //    {
    //        if (!Managers.Data.Inventory.ContainsKey(i))
    //        {
    //            _idx = i;
    //            break;
    //        }

    //    }
    //    if (_idx > _inventorySize) // �κ��丮 ����
    //    {
    //        Debug.LogError("�κ��丮�� �� á���ϴ�");
    //        return;
    //    }


    //    Managers.Data.InventoryDataChange(_idx, item);

    //    Slot slot = Slots[_idx];// ���� ������
    //    slot.gameObject.SetActive(true); // Ȱ��ȭ
    //    slot.inItem = true; // �������� ����
    //    slot.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Items/{item.Name}"); // �ش� ���Կ� �̹����� �ٲ�
    //    slot._itemInfo.Name = item.Name; // ������ ������ �̸� ����
    //    _idx++;

    //}

    public void Sell(Define.MouseState evt)
    {
        if (GameObject.FindObjectOfType<TownScene>() == null) // Ÿ���� �ƴ϶�� ����
            return;
        if (evt == Define.MouseState.RButtonDown && GameObject.FindObjectOfType<TownScene>().NPCUI.activeSelf) // �ش� evt�� ��Ŭ���̰� ���� �����ְ�
        {
           if (!clickInven) return;
            Slot sellSlot = Slots[sellSlotIdx]; // �ش� �Ǹ��� ������Ʈ
            
            if (sellSlot && sellSlot.inItem&&sellSlot.gameObject.layer == (int)Define.UI.Inventory) // null üũ
            {
                sellSlot.GetComponent<Image>().sprite = Resources.Load<Sprite>("Etc/emptySlot"); // �� ���� �̹����� ����
                sellSlot._itemInfo.Name = "emptySlot";
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

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    // ���̾ �κ��丮�̰� Ŭ���� ���� �±װ� �����̶�� ( ���� ���Կ��� �±׸� �־���ұ�?)
    //    if (eventData.button != PointerEventData.InputButton.Left)
    //        return;

    //    if (eventData.pointerPressRaycast.gameObject.CompareTag("Slot") &&
    //        eventData.pointerPressRaycast.gameObject.layer == (int)Define.UI.Inventory)
    //    {
    //        clickInven = true;
    //        Slot tempSlot = eventData.pointerPressRaycast.gameObject.GetComponent<Slot>();
    //        if (tempSlot.inItem)
    //        {
    //            if (GameObject.FindObjectOfType<TownScene>() == null) return; // Ÿ���� �ƴ϶��

    //            if (GameObject.FindObjectOfType<TownScene>().NPCUI.activeSelf)
    //            {
    //                toolTip.sellOrPurchase.text = "��Ŭ�� �Ǹ�"; // �κ��丮 �ؽ� �Ǹŷ� ����
    //            }
    //            else
    //            {
    //                toolTip.sellOrPurchase.text = ""; // ������ ������������
    //            }

    //            toolTip.gameObject.SetActive(true); // ���� Ȱ��ȭ
    //            toolTip.SetItemInfo(tempSlot._itemInfo.Name); // ������ �ش� ���� ������ ���� ����
    //            sellSlotIdx = tempSlot.transform.GetSiblingIndex();
    //        }

    //    }
    //    else
    //    {
    //        sellSlotIdx = -1;
    //    }
    //}

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (!eventData.pointerPressRaycast.gameObject.CompareTag("Slot") ||
            eventData.pointerPressRaycast.gameObject.layer != (int)Define.UI.Inventory)
            return;

        clickInven = true;

        Slot tempSlot = eventData.pointerPressRaycast.gameObject.GetComponent<Slot>();
        if (!tempSlot.inItem)
            return;

        TownScene townScene = GameObject.FindObjectOfType<TownScene>();
        if (townScene == null)
        {
            toolTip.gameObject.SetActive(true);
            toolTip.SetItemInfo(tempSlot._itemInfo.Name);
            toolTip.sellOrPurchase.text = "";
            return;
        }

        toolTip.gameObject.SetActive(true);
        toolTip.SetItemInfo(tempSlot._itemInfo.Name);
        sellSlotIdx = tempSlot.transform.GetSiblingIndex();

        if (townScene.NPCUI.activeSelf)
        {
            toolTip.sellOrPurchase.text = "��Ŭ�� �Ǹ�";
        }
        else
        {
            toolTip.sellOrPurchase.text = "";
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.gameObject.SetActive(false);
        clickInven = false;
    }
}
