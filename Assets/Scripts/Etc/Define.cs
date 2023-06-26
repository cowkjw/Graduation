using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{

    public enum EnemyType
    {
        Skelton = 500,
        Boss
    }

    public enum State
    {
        Idle,
        Moving,
        Attack,
        CrowdControl,
        Skill,
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
        Arrow, // ±âº»
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

    public enum Skill
    {
        A,
        B,
        C
    }

    public enum SkillType
    {
        Passive,
        Attack,
    }

    public enum Scene
    {
        Town,
        Dungeon,
        BossDungeon,
        Base
    }

}



