using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    [SerializeField]
    Text ItemNameTooltip;// 아이템 이름 툴팁

   public bool isDown;

    

    public void SetItemInfo(string name)
    {
        ItemNameTooltip.text = name;
    }
    private void Start()
    {
        isDown = false;
    }
    void Update()
    {
        if(!isDown)
        {
        transform.position = Input.mousePosition;
            
        }
    }

}
