using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
    public Sequence(string name)
    {
        _name = name;
    }

    public override Status Process()
    {
        Status childStatus = base.Process();

        if (childStatus == Status.Failure)
        {
            _currentChildIndex = 0;
            return Status.Failure;
        }
        
        if (childStatus == Status.Success)
        {
            _currentChildIndex++;
            if (_currentChildIndex >= _childrenNodes.Count)
            {
                _currentChildIndex = 0;
                return Status.Success;
            }
        }

        return Status.Running;
    }
}
