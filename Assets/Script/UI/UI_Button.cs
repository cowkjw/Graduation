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
    public void NPCCloseButton()
    {

        if (npcUI.activeSelf)
            npcUI.SetActive(false);
     
    }

    //public void InventoryClose()
    //{
    //    if (inventoryUI.activeSelf)
    //        inventoryUI.SetActive(false);
    //}

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
}
