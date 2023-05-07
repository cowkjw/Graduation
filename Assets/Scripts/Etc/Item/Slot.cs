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
        itemImage.sprite = emptySlotSprite; // 빈 슬롯 이미지로 변경
        inItem = false;
        _itemInfo = null;
    }

    void Equip() // 장비 장착
    {
        if (townScene!=null && townScene.NPCUI.activeSelf)
        {
            return;
        }
        if (!inItem)
            return;

        if (ItemInfo.ItemType == Define.ItemType.Equipment) // 장착하는 아이템 타입이 장비라면
        {
            int currentItemId = Managers.Data.PlayerData.equippedWeapon; // 현재 장착 무기 ID 
            weaponSocket?.ChangeWeapon(ItemInfo.Id); // 만약 널이 아니라면 불러와서 해당 아이템 ID로 변경
            if (Managers.Data.ItemDict.TryGetValue(currentItemId, out Contents.Item currentEquipItem)) // Item table에서 해제한 장비 정보 가져옴
            {
                Managers.Game.GetPlayer().GetComponent<PlayerStat>().Attack -= currentEquipItem.Attack; // 현재 장착 중인 무기 추가 공격력 빼기
                PutInItem(currentEquipItem);
            }
            int slotIndex = transform.GetSiblingIndex();
            Managers.Data.UpdateInventoryData(slotIndex, currentEquipItem, true); // 해당 인덱스 아이템 변경 및 빈 슬롯으로 교체 
        }

    }

    void Sell()
    {
        if (townScene == null|| !inItem) // 타운이 아니거나 아이템이 없는 슬롯이면
            return;
        
        if (gameObject.layer == (int)Define.UI.Inventory)
        {
            Managers.Data.Gold += ItemInfo.SellPrice; // 판매 했으니까 판매가격만큼 골드 올리기
            int slotIndex = transform.GetSiblingIndex();
            Managers.Data.UpdateInventoryData(slotIndex, default, false); // 해당 인덱스 아이템 삭제하고 해당 슬롯 빈 상태로 
            ClearSlot();
        }
        if (toolTip.gameObject.activeSelf)
        {
            toolTip.gameObject.SetActive(false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!inItem) // 아이템 없으면 리턴
        {
            return;
        }
        if (toolTip != null && toolTip.gameObject.activeSelf && eventData.button == PointerEventData.InputButton.Right) // toolTip이 null인지 확인
        {
            if (townScene != null && townScene.NPCUI.activeSelf) // townScene이 null인지 확인
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

        if (toolTip != null) // toolTip이 null인지 확인
        {
            if (townScene != null && townScene.NPCUI.activeSelf) // townScene이 null인지 확인
            {
                toolTip.sellOrPurchase.text = "우클릭 판매"; // 인벤토리 텍스 판매로 변경
            }
            else
            {
                toolTip.sellOrPurchase.text = "우클릭 장착"; // 상점이 안켜져있을때
            }

            toolTip.gameObject.SetActive(true); // 툴팁 활성화
            if (itemImage != null) // itemImage가 null인지 확인
            {
                toolTip.SetItemInfo(ItemInfo.Name); // 툴팁에 해당 슬롯 아이템 정보 설정
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.gameObject.SetActive(false);
    }
}
