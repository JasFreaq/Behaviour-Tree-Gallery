using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node
{
    #region Associated Data Structure(s)

    public enum Status
    {
        Running,
        Failure,
        Success
    }

    #endregion

    protected List<Node> _childrenNodes = new List<Node>();
    protected int _currentChildIndex;
    protected float _priority;
    protected bool _invert;

    protected string _name;
    
    public Node() { }

    public Node(string name, float priority = 0)
    {
        _name = name;
        _priority = priority;
    }

    public Node(string name, bool invert, float priority = 0)
    {
        _name = name;
        _invert = invert;
        _priority = priority;
    }

    public IReadOnlyList<Node> ChildrenNodes
    {
        get { return _childrenNodes; }
    }

    public float Priority
    {
        get { return _priority; }
    }

    public string Name
    {
        get { return _name; }
    }

    public virtual void AddChild(Node child)
    {
        _childrenNodes.Add(child);
    }

    public virtual Status Process()
    {
        if (_childrenNodes.Count == 0)
            return Status.Success;
        
        return _childrenNodes[_currentChildIndex].Process();
    }

    public void ChangePriority(float newPriority)
    {
        _priority = newPriority;
    }

    public void Reset()
    {
        foreach(Node node in _childrenNodes)
            node.Reset();

        _currentChildIndex = 0;
    }

    protected Status InvertStatus(Status status)
    {
        if (status == Status.Failure)
            return Status.Success;

        if (status == Status.Success)
            return Status.Failure;

        return Status.Running;
    }
}
