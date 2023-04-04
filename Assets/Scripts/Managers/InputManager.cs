using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class InputManager
{

    public Action KeyAction = null;
    public Action<Define.MouseState> MouseAction = null;
    public Action<Define.UI> KeyboardAction = null;


    bool _press = false;
    float _pressedTime = 0;

    public void MouseUpdate()
    {
        if (Input.anyKey && KeyAction != null)
        {
            KeyAction.Invoke();
        }

        if (MouseAction != null)
        {
            if (Input.GetMouseButton(0))
            {
                if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() == true) // UI �����ٸ� ���� 
                    return;
                if (!_press)
                {
                    MouseAction.Invoke(Define.MouseState.LButtonDown); 
                    _pressedTime = Time.time;
                }
                MouseAction.Invoke(Define.MouseState.Press);
                _press = true;
            }
            else if (Input.GetMouseButton(1))
            {
                if (!_press)
                {
                    MouseAction.Invoke(Define.MouseState.RButtonDown);
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

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Managers.Data.PlayerData.location = SceneManager.GetActiveScene().buildIndex;
                Managers.Data.PlayerDataChange();
                Application.Quit();
            }
        }
    }

    public void Clear() // �� �̵��� �ʱ�ȭ
    {
        KeyAction = null;
        MouseAction = null;
        KeyboardAction = null;
    }
}