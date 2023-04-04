using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{

    public enum EnemyType
    {
        Skelton = 500,
    }

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
        LButtonDown,
        ButtonUp,
        Click,
        RButtonDown
    }

    public enum CursorType
    {
        Arrow, // Idle
        Attack,
    }

    public enum ItemType
    {
        Equipment,
        Used
    }

    public enum UI
    {
        Inventory = 14,
        Shop,
        Stat,
    }

    public enum Coin
    {
        Gold,
        Sliver,
        Bronze
    }

}



