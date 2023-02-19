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
    bool clickInven = false;



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

        for (int i = 0; i < Slots.Length; i++)
        {
            Slot slot = Slots[i];

            if (_inventory.ContainsKey(i))
            {
                slot.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Items/{_inventory[i].Id}"); // �ش� ���Կ� �̹����� �ٲ�
                slot.inItem = true;
            }
            else
            {
                slot.GetComponent<Image>().sprite = Resources.Load<Sprite>("Items/emptySlot"); // �ش� ���Կ� �̹����� �ٲ�
            }

        }
    }

    public bool AddItem(Contents.Item item) // �������� �Ծ�����
    {
        int idx = 0;
        for (int i = 0; i < Slots.Length; i++)
        {
            if (Slots[i].inItem)
                idx++;
            else break;

        }
        if (idx > _inventorySize) // �κ��丮 ����
        {
            Debug.LogError("�κ��丮�� �� á���ϴ�");

            return false;
        }

        Managers.Data.InventoryDataChange(idx, item);

        Slot slot = Slots[idx].GetComponent<Slot>();// ���� ������
        slot.gameObject.SetActive(true); // Ȱ��ȭ
        slot.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Items/{item.Id}"); // �ش� ���Կ� �̹����� �ٲ�
        slot.ItemInfo.Name = item.Name; // ������ ������ �̸� ����
        slot.GetComponent<Slot>().inItem = true; // �������� ����

        return true;

    }

    public void Sell(Define.MouseState evt)
    {
        if (GameObject.FindObjectOfType<TownScene>() == null) // Ÿ���� �ƴ϶�� ����
            return;
        if (evt != Define.MouseState.RButtonDown || !GameObject.FindObjectOfType<TownScene>().NPCUI.activeSelf) // �ش� evt�� ��Ŭ���̰� ���� �����ְ�
            return;
        if (!clickInven) return;

        Slot sellSlot = Slots[sellSlotIdx].transform.gameObject.GetComponent<Slot>(); // �ش� �Ǹ��� ������Ʈ

        if (sellSlot && sellSlot.inItem && sellSlot.gameObject.layer == (int)Define.UI.Inventory) // null üũ
        {

            sellSlot.GetComponent<Image>().sprite = Resources.Load<Sprite>("items/emptySlot"); // �� ���� �̹����� ����
            sellSlot.inItem = false;
            Managers.Data.InventoryDataChange(sellSlotIdx, default, false); // �ش� �ε��� ������ �����ϰ� �ش� ���� �� ���·� 
        }
        if (toolTip.gameObject.activeSelf)
        {
            toolTip.gameObject.SetActive(false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        GameObject obj = eventData.pointerPressRaycast.gameObject;

        if (!obj.CompareTag("Slot") || obj.layer != (int)Define.UI.Inventory)
            return;

        BaseScene baseScene = GameObject.FindObjectOfType<BaseScene>(); 
        if (baseScene == null) return;

        clickInven = true;

        Slot tempSlot = obj.GetComponent<Slot>();
        if (!tempSlot.inItem)
            return;

        if (GameObject.FindObjectOfType<TownScene>()!=null&& GameObject.FindObjectOfType<TownScene>().NPCUI.activeSelf)
        {
            toolTip.sellOrPurchase.text = "��Ŭ�� �Ǹ�"; // �κ��丮 �ؽ� �Ǹŷ� ����
        }
        else
        {
            toolTip.sellOrPurchase.text = ""; // ������ ������������
        }

        toolTip.gameObject.SetActive(true); // ���� Ȱ��ȭ
        toolTip.SetItemInfo(tempSlot.ItemInfo.Name); // ������ �ش� ���� ������ ���� ����
        sellSlotIdx = tempSlot.transform.GetSiblingIndex();
    }





    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.gameObject.SetActive(false);
        clickInven = false;
    }
}
