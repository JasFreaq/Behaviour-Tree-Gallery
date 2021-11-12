using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Leaf : Node
{
    private Func<Status> _tickProcess;

    private Func<int, Status> _tickProcessList;
    private int _processIndexLimit;

    #region Constructors

    public Leaf() { }

    public Leaf(string name, Func<Status> tickProcess, float priority = 0) : base(name, priority)
    {
        _tickProcess = tickProcess;
    }
    
    public Leaf(string name, Func<Status> tickProcess, bool invert, float priority = 0) 
        : base(name, invert, priority)
    {
        _tickProcess = tickProcess;
    }
    
    public Leaf(string name, Func<int, Status> tickProcessList, int processIndexLimit, float priority = 0)
        : base(name, priority)
    {
        _tickProcessList = tickProcessList;
        _processIndexLimit = processIndexLimit;
    }
    
    public Leaf(string name, Func<int, Status> tickProcessList, int processIndexLimit, bool invert, float priority = 0)
        : base(name, invert, priority)
    {
        _tickProcessList = tickProcessList;
        _processIndexLimit = processIndexLimit;
    }

    #endregion

    public override Status Process()
    {
        Status status = Status.Failure;
        if (_tickProcess != null)
            status = _tickProcess.Invoke();
        else if (_tickProcessList != null)
            status = _tickProcessList.Invoke(Random.Range(0, _processIndexLimit));

        if (_invert)
            return InvertStatus(status);

        return status;
    }
}
