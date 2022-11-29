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
    int _mask = (1 << 6) | (1 << 8)| (1<<7)|(1<<11);

    InventoryController _inventory;

    Define.CursorType _cursorType = Define.CursorType.Arrow;

    void Start()
    {
        _inventory = GameObject.Find("UI").transform.Find("Inventory").GetComponent<InventoryController>();
        _idleCursor = (Texture2D)Resources.Load("Texture/Cursor_Basic"); // Texture2D 타입캐스팅
        _attackCursor = (Texture2D)Resources.Load("Texture/Cursor_Attack");
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
        if (hit.collider == null)
            return;
        if (hit.collider.gameObject.layer == 8 || hit.collider.gameObject.layer == 11)// 몬스터라면 포인터를 생성 x
            return;

        if (evt == Define.MouseState.ButtonDown && hit.collider.gameObject != null)
        {
            GameObject clickParticle = Instantiate(_clickEffect);
            clickParticle.transform.position = hit.point;
            if (clickParticle != null)
                Destroy(clickParticle, 0.5f);
        }
    }

    void ClickItem(Define.MouseState evt)
    {
        if (hit.collider == null)
            return;
        // 아이템이라면
        if (hit.collider.gameObject.layer == 11&&evt == Define.MouseState.Click)
        {
            Vector3 dis = hit.collider.transform.position - Managers.game._Player.transform.position;
            if (dis.magnitude<=2f)
            {
                if(Managers.Data.InventoryCount<16)
                {
                    _inventory.AchiveItem(hit.collider.name);
                    Destroy(hit.collider.gameObject);

                }
            }
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
