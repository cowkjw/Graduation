using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopController : MonoBehaviour, IPointerDownHandler, IPointerExitHandler
{
    public ItemTooltip toolTip;
    Slot buySlot = null;
    Text price;
    InventoryController inventory;
    void Start()
    {
        if (transform.childCount != 0)
        {
            price = transform.GetChild(0).GetComponent<Text>();
            price.text = GetComponent<Slot>().ItemInfo.Price + "G";
        }
        inventory = FindObjectOfType<InventoryController>();
        Managers.Input.MouseAction -= BuyingItem;
        Managers.Input.MouseAction += BuyingItem;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        GameObject pointer = eventData.pointerPressRaycast.gameObject;
        buySlot = pointer.gameObject.GetComponent<Slot>();

        if (pointer.CompareTag("Slot") && pointer.layer == (int)Define.UI.Shop)
        {
            Slot slot = pointer.GetComponent<Slot>();
            if (slot == null || !slot.inItem)
                return;

            toolTip.sellOrPurchase.text = "��Ŭ�� ����";
            toolTip.gameObject.SetActive(true);
            toolTip.SetItemInfo(slot.ItemInfo.Name);
        }
    }

    public void BuyingItem(Define.MouseState evt)
    {
        if (buySlot == null) // null �Ǵ�
            return;
        if (evt != Define.MouseState.RButtonDown || buySlot.gameObject.layer != (int)Define.UI.Shop)
            return;

        int itemPrice = buySlot.ItemInfo.Price;
        if (Managers.Data.Gold < itemPrice)
        {
            Debug.Log("���� �Ұ�");
            return;
        }

        inventory.AddItem(buySlot.ItemInfo);
        Managers.Data.Gold -= itemPrice;
        Managers.Data.PlayerDataChange();
        Debug.Log("���� ����");
        buySlot = null;
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.gameObject.SetActive(false);
        buySlot = null;
    }
}
