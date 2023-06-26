using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Button : MonoBehaviour
{
    [SerializeField]
    GameObject npcUI;
    [SerializeField]
    GameObject inventoryUI;
    [SerializeField]
    GameObject shop;
    [SerializeField]
    GameObject combination;

    void Start()
    {
        Managers.Input.KeyboardAction -= UIKeyEvent;
        Managers.Input.KeyboardAction += UIKeyEvent;
    }

    public void NPCCloseButton()
    {
        if (npcUI.activeSelf)
        {
            npcUI.SetActive(false);
        }
    }

    public void InventoryClose()
    {
        if (inventoryUI.activeSelf)
        {
            inventoryUI.SetActive(false);
        }
    }

    public void DisplayShop()
    {
        combination.SetActive(false);
        shop.SetActive(true);
    }

    public void DisPlayCombination()
    {
        shop.SetActive(false);
        combination.SetActive(true);
    }


    public void UIKeyEvent(Enum uiType)
    {
        if ((Define.UI)uiType == Define.UI.Inventory)
        {
            if (Managers.UI.Inventory.activeSelf) // 인벤토리가 켜져있다면
            {
                if (GameObject.FindGameObjectWithTag("NPC")) // 해당 맵에 상점 NPC가 있다면
                {
                    if (Managers.UI.NpcUI?.activeSelf == false) // NPC 널체크 상점이 안켜져있을 때 인벤토리 비활성화
                    {
                        Managers.UI.Inventory?.SetActive(false);
                    }
                }
                else
                {
                    Managers.UI.Inventory?.SetActive(false);
                }
            }
            else
            {
                Managers.UI.Inventory?.SetActive(true);
            }
        }
    }
}
