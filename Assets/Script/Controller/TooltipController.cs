using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public ItemTooltip toolTip;
    bool isDown;


    public void OnPointerEnter(PointerEventData eventData)
    {
        //if (this.gameObject.layer == (int)Define.UI.Inventory) // Ŭ���Ѱ� ���̾ �κ��丮���
        //{
        //    if (GetComponent<Slot>().inItem)
        //    {
        //        toolTip.sellOrPurchase.text = "��Ŭ�� �Ǹ�"; // �κ��丮 true;
        //        toolTip.gameObject.SetActive(true);
        //        toolTip.SetItemInfo(GetComponent<Slot>()._itemInfo.itemName);
        //    }
        //}
        //else if (this.gameObject.layer == (int)Define.UI.Shop)
        //{
        //    if (GetComponent<Slot>().inItem)
        //    {
        //        toolTip.sellOrPurchase.text = "��Ŭ�� ����";
        //        toolTip.gameObject.SetActive(true);
        //        toolTip.SetItemInfo(GetComponent<Slot>()._itemInfo.itemName);
        //    }
        //}

    }

    public void OnPointerExit(PointerEventData eventData)
    {
       // toolTip.gameObject.SetActive(false);
    }


    void Start()
    {
        toolTip = transform.root.GetChild(3).GetComponent<ItemTooltip>();
        toolTip.gameObject.SetActive(false);
    }
}
