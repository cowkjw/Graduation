using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Slot : MonoBehaviour, IPointerDownHandler, IPointerExitHandler
{
    public bool inItem = false;
    public Contents.Item ItemInfo { get { return _itemInfo; } private set { _itemInfo = value; } }
    public ItemTooltip ToolTip;

    Contents.Item _itemInfo = new Contents.Item();
    Image _itemImage;
    Sprite _emptySlotSprite;
    TownScene _townScene;
    InventoryController _inventory;
    WeaponChangeController _weaponSocket;

    void Awake()
    {
        _weaponSocket = Managers.Game.GetPlayer().GetComponentInChildren<WeaponChangeController>();
        _inventory = GetComponentInParent<InventoryController>();
        _itemImage = GetComponent<Image>();
        _emptySlotSprite = Resources.Load<Sprite>("Items/emptySlot");
        _townScene = FindObjectOfType<TownScene>();
        if (Managers.Data.ItemDict.TryGetValue(_itemImage.sprite.name != "emptySlot" ?
    int.Parse(_itemImage.sprite.name) : -1, out Contents.Item tempItem))
        {
            _itemInfo = tempItem;
        }
            _itemImage.sprite = inItem ? Resources.Load<Sprite>($"Items/{_itemInfo.Id}") : _emptySlotSprite;
    }

    public void PutInItem(Contents.Item item)
    {
        _itemInfo = item;
        inItem = true;
        _itemImage.sprite = Resources.Load<Sprite>($"Items/{ItemInfo.Id}");
    }

    public void ClearSlot()
    {
        _itemImage.sprite = _emptySlotSprite; // �� ���� �̹����� ����
        inItem = false;
        _itemInfo = null;
    }

    void Equip() // ��� ����
    {
        if (_townScene!=null && Managers.UI.NpcUI.activeSelf)
        {
            return;
        }
        if (!inItem)
            return;

        if (ItemInfo.ItemType == Define.ItemType.Equipment) // �����ϴ� ������ Ÿ���� �����
        {
            int currentItemId = Managers.Data.PlayerData.equippedWeapon; // ���� ���� ���� ID 
            _weaponSocket?.ChangeWeapon(ItemInfo.Id); // ���� ���� �ƴ϶�� �ҷ��ͼ� �ش� ������ ID�� ����
            if (Managers.Data.ItemDict.TryGetValue(currentItemId, out Contents.Item currentEquipItem)) // Item table���� ������ ��� ���� ������
            {
                Managers.Game.GetPlayer().GetComponent<PlayerStat>().Attack -= currentEquipItem.Attack; // ���� ���� ���� ���� �߰� ���ݷ� ����
                PutInItem(currentEquipItem);
            }
            int slotIndex = transform.GetSiblingIndex();
            Managers.Data.UpdateInventoryData(slotIndex, currentEquipItem, true); // �ش� �ε��� ������ ���� �� �� �������� ��ü 
        }

    }

    void Sell()
    {
        if (_townScene == null|| !inItem) // Ÿ���� �ƴϰų� �������� ���� �����̸�
            return;
        
        if (gameObject.layer == (int)Define.UI.Inventory)
        {
            Managers.Data.Gold += ItemInfo.SellPrice; // �Ǹ� �����ϱ� �ǸŰ��ݸ�ŭ ��� �ø���
            int slotIndex = transform.GetSiblingIndex();
            Managers.Data.UpdateInventoryData(slotIndex, default, false); // �ش� �ε��� ������ �����ϰ� �ش� ���� �� ���·� 
            ClearSlot();
        }
        if (ToolTip.gameObject.activeSelf)
        {
            ToolTip.gameObject.SetActive(false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!inItem) // ������ ������ ����
        {
            return;
        }
        if (ToolTip != null && ToolTip.gameObject.activeSelf && eventData.button == PointerEventData.InputButton.Right) // toolTip�� null���� Ȯ��
        {
            if (_townScene != null && Managers.UI.NpcUI.activeSelf) // townScene�� null���� Ȯ��
            {
                Sell();
            }
            else
            {
                Equip();
            }
        }

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (ToolTip != null) // toolTip�� null���� Ȯ��
        {
            if (_townScene != null && Managers.UI.NpcUI.activeSelf) // townScene�� null���� Ȯ��
            {
                ToolTip.SellOrPurchaseText.text = "��Ŭ�� �Ǹ�"; // �κ��丮 �ؽ� �Ǹŷ� ����
            }
            else
            {
                ToolTip.SellOrPurchaseText.text = "��Ŭ�� ����"; // ������ ������������
            }

            ToolTip.gameObject.SetActive(true); // ���� Ȱ��ȭ
            if (_itemImage != null) // itemImage�� null���� Ȯ��
            {
                ToolTip.SetItemInfo(ItemInfo.Name); // ������ �ش� ���� ������ ���� ����
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTip.gameObject.SetActive(false);
    }
}
