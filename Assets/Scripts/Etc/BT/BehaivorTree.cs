using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public interface INode
{
    bool Execute();
}

public class BehaviorTree
{
    INode rootNode;

    public void SetRootNode(INode rootNode)
    {
        this.rootNode = rootNode;
    }

    public void Update()
    {
        rootNode.Execute();
    }
}
