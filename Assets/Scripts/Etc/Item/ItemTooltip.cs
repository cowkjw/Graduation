using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    public Text SellOrPurchaseText;
    [SerializeField]
    Text _itemNameTooltip;// ������ �̸� ����

    public void SetItemInfo(string name)
    {
        _itemNameTooltip.text = name;
    }
    void Update()
    {
        transform.position = Input.mousePosition;
    }

}
