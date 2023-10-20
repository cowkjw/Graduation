using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActionNode : INode
{
     Action action;

    public ActionNode(Action action)
    {
        this.action = action;
    }

    public bool Execute()
    {
        action();
        return true;
    }
}