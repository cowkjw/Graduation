using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ConditionNode : INode
{
    Func<bool> condition; // 참인지 확인

    public ConditionNode(Func<bool> condition)
    {
        this.condition = condition;
    }

    public bool Execute()
    {
        return condition();
    }
}
