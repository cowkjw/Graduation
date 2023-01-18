using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    [SerializeField]
    Text ItemNameTooltip;// ������ �̸� ����
    
    public Text sellOrPurchase;


    public void SetItemInfo(string name)
    {
        ItemNameTooltip.text = name;
    }
    void Update()
    {
        transform.position = Input.mousePosition;
    }

}
