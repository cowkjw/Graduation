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
    int _mask = (1 << 6) | (1 << 8)| (1<<7);


    Define.CursorType _cursorType = Define.CursorType.Arrow;

    void Start()
    {

        _idleCursor = (Texture2D)Resources.Load("Texture/Cursor_Basic"); // Texture2D 타입캐스팅
        _attackCursor = (Texture2D)Resources.Load("Texture/Cursor_Attack");
        Cursor.SetCursor(_idleCursor, new Vector2(_idleCursor.width / 5, 0), CursorMode.Auto);

        Managers.Input.MouseAction -= MousePointEffect;
        Managers.Input.MouseAction += MousePointEffect;
    }


    void MousePointEffect(Define.MouseState evt)
    {

        if (hit.collider.gameObject.layer == 8)// 몬스터라면 포인터를 생성 x
            return;
        if (evt == Define.MouseState.ButtonDown && hit.collider.gameObject != null) // 땅일때만 표시
        {

            GameObject clickParticle = Instantiate(_clickEffect);
            clickParticle.transform.position = hit.point;
            if(clickParticle!=null)
                Destroy(clickParticle, 0.5f);
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
