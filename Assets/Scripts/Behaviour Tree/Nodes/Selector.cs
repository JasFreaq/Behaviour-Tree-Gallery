using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : Node
{
    public Selector(string name, float priority = 0)
    {
        _name = name;
        _priority = priority;
    }
    
    public Selector(string name, bool invert, float priority = 0)
    {
        _name = name;
        _invert = invert;
        _priority = priority;
    }

    public override Status Process()
    {
        Status status = Status.Running;
        Status childStatus = base.Process();

        if (childStatus == Status.Success)
        {
            Reset();
            status = Status.Success;
        }
        else if (childStatus == Status.Failure)
        {
            _currentChildIndex++;
            if (_currentChildIndex >= _childrenNodes.Count)
            {
                Reset();
                status = Status.Failure;
            }
        }

        if (_invert)
            return InvertStatus(status);

        return status;
    }
}
