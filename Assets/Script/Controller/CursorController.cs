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
        _idleCursor = (Texture2D)Resources.Load("Textures/Cursor_Basic"); // Texture2D Ÿ��ĳ����
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
        if (hit.collider.gameObject.transform.root.gameObject.layer == 3) // �ش� ������Ʈ�� �θ� Player��� (�÷��̾ ������ �ִ� �������̹Ƿ� ����)
            return;
        // �������̶��
        if (hit.collider.gameObject.layer == 11 && evt == Define.MouseState.LButtonDown)
        {
            Vector3 dis = hit.collider.transform.position - Managers.Game.Player.transform.position;
            if (dis.magnitude > 2f)
            {
                return;
            }
            //////////////////�ٽ� �����ϱ�
            int itemID = hit.collider.GetComponent<Item>().Id;
            if (Managers.Data.ItemDict.TryGetValue(itemID, out Contents.Item tempItem))
            {
         
                if (_inventory.AddItem(tempItem))
                    Destroy(hit.collider.gameObject);
            }
            else
            {
                Debug.LogError("�������� �ʴ� ������ ���̵��Դϴ�.");
            }
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
            Cursor.SetCursor(hit.collider.gameObject.layer == 8 ? _attackCursor : _idleCursor, new Vector2(_idleCursor.width / 5, 0), CursorMode.Auto);
            _cursorType = hit.collider.gameObject.layer == 8 ? Define.CursorType.Attack : Define.CursorType.Arrow;
        }
    }

    void Update()
    {
        SetCursorIcon(); // �� �Լ����� ����ؼ� ray�� �� hit�� �ٲ�
    }


}
