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
        //if (this.gameObject.layer == (int)Define.UI.Inventory) // 클릭한게 레이어가 인벤토리라면
        //{
        //    if (GetComponent<Slot>().inItem)
        //    {
        //        toolTip.sellOrPurchase.text = "우클릭 판매"; // 인벤토리 true;
        //        toolTip.gameObject.SetActive(true);
        //        toolTip.SetItemInfo(GetComponent<Slot>()._itemInfo.itemName);
        //    }
        //}
        //else if (this.gameObject.layer == (int)Define.UI.Shop)
        //{
        //    if (GetComponent<Slot>().inItem)
        //    {
        //        toolTip.sellOrPurchase.text = "우클릭 구매";
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
