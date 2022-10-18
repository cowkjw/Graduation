using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{

    public enum State
    {
        Idle,
        Moving,
        Attack,
        Die,
    }

    public enum MouseState
    {
        Press,
        ButtonDown,
        ButtonUp,
        Click
    }

    public enum CursorType
    {
        Arrow, // Idle
        Attack,
    }


}
    


