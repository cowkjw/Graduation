using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class DragItem : MonoBehaviour, IPointerClickHandler
{

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log(transform.parent.GetComponent<Slot>()._itemInfo.itemName);
    }
}
