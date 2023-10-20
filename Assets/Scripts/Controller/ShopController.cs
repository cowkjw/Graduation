using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopController : MonoBehaviour, IPointerDownHandler, IPointerExitHandler
{
    public ItemTooltip toolTip;

    Slot _buySlot = null;
    Text _price;
    InventoryController _inventory;
    
    void Start()
    {
        if (transform.childCount != 0)
        {
            _price = transform.GetChild(0).GetComponent<Text>();
            _price.text = GetComponent<Slot>().ItemInfo.Price + "G";
        }
        _inventory = FindObjectOfType<InventoryController>();
        Managers.Input.MouseAction -= BuyingItem;
        Managers.Input.MouseAction += BuyingItem;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        GameObject pointer = eventData.pointerPressRaycast.gameObject;
        _buySlot = pointer.gameObject.GetComponent<Slot>();

        if (pointer.CompareTag("Slot") && pointer.layer == (int)Define.UI.Shop)
        {
            Slot slot = pointer.GetComponent<Slot>();
            if (slot == null || !slot.inItem)
                return;

            toolTip.SellOrPurchaseText.text = "��Ŭ�� ����";
            toolTip.gameObject.SetActive(true);
            toolTip.SetItemInfo(slot.ItemInfo.Name);
        }
    }

    public void BuyingItem(Define.MouseState evt)
    {
        if (_buySlot == null) // null �Ǵ�
            return;
        if (evt != Define.MouseState.RButtonDown || _buySlot.gameObject.layer != (int)Define.UI.Shop)
            return;

        int itemPrice = _buySlot.ItemInfo.Price;
        if (Managers.Data.Gold < itemPrice)
        {
#if UNITY_EDITOR
            Debug.Log("���� �Ұ�");
#endif
            return;
        }

        _inventory.AddItem(_buySlot.ItemInfo);
        Managers.Data.Gold -= itemPrice;
        Managers.Data.PlayerDataChange();
#if UNITY_EDITOR
        Debug.Log("���� ����");
#endif
        _buySlot = null;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.gameObject.SetActive(false);
        _buySlot = null;
    }
}
