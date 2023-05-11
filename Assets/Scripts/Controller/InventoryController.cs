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
        _goldText.text = Managers.Data.Gold.ToString(); // ���� ����
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

    public bool AddItem(Contents.Item item) // �������� �Ծ�����
    {

        int idx = Array.FindIndex(Slots, slot => !slot.inItem); // ���ٽ� ���
        if (idx < 0) // ������ -1 ��ȯ�ϱ� ����
        {
#if UNITY_EDITOR
            Debug.LogError("�κ��丮 ����");
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
