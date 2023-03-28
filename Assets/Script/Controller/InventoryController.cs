using System;
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
    int selectSlotIdx;
    bool clickInven = false;
    public ItemTooltip toolTip;
    Text _goldText;
    WeaponChangeController weaponSocket;
    BaseScene baseScene;

    void Start()
    {
        Managers.Input.MouseAction -= Sell;
        Managers.Input.MouseAction += Sell;
        Managers.Input.MouseAction -= Equip;
        Managers.Input.MouseAction += Equip;
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
        weaponSocket= Managers.Game.GetPlayer().GetComponentInChildren<WeaponChangeController>();// ���� ���� ã��
        baseScene = FindObjectOfType<BaseScene>();

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
        
        int idx = Array.FindIndex(Slots, slot => !slot.inItem); // ���ٽ� ���
        if (idx < 0) // ������ -1 ��ȯ�ϱ� ����
        {
            Debug.LogError("�κ��丮 ����");
            return false;
        }

        Managers.Data.InventoryDataChange(idx, item);

        Slot slot = Slots[idx];
        slot.gameObject.SetActive(true);
        slot.PutInItem(item);

        return true;
    }

    public void Sell(Define.MouseState evt)
    {
        if (GameObject.FindObjectOfType<TownScene>() == null) // Ÿ���� �ƴ϶�� ����
            return;
        if (evt != Define.MouseState.RButtonDown || !GameObject.FindObjectOfType<TownScene>().NPCUI.activeSelf) // �ش� evt�� ��Ŭ���̰� ���� �����ְ�
            return;
        if (!clickInven) return;
        if (selectSlotIdx == -1) return;

        Slot sellSlot = Slots[selectSlotIdx].GetComponent<Slot>(); // �ش� �Ǹ��� ������Ʈ

        if (sellSlot && sellSlot.inItem && sellSlot.gameObject.layer == (int)Define.UI.Inventory) // null üũ
        {

            sellSlot.GetComponent<Image>().sprite = Resources.Load<Sprite>("items/emptySlot"); // �� ���� �̹����� ����
            sellSlot.inItem = false;
            Managers.Data.InventoryDataChange(selectSlotIdx, default, false); // �ش� �ε��� ������ �����ϰ� �ش� ���� �� ���·� 
        }
        if (toolTip.gameObject.activeSelf)
        {
            toolTip.gameObject.SetActive(false);
        }
    }

    public void Equip(Define.MouseState evt) // ��� ����
    {
        if (evt != Define.MouseState.RButtonDown) // �ش� evt�� ��Ŭ���̰� ���� �����ְ�
            return;
        if (!clickInven) return;
        if (selectSlotIdx == -1) return;

        if (baseScene is TownScene townScene && townScene.NPCUI.activeSelf)
        {
            return;
        }

        Slot equipSlot = Slots[selectSlotIdx].GetComponent<Slot>(); // ������ ������Ʈ
        
        weaponSocket?.ChangeWeapon(equipSlot.ItemInfo.Id); // ���� ���� �ƴ϶�� �ҷ�����
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        GameObject obj = eventData.pointerPressRaycast.gameObject;

        if (!obj.CompareTag("Slot") || obj.layer != (int)Define.UI.Inventory)
            return;

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
            toolTip.sellOrPurchase.text = "��Ŭ�� ����"; // ������ ������������
        }

        toolTip.gameObject.SetActive(true); // ���� Ȱ��ȭ
        toolTip.SetItemInfo(tempSlot.ItemInfo.Name); // ������ �ش� ���� ������ ���� ����
        selectSlotIdx = tempSlot.transform.GetSiblingIndex();
    }




    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.gameObject.SetActive(false);
        clickInven = false;
        selectSlotIdx = -1;
    }

}
