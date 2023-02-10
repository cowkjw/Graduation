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

    void Start()
    {
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

            toolTip.sellOrPurchase.text = "우클릭 구매";
            toolTip.gameObject.SetActive(true);
            toolTip.SetItemInfo(slot._itemInfo.Name);
        }
    }

    public void BuyingItem(Define.MouseState evt)
    {
        if (buySlot == null) // null 판단
            return;
        if (evt != Define.MouseState.RButtonDown && buySlot.gameObject.layer != (int)Define.UI.Shop)
            return;

        int haveGold = Convert.ToInt32(buySlot.transform.GetChild(0).GetComponent<Text>().text.Trim('G'));
        InventoryController inventory = FindObjectOfType<InventoryController>();
        if (Managers.Data.Gold < haveGold)
        {
            Debug.Log("구매 불가");
           // return;
        }

        inventory.AddItem(buySlot._itemInfo);
        Debug.Log("구매 가능");
        buySlot = null;
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.gameObject.SetActive(false);
        buySlot = null;
    }
}
