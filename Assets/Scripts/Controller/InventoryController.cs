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
    int selectSlotIdx;
    bool clickInven = false;
    public ItemTooltip toolTip;
    Text _goldText;
    WeaponChangeController weaponSocket;
    BaseScene baseScene;

    void Start()
    {
        Managers.Input.MouseAction -= Sell;
        Managers.Input.MouseAction += Sell;
        Managers.Input.MouseAction -= Equip;
        Managers.Input.MouseAction += Equip;
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
        weaponSocket = Managers.Game.GetPlayer().GetComponentInChildren<WeaponChangeController>();// 웨폰 소켓 찾기
        baseScene = FindObjectOfType<BaseScene>();

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

        Managers.Data.UpdateInventoryData(idx, item);

        Slot slot = Slots[idx];
        slot.gameObject.SetActive(true);
        slot.PutInItem(item);

        return true;
    }

    public void Sell(Define.MouseState evt)
    {
        if (GameObject.FindObjectOfType<TownScene>() == null) // 타운이 아니라면 리턴
            return;
        if (evt != Define.MouseState.RButtonDown || !GameObject.FindObjectOfType<TownScene>().NPCUI.activeSelf) // 해당 evt가 우클릭이고 상점 켜져있고
            return;
        if (!clickInven) return;
        if (selectSlotIdx == -1) return;

        Slot sellSlot = Slots[selectSlotIdx].GetComponent<Slot>(); // 해당 판매할 오브젝트

        if (sellSlot?.inItem == true && sellSlot.gameObject.layer == (int)Define.UI.Inventory) // null 체크
        {

            sellSlot.GetComponent<Image>().sprite = Resources.Load<Sprite>("items/emptySlot"); // 빈 슬롯 이미지로 변경
            sellSlot.inItem = false;
            Managers.Data.Gold += sellSlot.ItemInfo.SellPrice; // 판매 했으니까 판매가격만큼 골드 올리기
            Managers.Data.UpdateInventoryData(selectSlotIdx, default, false); // 해당 인덱스 아이템 삭제하고 해당 슬롯 빈 상태로 
        }
        if (toolTip.gameObject.activeSelf)
        {
            toolTip.gameObject.SetActive(false);
        }
    }

    public void Equip(Define.MouseState evt) // 장비 장착
    {
        if (evt != Define.MouseState.RButtonDown) // 해당 evt가 우클릭이고 상점 켜져있고
            return;
        if (!clickInven) return;
        if (selectSlotIdx == -1) return;

        if (baseScene is TownScene townScene && townScene.NPCUI.activeSelf)
        {
            return;
        }

        Slot equipSlot = Slots[selectSlotIdx].GetComponent<Slot>(); // 장착할 Slot

        if (equipSlot.ItemInfo.ItemType == Define.ItemType.Equipment) // 장착하는 아이템 타입이 장비라면
        {
            int currentItemId = Managers.Data.PlayerData.equippedWeapon; // 현재 장착 무기 ID 
            weaponSocket?.ChangeWeapon(equipSlot.ItemInfo.Id); // 만약 널이 아니라면 불러와서 해당 아이템 ID로 변경
            if (Managers.Data.ItemDict.TryGetValue(currentItemId, out Contents.Item currentEquipItem)) // Item table에서 해제한 장비 정보 가져옴
            {
                Managers.Game.GetPlayer().GetComponent<PlayerStat>().Attack -= currentEquipItem.Attack; // 현재 장착 중인 무기 추가 공격력 빼기
                Debug.Log(currentEquipItem.Attack);
                equipSlot.PutInItem(currentEquipItem);// 현재 장착한 아이템을 선택한 슬롯으로 넣기 
            }
                Debug.Log(equipSlot.ItemInfo.Attack);
            Managers.Data.UpdateInventoryData(selectSlotIdx, currentEquipItem, true); // 해당 인덱스 아이템 변경 및 빈 슬롯으로 교체 
        }

    }

    public void OnPointerDown(PointerEventData eventData)
    {

        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        GameObject obj = eventData.pointerPressRaycast.gameObject;

        if (!obj.CompareTag("Slot") || obj.layer != (int)Define.UI.Inventory)
            return;

        if (baseScene == null) return;

        clickInven = true;

        Slot tempSlot = obj.GetComponent<Slot>();
        if (!tempSlot.inItem)
            return;

        if (GameObject.FindObjectOfType<TownScene>() != null && GameObject.FindObjectOfType<TownScene>().NPCUI.activeSelf)
        {
            toolTip.sellOrPurchase.text = "우클릭 판매"; // 인벤토리 텍스 판매로 변경
        }
        else
        {
            toolTip.sellOrPurchase.text = "우클릭 장착"; // 상점이 안켜져있을때
        }

        toolTip.gameObject.SetActive(true); // 툴팁 활성화
        toolTip.SetItemInfo(tempSlot.ItemInfo.Name); // 툴팁에 해당 슬롯 아이템 정보 설정
        selectSlotIdx = tempSlot.transform.GetSiblingIndex();
    }




    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.gameObject.SetActive(false);
        clickInven = false;
        selectSlotIdx = -1;
    }

}
