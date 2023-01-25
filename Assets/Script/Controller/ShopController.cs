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
        // ���̾ �κ��丮�̰� Ŭ���� ���� �±װ� �����̶�� ( ���� ���Կ��� �±׸� �־���ұ�?)
        if (eventData.pointerPressRaycast.gameObject.CompareTag("Slot") &&
            eventData.pointerPressRaycast.gameObject.layer == (int)Define.UI.Shop)
        {
            Slot tempSlot = eventData.pointerPressRaycast.gameObject.GetComponent<Slot>();
            if (tempSlot == null)
                return;
            buySlot = tempSlot;
            if (tempSlot.inItem)
            {

                toolTip.sellOrPurchase.text = "��Ŭ�� ����"; // �κ��丮 �ؽ� �Ǹŷ� ����
                toolTip.gameObject.SetActive(true); // ���� Ȱ��ȭ
                toolTip.SetItemInfo(tempSlot._itemInfo.itemName); // ������ �ش� ���� ������ ���� ����
            }

        }
    }

    public void BuyingItem(Define.MouseState evt)
    {
        if (buySlot == null) // null �Ǵ�
            return;
        if (evt == Define.MouseState.RButtonDown && buySlot.gameObject.layer == (int)Define.UI.Shop)
        {

            int haveGold = Convert.ToInt32(buySlot.transform.GetChild(0).GetComponent<Text>().text.Trim('G'));
            InventoryController inventory = FindObjectOfType<InventoryController>();
            inventory.AchiveItem(buySlot.GetComponent<Image>().sprite.name);
            if (Managers.Data.Gold >= haveGold)
            {
                Debug.Log("���� ����");
                 
            }
            else
            {
                Debug.Log("���� �Ұ�");
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
