using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class InventoryController : MonoBehaviour, IPointerDownHandler, IPointerExitHandler
{
    public Slot[] Slots;

    List<Contents.Item> testInven;
    Dictionary<int, Item> _inventory;
    public ItemTooltip toolTip;
    Text _goldText;
    int inventorSize = 15;
    int sellSlotIdx;
    public bool clickInven;

    void OnEnable() // 앞에서 다 초기화되면 ( 이벤트 루프 오류 )
    {
        clickInven = false;
        testInven = Managers.Data.InvenList;
        _goldText = transform.GetChild(1).GetChild(0).GetComponent<Text>();

        for (int idx = 0; idx < testInven.Count; idx++) // 시작전에 DataManager에 있는 숫자만큼 인벤토리를 채움
        {
            Slot slot = Slots[idx];// 슬롯 가져옴

           // slot.gameObject.SetActive(true); // 활성화
            slot.inItem = true;
            slot.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Items/{testInven[idx].Name}"); // 해당 슬롯에 이미지를 바꿈
            slot._itemInfo.Name = testInven[idx].Name; // 아이템 이름 설정
        }


    }

    void Start()
    {
        Managers.Input.MouseAction -= Sell;
        Managers.Input.MouseAction += Sell;
    }

    private void Update()
    {

        _goldText.text = Managers.Data.Gold.ToString(); // 수정 예정
    }


    public bool AddItem(Contents.Item item) // 아이템을 먹었을때
    {
        int idx = 0;
        foreach (var checkSlot in Slots)
        {
            if (checkSlot._itemInfo.Id!=0)
                idx++;
            else break;
        }

        if (idx > inventorSize)
        {
            Debug.LogError("인벤토리가 꽉 찼습니다");
            return false ;
        }
        Managers.Data.InventoryDataChange(idx, item);

        Slot slot = Slots[idx];// 슬롯 가져옴
        slot.gameObject.SetActive(true); // 활성화
        slot.inItem = true; // 아이템이 들어갔음
        slot.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Items/{item.Name}"); // 해당 슬롯에 이미지를 바꿈
        slot._itemInfo.Name = item.Name; // 아이템 정보에 이름 전달

        return true;
    }

    public void Sell(Define.MouseState evt)
    {
        if (GameObject.FindObjectOfType<TownScene>() == null) // 타운이 아니라면 리턴
            return;
        if (evt == Define.MouseState.RButtonDown && GameObject.FindObjectOfType<TownScene>().NPCUI.activeSelf) // 해당 evt가 우클릭이고 상점 켜져있고
        {
            if (!clickInven) return;
            Slot sellSlot = Slots[sellSlotIdx]; // 해당 판매할 오브젝트

            if (sellSlot && sellSlot.inItem && sellSlot.gameObject.layer == (int)Define.UI.Inventory) // null 체크
            {
                sellSlot.GetComponent<Image>().sprite = Resources.Load<Sprite>("items/emptySlot"); // 빈 슬롯 이미지로 변경
                sellSlot._itemInfo.Name = "emptySlot";
                sellSlot.inItem = false;
                Managers.Data.InventoryDataChange(sellSlotIdx,new Contents.Item(),false); // 해당 인덱스 아이템 삭제하고 해당 슬롯 빈 상태로 
            }

            if (toolTip.gameObject.activeSelf)
            {
                toolTip.gameObject.SetActive(false);
            }
        }
    }

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    // 레이어가 인벤토리이고 클린된 곳의 태그가 슬롯이라면 ( 굳이 슬롯에도 태그를 넣어야할까?)
    //    if (eventData.button != PointerEventData.InputButton.Left)
    //        return;

    //    if (eventData.pointerPressRaycast.gameObject.CompareTag("Slot") &&
    //        eventData.pointerPressRaycast.gameObject.layer == (int)Define.UI.Inventory)
    //    {
    //        clickInven = true;
    //        Slot tempSlot = eventData.pointerPressRaycast.gameObject.GetComponent<Slot>();
    //        if (tempSlot.inItem)
    //        {
    //            if (GameObject.FindObjectOfType<TownScene>() == null) return; // 타운이 아니라면

    //            if (GameObject.FindObjectOfType<TownScene>().NPCUI.activeSelf)
    //            {
    //                toolTip.sellOrPurchase.text = "우클릭 판매"; // 인벤토리 텍스 판매로 변경
    //            }
    //            else
    //            {
    //                toolTip.sellOrPurchase.text = ""; // 상점이 안켜져있을때
    //            }

    //            toolTip.gameObject.SetActive(true); // 툴팁 활성화
    //            toolTip.SetItemInfo(tempSlot._itemInfo.Name); // 툴팁에 해당 슬롯 아이템 정보 설정
    //            sellSlotIdx = tempSlot.transform.GetSiblingIndex();
    //        }

    //    }
    //    else
    //    {
    //        sellSlotIdx = -1;
    //    }
    //}

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (!eventData.pointerPressRaycast.gameObject.CompareTag("Slot") ||
            eventData.pointerPressRaycast.gameObject.layer != (int)Define.UI.Inventory)
            return;

        clickInven = true;

        Slot tempSlot = eventData.pointerPressRaycast.gameObject.GetComponent<Slot>();
        if (!tempSlot.inItem)
            return;

        TownScene townScene = GameObject.FindObjectOfType<TownScene>();
        if (townScene == null)
        {
            toolTip.gameObject.SetActive(true);
            toolTip.SetItemInfo(tempSlot._itemInfo.Name);
            toolTip.sellOrPurchase.text = "";
            return;
        }

        toolTip.gameObject.SetActive(true);
        toolTip.SetItemInfo(tempSlot._itemInfo.Name);
        sellSlotIdx = tempSlot.transform.GetSiblingIndex();

        if (townScene.NPCUI.activeSelf)
        {
            toolTip.sellOrPurchase.text = "우클릭 판매";
        }
        else
        {
            toolTip.sellOrPurchase.text = "";
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.gameObject.SetActive(false);
        clickInven = false;
    }
}
