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

    public Leaf(string name, Func<Status> tickProcess, int priority = 0)
    {
        _name = name;
        _tickProcess = tickProcess;
        _priority = priority;
    }
    
    public Leaf(string name, Func<Status> tickProcess, bool invert, int priority = 0)
    {
        _name = name;
        _tickProcess = tickProcess;
        _invert = invert;
        _priority = priority;
    }
    
    public Leaf(string name, Func<int, Status> tickProcessList, int processIndexLimit, int priority = 0)
    {
        _name = name;
        _tickProcessList = tickProcessList;
        _processIndexLimit = processIndexLimit;
        _priority = priority;
    }
    
    public Leaf(string name, Func<int, Status> tickProcessList, int processIndexLimit, bool invert, int priority = 0)
    {
        _name = name;
        _tickProcessList = tickProcessList;
        _processIndexLimit = processIndexLimit;
        _invert = invert;
        _priority = priority;
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
