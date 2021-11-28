using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopBehaviour : BTAgent
{
    [SerializeField] private Transform[] _waypoints;
    [SerializeField] private Transform _robber;
    [SerializeField] private float _robberDetectMaxDistance = 12.5f;
    [SerializeField] private float _robberDetectMinDistance = 7.5f;
    [SerializeField] private float _robberDetectAngle = 60f;
    [SerializeField] private float _chaseDistance = 15f;

    void Start()
    {
        Sequence checkRobberSequence = new Sequence("Check For Robber");
        checkRobberSequence.AddChild(new Leaf("Can See Robber", CanSeeRobber));
        checkRobberSequence.AddChild(new Leaf("Chase Robber", ChaseRobber));

        DepSequence patrolSequence = new DepSequence("Patrolling",
            new Leaf("Can See Cop", CanSeeRobber, true), _navAgent);
        for (int i = 0; i < _waypoints.Length; i++)
            patrolSequence.AddChild(new Leaf($"Move to {_waypoints[i]}", MoveToWaypoint, i));

        Selector baseSelector = new Selector("Base Selector");
        baseSelector.AddChild(checkRobberSequence);
        baseSelector.AddChild(patrolSequence);

        _tree.AddChild(baseSelector);
        base.Start();
    }

    private Node.Status MoveToWaypoint(int index)
    {
        return MoveToLocation(_waypoints[index].position);
    }

    private Node.Status CanSeeRobber()
    {
        return CanSee(_robber.transform.position, "Robber", _robberDetectMaxDistance, _robberDetectMinDistance, _robberDetectAngle);
    }

    private Node.Status ChaseRobber()
    {
        return Chase(_robber.position, _chaseDistance);
    }
}
