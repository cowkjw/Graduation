using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipController : MonoBehaviour,IPointerEnterHandler,IPointerClickHandler,IPointerExitHandler
{

    public ItemTooltip toolTip;
    public void OnPointerClick(PointerEventData eventData)
    {
     
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GetComponent<Slot>().inItem)
        {
            toolTip.gameObject.SetActive(true);
            toolTip.SetItemInfo(GetComponent<Slot>()._itemInfo.itemName);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.gameObject.SetActive(false);
    }

    void Start()
    {
        toolTip.gameObject.SetActive(false);
    }

    
    void Update()
    {
        
    }
}
