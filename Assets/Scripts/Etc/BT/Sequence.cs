using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : INode
{
    List<INode> children = new List<INode>();

    public void AddChild(INode child)
    {
        children.Add(child);
    }

    public bool Execute()
    {
        foreach (INode child in children)
        {
            if (!child.Execute())
            {
                return false;
            }
        }
        return true;
    }
}