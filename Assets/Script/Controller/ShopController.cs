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
        // 레이어가 인벤토리이고 클린된 곳의 태그가 슬롯이라면 ( 굳이 슬롯에도 태그를 넣어야할까?)
        if (eventData.pointerPressRaycast.gameObject.CompareTag("Slot") &&
            eventData.pointerPressRaycast.gameObject.layer == (int)Define.UI.Shop)
        {
            Slot tempSlot = eventData.pointerPressRaycast.gameObject.GetComponent<Slot>();
            if (tempSlot == null)
                return;
            buySlot = tempSlot;
            if (tempSlot.inItem)
            {

                toolTip.sellOrPurchase.text = "우클릭 구매"; // 인벤토리 텍스 판매로 변경
                toolTip.gameObject.SetActive(true); // 툴팁 활성화
                toolTip.SetItemInfo(tempSlot._itemInfo.itemName); // 툴팁에 해당 슬롯 아이템 정보 설정
            }

        }
    }

    public void BuyingItem(Define.MouseState evt)
    {
        if (buySlot == null) // null 판단
            return;
        if (evt == Define.MouseState.RButtonDown && buySlot.gameObject.layer == (int)Define.UI.Shop)
        {

            int haveGold = Convert.ToInt32(buySlot.transform.GetChild(0).GetComponent<Text>().text.Trim('G'));
            InventoryController inventory = FindObjectOfType<InventoryController>();
            inventory.AchiveItem(buySlot.GetComponent<Image>().sprite.name);
            if (Managers.Data.Gold >= haveGold)
            {
                Debug.Log("구매 가능");
                 
            }
            else
            {
                Debug.Log("구매 불가");
            }
            buySlot = null;
        }
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.gameObject.SetActive(false);
        buySlot = null;
    }
}
