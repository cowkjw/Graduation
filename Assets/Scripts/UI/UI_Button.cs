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
            if (Managers.UI.Inventory.activeSelf) // �κ��丮�� �����ִٸ�
            {
                if (GameObject.FindGameObjectWithTag("NPC")) // �ش� �ʿ� ���� NPC�� �ִٸ�
                {
                    if (Managers.UI.NpcUI?.activeSelf == false) // NPC ��üũ ������ ���������� �� �κ��丮 ��Ȱ��ȭ
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
