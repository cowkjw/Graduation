using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Slot : MonoBehaviour, IPointerDownHandler, IPointerExitHandler
{
    public bool inItem = false;
    public Contents.Item ItemInfo { get { return _itemInfo; } private set { _itemInfo = value; } }
    public ItemTooltip toolTip;

    Contents.Item _itemInfo = new Contents.Item();
    Image itemImage;
    Sprite emptySlotSprite;
    TownScene townScene;
    InventoryController inventory;
    WeaponChangeController weaponSocket;

    void Awake()
    {
        weaponSocket = Managers.Game.GetPlayer().GetComponentInChildren<WeaponChangeController>();
        inventory = GetComponentInParent<InventoryController>();
        itemImage = GetComponent<Image>();
        emptySlotSprite = Resources.Load<Sprite>("Items/emptySlot");
        townScene = FindObjectOfType<TownScene>();
        if (Managers.Data.ItemDict.TryGetValue(itemImage.sprite.name != "emptySlot" ?
    int.Parse(itemImage.sprite.name) : -1, out Contents.Item tempItem))
        {
            _itemInfo = tempItem;
        }
            this.itemImage.sprite = inItem ? Resources.Load<Sprite>($"Items/{_itemInfo.Id}") : emptySlotSprite;
    }

    public void PutInItem(Contents.Item item)
    {
        _itemInfo = item;
        inItem = true;
        this.itemImage.sprite = Resources.Load<Sprite>($"Items/{ItemInfo.Id}");
    }

    public void ClearSlot()
    {
        itemImage.sprite = emptySlotSprite; // �� ���� �̹����� ����
        inItem = false;
        _itemInfo = null;
    }

    void Equip() // ��� ����
    {
        if (townScene!=null && townScene.NPCUI.activeSelf)
        {
            return;
        }
        if (!inItem)
            return;

        if (ItemInfo.ItemType == Define.ItemType.Equipment) // �����ϴ� ������ Ÿ���� �����
        {
            int currentItemId = Managers.Data.PlayerData.equippedWeapon; // ���� ���� ���� ID 
            weaponSocket?.ChangeWeapon(ItemInfo.Id); // ���� ���� �ƴ϶�� �ҷ��ͼ� �ش� ������ ID�� ����
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
        if (townScene == null|| !inItem) // Ÿ���� �ƴϰų� �������� ���� �����̸�
            return;
        
        if (gameObject.layer == (int)Define.UI.Inventory)
        {
            Managers.Data.Gold += ItemInfo.SellPrice; // �Ǹ� �����ϱ� �ǸŰ��ݸ�ŭ ��� �ø���
            int slotIndex = transform.GetSiblingIndex();
            Managers.Data.UpdateInventoryData(slotIndex, default, false); // �ش� �ε��� ������ �����ϰ� �ش� ���� �� ���·� 
            ClearSlot();
        }
        if (toolTip.gameObject.activeSelf)
        {
            toolTip.gameObject.SetActive(false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!inItem) // ������ ������ ����
        {
            return;
        }
        if (toolTip != null && toolTip.gameObject.activeSelf && eventData.button == PointerEventData.InputButton.Right) // toolTip�� null���� Ȯ��
        {
            if (townScene != null && townScene.NPCUI.activeSelf) // townScene�� null���� Ȯ��
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

        if (toolTip != null) // toolTip�� null���� Ȯ��
        {
            if (townScene != null && townScene.NPCUI.activeSelf) // townScene�� null���� Ȯ��
            {
                toolTip.sellOrPurchase.text = "��Ŭ�� �Ǹ�"; // �κ��丮 �ؽ� �Ǹŷ� ����
            }
            else
            {
                toolTip.sellOrPurchase.text = "��Ŭ�� ����"; // ������ ������������
            }

            toolTip.gameObject.SetActive(true); // ���� Ȱ��ȭ
            if (itemImage != null) // itemImage�� null���� Ȯ��
            {
                toolTip.SetItemInfo(ItemInfo.Name); // ������ �ش� ���� ������ ���� ����
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.gameObject.SetActive(false);
    }
}
