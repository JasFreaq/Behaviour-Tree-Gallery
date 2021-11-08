using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    #region Associated Data Structure(s)

    public enum Status
    {
        Failure,
        Running,
        Success
    }

    #endregion

    private Status _nodeStatus;
    protected List<Node> _childrenNodes = new List<Node>();
    protected int _currentChildIndex;

    protected string _name;
    
    public Node() { }

    public Node(string name)
    {
        _name = name;
    }
    
    public IReadOnlyList<Node> ChildrenNodes
    {
        get { return _childrenNodes; }
    }

    public string Name
    {
        get { return _name; }
    }

    public void AddChild(Node child)
    {
        _childrenNodes.Add(child);
    }

    public virtual Status Process()
    {
        return _childrenNodes[_currentChildIndex].Process();
    }
}
