using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 
 

public class Slot : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
{
    public struct ItemInfo
    {
        public string itemName;
        public Define.ItemType itemType;
    }


    Image _itemImage;
    public ItemInfo _itemInfo;

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
      
    }
}
