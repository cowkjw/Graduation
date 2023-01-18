using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{

    public Action KeyAction = null;
    public Action<Define.MouseState> MouseAction = null;
    public Action<Define.UI> KeyboardAction = null;


    bool _press = false;
    float _pressedTime = 0;

    public void MouseUpdate()
    {

        SellPurchase();
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() == true) // UI �����ٸ� ���� 
            return;
        //if (Input.anyKey && KeyAction != null)
        //{
        //    KeyAction.Invoke();
        //}

        if (MouseAction != null)
        {
            if (Input.GetMouseButton(0))
            {
                if (!_press)
                {
                    MouseAction.Invoke(Define.MouseState.ButtonDown);
                    _pressedTime = Time.time;
                }
                MouseAction.Invoke(Define.MouseState.Press);
                _press = true;
            }
            else
            {
                if (_press)
                {
                    if (Time.time < _pressedTime + 0.3f) // ���� �ð��� ���� 
                    {
                        MouseAction.Invoke(Define.MouseState.Click);
                    }
                    MouseAction.Invoke(Define.MouseState.ButtonUp);
                }

                //�ʱ�ȭ
                _press = false;
                _pressedTime = 0;
            }

        }
    }

    public void KeyboardUpdate()
    {
        if (Input.anyKey && KeyAction != null)
        {
            KeyAction.Invoke();
        }
        if (KeyboardAction != null)
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                KeyboardAction.Invoke(Define.UI.Inventory);
            }
        }
    }

    public void SellPurchase() // ������ �κ��丮 �Ǹ� ���� �Է� �Ǵ� �Լ�
    {
        if (Input.anyKey && KeyAction != null)
        {
            KeyAction.Invoke();
        }

        if (Input.GetMouseButton(1))
        {
            MouseAction.Invoke(Define.MouseState.RButtonDown);
        }
    }
    public void Clear() // �� �̵��� �ʱ�ȭ
    {
        KeyAction = null;
        MouseAction = null;
        KeyboardAction = null;
    }
}
