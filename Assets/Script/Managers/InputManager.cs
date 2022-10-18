using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    public Action KeyAction = null;
    public Action<Define.MouseState> MouseAction = null;

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
                    if (Time.time < _pressedTime + 0.3f) // 현재 시간이 누른 
                    {
                     
                        MouseAction.Invoke(Define.MouseState.Click);
                    }
                    MouseAction.Invoke(Define.MouseState.ButtonUp);
                }

                //초기화
                _press = false;
                _pressedTime = 0;
            }

        }
    }
}
