using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
    public Sequence(string name, int priority = 0)
    {
        _name = name;
        _priority = priority;
    }
    
    public Sequence(string name, bool invert, int priority = 0)
    {
        _name = name;
        _invert = invert;
        _priority = priority;
    }

    public override Status Process()
    {
        Status status = Status.Running;
        Status childStatus = base.Process();

        if (childStatus == Status.Failure)
        {
            _currentChildIndex = 0;
            status = Status.Failure;
        }
        else if (childStatus == Status.Success)
        {
            _currentChildIndex++;
            if (_currentChildIndex >= _childrenNodes.Count)
            {
                _currentChildIndex = 0;
                status = Status.Success;
            }
        }

        if (_invert)
            return InvertStatus(status);

        return status;
    }
}
