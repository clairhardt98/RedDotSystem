using System.Collections.Generic;
using System.Collections;

public class RedDotNode
{
    RedDot _redDotRef;
    BitArray _childActivationBitArray;
    public List<RedDotNode> Children { get; set; }

    public RedDotNode(RedDot redDotref)
    {
        _redDotRef = redDotref;
        Children = new();
    }

    public void AddChild(RedDotNode childNode)
    {
        Children.Add(childNode);
    }

    public void RemoveChild(RedDotNode childNode)
    {
        Children.Remove(childNode);
    }
}

public class RedDotTree
{

}
