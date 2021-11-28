using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Leaf : Node
{
    private Func<Status> _tickProcess;

    private Func<int, Status> _tickProcessList;
    private bool _processRandom;
    private int _processIndex, _processIndexLimit;

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
    
    public Leaf(string name, Func<int, Status> tickProcessList, int processIndex, float priority = 0)
        : base(name, priority)
    {
        _tickProcessList = tickProcessList;
        _processIndex = processIndex;
    }
    
    public Leaf(string name, Func<int, Status> tickProcessList, int processIndex, bool invert, float priority = 0)
        : base(name, invert, priority)
    {
        _tickProcessList = tickProcessList;
        _processIndex = processIndex;
    }
    
    public Leaf(string name, Func<int, Status> tickProcessList, bool processRandom, int processIndexLimit, float priority = 0)
        : base(name, priority)
    {
        _tickProcessList = tickProcessList;
        _processRandom = processRandom;
        _processIndexLimit = processIndexLimit;
    }
    
    public Leaf(string name, Func<int, Status> tickProcessList, bool processRandom, int processIndexLimit, bool invert, float priority = 0)
        : base(name, invert, priority)
    {
        _tickProcessList = tickProcessList;
        _processRandom = processRandom;
        _processIndexLimit = processIndexLimit;
    }

    #endregion

    public override Status Process()
    {
        Status status = Status.Failure;
        if (_tickProcess != null)
            status = _tickProcess.Invoke();
        else if (_tickProcessList != null)
            status = _tickProcessList.Invoke(_processRandom ? Random.Range(0, _processIndexLimit) : _processIndex);
        
        if (_invert)
            return InvertStatus(status);

        return status;
    }
}
