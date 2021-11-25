using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loop : Node
{
    private Node _dependancy;

    public Loop(string name, Node dependancy, float priority = 0) : base(name, priority)
    {
        _dependancy = dependancy;
    }

    public Loop(string name, Node dependancy, bool invert, float priority = 0)
        : base(name, invert, priority)
    {
        _dependancy = dependancy;
    }

    public override Status Process()
    {
        if (_dependancy.Process() == Status.Failure)
            return Status.Success;

        Status status = Status.Running;
        Status childStatus = base.Process();

        if (childStatus == Status.Failure)
        {
            Reset();
            status = Status.Failure;
        }
        else if (childStatus == Status.Success)
        {
            _currentChildIndex++;
            if (_currentChildIndex >= _childrenNodes.Count)
                _currentChildIndex = 0;
        }

        if (_invert)
            return InvertStatus(status);

        return status;
    }
}
