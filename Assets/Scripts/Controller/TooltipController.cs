using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipController : MonoBehaviour, IPointerExitHandler
{

    public ItemTooltip toolTip;
    bool isDown;


   
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
