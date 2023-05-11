using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class InventoryController : MonoBehaviour/*, IPointerDownHandler//, IPointerExitHandler*/
{

    public Slot[] Slots;
    Text _goldText;
    BaseScene _baseScene;

    void Start()
    {
        Init();
    }

    private void Update()
    {
        _goldText.text = Managers.Data.Gold.ToString(); // 수정 예정
    }

    void Init()
    {
        _goldText = transform.GetChild(1).GetChild(0).GetComponent<Text>();
        _baseScene = FindObjectOfType<BaseScene>();
        for (int i = 0; i < Slots.Length; i++)
        {
            Slot slot = Slots[i];

            if (Managers.Data.InvenDict.ContainsKey(i))
            {
                slot.PutInItem(Managers.Data.InvenDict[i]);
            }
            else
            {
                slot.ClearSlot();
            }

        }
    }

    public bool AddItem(Contents.Item item) // 아이템을 먹었을때
    {

        int idx = Array.FindIndex(Slots, slot => !slot.inItem); // 람다식 사용
        if (idx < 0) // 없으면 -1 반환하기 때문
        {
#if UNITY_EDITOR
            Debug.LogError("인벤토리 꽉참");
#endif
            return false;
        }
        Managers.Data.UpdateInventoryData(idx, item);
        Slot slot = Slots[idx];
        slot.gameObject.SetActive(true);
        slot.PutInItem(item);

        return true;
    }
}
