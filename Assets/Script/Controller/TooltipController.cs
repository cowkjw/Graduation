using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipController : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler, IPointerDownHandler
{

    public ItemTooltip toolTip;
    bool isDown;
    float timeCheck;
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Click");
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (GetComponent<Slot>().inItem)
        {
            toolTip.gameObject.SetActive(true);
            toolTip.SetItemInfo(GetComponent<Slot>()._itemInfo.itemName);
        }
        toolTip.isDown = isDown = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GetComponent<Slot>().inItem&&!isDown)
        {
            toolTip.gameObject.SetActive(true);
            toolTip.SetItemInfo(GetComponent<Slot>()._itemInfo.itemName);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        timeCheck += Time.deltaTime;
        toolTip.isDown = isDown = false;
        if (timeCheck>=2f)
        {
            Debug.Log(timeCheck);
            toolTip.gameObject.SetActive(false);
            toolTip.isDown = isDown = false;
            timeCheck = 0;
        }

    }

    void Start()
    {
        timeCheck = 0f;
        isDown = false;
        toolTip = transform.root.GetChild(3).GetComponent<ItemTooltip>();
        toolTip.gameObject.SetActive(false);
    }


    void Update()
    {

    }


}
