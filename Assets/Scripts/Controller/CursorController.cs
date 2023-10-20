using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CursorController : MonoBehaviour
{
    public GameObject ClickEffect;
    Texture2D _idleCursor;
    Texture2D _attackCursor;
    InventoryController _inventory;
    Ray _ray;
    RaycastHit _hit;
    bool _raycastHit;
    int _mask = (1 << 6) | (1 << 7) | (1 << 8) | (1 << 9) | (1 << 10) | (1 << 11) | (1 << 12); // Ground, Wall, Enemy, Dungeon1, Town, item, StoreNPC
    Define.CursorType _cursorType = Define.CursorType.Arrow;

    void Start()
    {
        _idleCursor = (Texture2D)Resources.Load("Textures/Cursor_Basic"); // Texture2D 타입캐스팅
        _attackCursor = (Texture2D)Resources.Load("Textures/Cursor_Attack");
        _inventory = GameObject.Find("UI").transform.Find("Inventory").GetComponent<InventoryController>();
        Cursor.SetCursor(_idleCursor, new Vector2(_idleCursor.width / 5, 0), CursorMode.Auto);

        Managers.Input.MouseAction -= MousePointEvent;
        Managers.Input.MouseAction += MousePointEvent;
    }

    void MousePointEvent(Define.MouseState evt)
    {
        ClickPointerEffect(evt);
        ClickItem(evt);
    }

    void ClickPointerEffect(Define.MouseState evt)
    {
        if (!_hit.collider)
            return;

        int hitLayer = _hit.collider.gameObject.layer;
        if (hitLayer == 8 || hitLayer == 11||hitLayer == 7)
            return;

        if (evt == Define.MouseState.LButtonDown)
        {
            GameObject clickParticle = Instantiate(ClickEffect, _hit.point, Quaternion.identity);
            Destroy(clickParticle, 0.5f);
        }
    }

    void ClickItem(Define.MouseState evt)
    {
        if (_hit.collider == null)
            return;
        if (_hit.collider.transform.root.gameObject.layer == 3) // 해당 오브젝트의 부모가 Player라면 (플레이어가 가지고 있는 아이템이므로 무시)
            return;
        // 아이템이라면
        if (_hit.collider.gameObject.layer == 11 && evt == Define.MouseState.LButtonDown)
        {
            Vector3 dis = _hit.collider.transform.position - Managers.Game.GetPlayer().transform.position;
            if (dis.magnitude > 2f)
            {
                return;
            }
            int itemID = _hit.collider.GetComponent<Item>().Id;
            try
            {
                Contents.Item tempItem = Managers.Data.ItemDict[itemID];
                if (_inventory.AddItem(tempItem))
                    Destroy(_hit.collider.gameObject);
            }
            catch (KeyNotFoundException)
            {
#if UNITY_EDITOR
                Debug.LogError("존재하지 않는 아이템 아이디입니다.");
#endif
            }
        }
    }

    void SetCursorIcon()
    {
        _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        _raycastHit = Physics.Raycast(_ray, out _hit, 100.0f, _mask);

        if (_raycastHit)
        {
            Cursor.SetCursor(_hit.collider.gameObject.layer == 8 ? _attackCursor : _idleCursor, new Vector2(_idleCursor.width / 5, 0), CursorMode.Auto);
            _cursorType = _hit.collider.gameObject.layer == 8 ? Define.CursorType.Attack : Define.CursorType.Arrow;
        }
    }

    void Update()
    {
        SetCursorIcon(); // 이 함수에서 계속해서 ray를 쏴 hit을 바꿈
    }
}
