using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DepSequence : Sequence
{
    private Node _dependancy;
    private NavMeshAgent _navAgent;

    public DepSequence(string name, Node dependancy, NavMeshAgent agent, float priority = 0)
        : base(name, priority)
    {
        _dependancy = dependancy;
        _navAgent = agent;
    }

    public DepSequence(string name, Node dependancy, NavMeshAgent agent, bool invert, float priority = 0) 
        : base(name, invert, priority)
    {
        _dependancy = dependancy;
        _navAgent = agent;
    }

    public override Status Process()
    {
        if (_dependancy.Process() == Status.Failure)
        {
            _navAgent.ResetPath();
            Reset();
            return Status.Failure;
        }
        
        return base.Process();
    }
}