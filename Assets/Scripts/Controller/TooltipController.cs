using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipController : MonoBehaviour, IPointerExitHandler
{

    public ItemTooltip ToolTip;
    bool isDown;
    public void OnPointerExit(PointerEventData eventData)
    {
       // toolTip.gameObject.SetActive(false);
    }


    void Start()
    {
        ToolTip = transform.root.GetChild(3).GetComponent<ItemTooltip>();
        ToolTip.gameObject.SetActive(false);
    }
}
