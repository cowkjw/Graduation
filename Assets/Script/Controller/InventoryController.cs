using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class InventoryController : MonoBehaviour, IPointerDownHandler, IPointerExitHandler
{

    public Slot[] Slots;

    List<Contents.Item> testInven;
    Dictionary<int, Contents.Item> _inventory;

    public ItemTooltip toolTip;
    Text _goldText;

    int sellSlotIdx;
    bool clickInven = false;



    void Start()
    {
        Managers.Input.MouseAction -= Sell;
        Managers.Input.MouseAction += Sell;
        Init();
    }

    private void Update()
    {
        _goldText.text = Managers.Data.Gold.ToString(); // 수정 예정
    }


    void Init()
    {
        clickInven = false;
        _inventory = Managers.Data.InvenDict;
        _goldText = transform.GetChild(1).GetChild(0).GetComponent<Text>();

        for (int i = 0; i < Slots.Length; i++)
        {
            Slot slot = Slots[i];

            if (_inventory.ContainsKey(i))
            {
                slot.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Items/{_inventory[i].Id}"); // 해당 슬롯에 이미지를 바꿈
                slot.inItem = true;
            }
            else
            {
                slot.GetComponent<Image>().sprite = Resources.Load<Sprite>("Items/emptySlot"); // 해당 슬롯에 이미지를 바꿈
            }

        }
    }

    public bool AddItem(Contents.Item item) // 아이템을 먹었을때
    {

        int idx = Array.FindIndex(Slots, slot => !slot.inItem); // 람다식 사용
        if (idx < 0) // 없으면 -1 반환하기 때문
        {
            Debug.LogError("인벤토리 꽉참");
            return false;
        }

        Managers.Data.InventoryDataChange(idx, item);

        Slot slot = Slots[idx];
        slot.ItemInfo.Name = item.Name;
        slot.gameObject.SetActive(true);
        slot.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Items/{item.Id}");
        slot.inItem = true;

        return true;
    }

    public void Sell(Define.MouseState evt)
    {
        if (GameObject.FindObjectOfType<TownScene>() == null) // 타운이 아니라면 리턴
            return;
        if (evt != Define.MouseState.RButtonDown || !GameObject.FindObjectOfType<TownScene>().NPCUI.activeSelf) // 해당 evt가 우클릭이고 상점 켜져있고
            return;
        if (!clickInven) return;
        if (sellSlotIdx == -1) return;

        Slot sellSlot = Slots[sellSlotIdx].GetComponent<Slot>(); // 해당 판매할 오브젝트

        if (sellSlot && sellSlot.inItem && sellSlot.gameObject.layer == (int)Define.UI.Inventory) // null 체크
        {

            sellSlot.GetComponent<Image>().sprite = Resources.Load<Sprite>("items/emptySlot"); // 빈 슬롯 이미지로 변경
            sellSlot.inItem = false;
            Managers.Data.InventoryDataChange(sellSlotIdx, default, false); // 해당 인덱스 아이템 삭제하고 해당 슬롯 빈 상태로 
        }
        if (toolTip.gameObject.activeSelf)
        {
            toolTip.gameObject.SetActive(false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        GameObject obj = eventData.pointerPressRaycast.gameObject;

        if (!obj.CompareTag("Slot") || obj.layer != (int)Define.UI.Inventory)
            return;

        BaseScene baseScene = GameObject.FindObjectOfType<BaseScene>(); 
        if (baseScene == null) return;

        clickInven = true;

        Slot tempSlot = obj.GetComponent<Slot>();
        if (!tempSlot.inItem)
            return;

        if (GameObject.FindObjectOfType<TownScene>()!=null&& GameObject.FindObjectOfType<TownScene>().NPCUI.activeSelf)
        {
            toolTip.sellOrPurchase.text = "우클릭 판매"; // 인벤토리 텍스 판매로 변경
        }
        else
        {
            toolTip.sellOrPurchase.text = ""; // 상점이 안켜져있을때
        }

        toolTip.gameObject.SetActive(true); // 툴팁 활성화
        toolTip.SetItemInfo(tempSlot.ItemInfo.Name); // 툴팁에 해당 슬롯 아이템 정보 설정
        sellSlotIdx = tempSlot.transform.GetSiblingIndex();
    }





    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.gameObject.SetActive(false);
        clickInven = false;
        sellSlotIdx = -1;
    }
}
