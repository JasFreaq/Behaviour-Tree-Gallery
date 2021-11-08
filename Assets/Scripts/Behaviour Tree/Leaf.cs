using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : Node
{
    private Func<Status> _tick;

    public Leaf() { }

    public Leaf(string name, Func<Status> tick)
    {
        _name = name;
        _tick = tick;
    }

    public override Status Process()
    {
        return _tick?.Invoke() ?? Status.Failure;
    }
}
