using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{

    Texture2D _idleIcon;
    Texture2D _attackIcon;
    public GameObject _clickEffect;

    Ray ray;
    RaycastHit hit;
    bool raycastHit;

    int _mask = (1 << 6) | (1 << 8)|(1<<7);


    Define.CursorType _cursorType = Define.CursorType.Arrow;

    void Start()
    {

        _idleIcon = (Texture2D)Resources.Load("Texture/Cursor_Basic"); // Texture2D 타입캐스팅
        _attackIcon = (Texture2D)Resources.Load("Texture/Cursor_Shoot");
        Cursor.SetCursor(_idleIcon, new Vector2(_idleIcon.width / 5, 0), CursorMode.Auto);

        Managers.Input.MouseAction -= MousePointEffect;
        Managers.Input.MouseAction += MousePointEffect;
    }


    void MousePointEffect(Define.MouseState evt)
    {
        if (evt == Define.MouseState.Press)
            return;

        if (hit.collider.gameObject.layer == 6 && evt == Define.MouseState.ButtonDown) // 땅일때만 표시
        {
            GameObject clickParticle = Instantiate(_clickEffect);
            clickParticle.transform.position = hit.point;
            Destroy(clickParticle, 0.5f);
        }
              
    }

    void Update()
    {

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        raycastHit = Physics.Raycast(ray, out hit, 100.0f, _mask);

        if (raycastHit)
        {
            if (hit.collider.gameObject.layer == 8)
            {


                if (_cursorType != Define.CursorType.Attack)
                {
                    Cursor.SetCursor(_attackIcon, new Vector2(_attackIcon.width / 5, 0), CursorMode.Auto);
                    _cursorType = Define.CursorType.Attack;
                }

            }
            else
            {
                if (_cursorType != Define.CursorType.Arrow)

                {
                    Cursor.SetCursor(_idleIcon, new Vector2(_idleIcon.width / 5, 0), CursorMode.Auto);
                    _cursorType = Define.CursorType.Arrow;
                }
            }

        }
    }

}
