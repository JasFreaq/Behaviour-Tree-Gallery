using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : Node
{
    public Selector(string name)
    {
        _name = name;
    }

    public override Status Process()
    {
        Status childStatus = base.Process();

        if (childStatus == Status.Success)
        {
            _currentChildIndex = 0;
            return Status.Success;
        }

        if (childStatus == Status.Failure)
        {
            _currentChildIndex++;
            if (_currentChildIndex >= _childrenNodes.Count)
            {
                _currentChildIndex = 0;
                return Status.Failure;
            }
        }

        return Status.Running;
    }
}
