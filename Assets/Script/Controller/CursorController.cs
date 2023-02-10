using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CursorController : MonoBehaviour
{

    Texture2D _idleCursor;
    Texture2D _attackCursor;
    public GameObject _clickEffect;


    Ray ray;
    RaycastHit hit;
    bool raycastHit;
    int _mask = (1 << 6) | (1 << 8) | (1 << 7) | (1 << 11) | (1 << 9) | (1 << 10) | (1 << 12);

    InventoryController _inventory;

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
        ClickEffect(evt);
        ClickItem(evt);
    }


    void ClickEffect(Define.MouseState evt)
    {
        if (!hit.collider)
            return;

        int hitLayer = hit.collider.gameObject.layer;
        if (hitLayer == 8 || hitLayer == 11)
            return;

        if (evt == Define.MouseState.LButtonDown)
        {
            GameObject clickParticle = Instantiate(_clickEffect, hit.point, Quaternion.identity);
            Destroy(clickParticle, 0.5f);
        }
    }

    void ClickItem(Define.MouseState evt)
    {
        if (hit.collider == null)
            return;
        // 아이템이라면
        if (hit.collider.gameObject.layer == 11 && evt == Define.MouseState.LButtonDown)
        {
            Vector3 dis = hit.collider.transform.position - Managers.Game._Player.transform.position;
            if (dis.magnitude > 2f)
            {
                return;
            }
            //////////////////다시 수정하기
            Item item = hit.collider.GetComponent<Item>();
            Contents.Item tempItem = new Contents.Item();
            tempItem.Name = item.Name;
            tempItem.Id = item.Id;
            tempItem.ItemType = item.ItemType;
            if (_inventory.AddItem(tempItem))
                Destroy(hit.collider.gameObject);
            //_inventory.AchiveItem(hit.collider.GetComponent<Item>());
            ////////////////////////////

        }

    }

    void SetCursorIcon()
    {

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        raycastHit = Physics.Raycast(ray, out hit, 100.0f, _mask);

        if (raycastHit)
        {
            if (hit.collider.gameObject.layer == 8)
            {


                if (_cursorType != Define.CursorType.Attack)
                {
                    Cursor.SetCursor(_attackCursor, new Vector2(_attackCursor.width / 5, 0), CursorMode.Auto);
                    _cursorType = Define.CursorType.Attack;
                }

            }
            else
            {
                if (_cursorType != Define.CursorType.Arrow)

                {
                    Cursor.SetCursor(_idleCursor, new Vector2(_idleCursor.width / 5, 0), CursorMode.Auto);
                    _cursorType = Define.CursorType.Arrow;
                }
            }

        }
    }

    void Update()
    {
        SetCursorIcon(); // 이 함수에서 계속해서 ray를 쏴 hit을 바꿈
    }


}
