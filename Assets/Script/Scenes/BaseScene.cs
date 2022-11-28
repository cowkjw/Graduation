using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScene : MonoBehaviour
{
    
    protected Vector3 _playerPos;
    protected GameObject _player =null;

    void Awake()
    {
        Init();
    }

    public virtual void Init()
    {

        Managers.Clear(); // 备刀沁带芭 null贸府
        Managers.Input.KeyboardAction -= InputUIHotKey;
        Managers.Input.KeyboardAction += InputUIHotKey;
    }

    void InputUIHotKey(Define.UI uiType)
    {
        if (uiType == Define.UI.Inventory)
        {
            GameObject inventory = GameObject.Find("UI").transform.Find("Inventory").gameObject;
            if (inventory.activeSelf == true)
            {
                inventory.SetActive(false);

            }
            else if (inventory.activeSelf == false)
            {
                inventory.SetActive(true);

            }
        }
    }
}
