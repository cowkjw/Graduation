using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        _inventory = GameObject.FindObjectOfType<InventoryController>();
        _idleCursor = (Texture2D)Resources.Load("Texture/Cursor_Basic"); // Texture2D 타입캐스팅
        _attackCursor = (Texture2D)Resources.Load("Texture/Cursor_Attack");
        Cursor.SetCursor(_idleCursor, new Vector2(_idleCursor.width / 5, 0), CursorMode.Auto);

        Managers.Input.MouseAction -= MousePointEvent;
        Managers.Input.MouseAction += MousePointEvent;
    }


    void MousePointEvent(Define.MouseState evt)
    {

        //if (hit.collider.gameObject.layer == 8)// 몬스터라면 포인터를 생성 x
        //    return;
        //if (evt == Define.MouseState.ButtonDown && hit.collider.gameObject != null) // 땅일때만 표시
        //{

        //    GameObject clickParticle = Instantiate(_clickEffect);
        //    clickParticle.transform.position = hit.point;
        //    if(clickParticle!=null)
        //        Destroy(clickParticle, 0.5f);
        //}
        ClickEffect(evt);
        ClickItem(evt);


    }

    void ClickEffect(Define.MouseState evt)
    {
        if (hit.collider.gameObject.layer == 8||hit.collider.gameObject.layer==11)// 몬스터라면 포인터를 생성 x
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
        // 아이템이라면
        if (hit.collider.gameObject.layer == 11&&evt == Define.MouseState.Click)
        {
            Vector3 dis = hit.collider.transform.position - Managers.game._Player.transform.position;
            if (dis.magnitude<=2f)
            {
                _inventory.AchiveItem();
             //   Destroy(hit.collider.gameObject);
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
        SetCursorIcon();
    }

}
