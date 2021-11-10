using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : Node
{
    private Func<Status> _tick;

    public Leaf() { }

    public Leaf(string name, Func<Status> tick, int priority = 0)
    {
        _name = name;
        _tick = tick;
        _priority = priority;
    }
    
    public Leaf(string name, Func<Status> tick, bool invert, int priority = 0)
    {
        _name = name;
        _tick = tick;
        _invert = invert;
        _priority = priority;
    }

    public override Status Process()
    {
        Status status = _tick?.Invoke() ?? Status.Failure;

        if (_invert)
            return InvertStatus(status);

        return status;
    }
}
