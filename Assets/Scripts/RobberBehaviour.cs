using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberBehaviour : MonoBehaviour
{
    [SerializeField] private Transform _diamond;
    [SerializeField] private Transform _frontdoor;
    [SerializeField] private Transform _backdoor;
    [SerializeField] private Transform _van;

    [Header("Navigation")] 
    [SerializeField] private float _destReachBuffer = 2f;

    private BehaviourTree _behaviourTree;
    private NavMeshAgent _navAgent;

    private ActionState _currentState = ActionState.Idle;
    private Node.Status _treeStatus;

    private void Awake()
    {
        _behaviourTree = new BehaviourTree();

        _navAgent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        Selector operDoorSelector = new Selector("Open Door");
        operDoorSelector.AddChild(new Leaf("Move To Backdoor", MoveToBackdoor));
        operDoorSelector.AddChild(new Leaf("Move To Front Door", MoveToFrontdoor));

        Sequence stealSequence = new Sequence("Steal Diamond");
        stealSequence.AddChild(operDoorSelector);
        stealSequence.AddChild(new Leaf("Move To Diamond", MoveToDiamond));
        stealSequence.AddChild(operDoorSelector);
        stealSequence.AddChild(new Leaf("Move To Van", MoveToVan));

        _behaviourTree.AddChild(stealSequence);

        //_behaviourTree.PrintTree();
        _treeStatus = Node.Status.Running;
    }

    private void Update()
    {
        if (_treeStatus == Node.Status.Running)
            _treeStatus = _behaviourTree.Process();
    }
    
    private Node.Status MoveToDiamond()
    {
        return MoveToLocation(_diamond.position);
    }

    private Node.Status MoveToFrontdoor()
    {
        return MoveToLocation(_frontdoor.position);
    }
    
    private Node.Status MoveToBackdoor()
    {
        return MoveToLocation(_backdoor.position);
    }

    private Node.Status MoveToVan()
    {
        return MoveToLocation(_van.position);
    }
    
    private Node.Status MoveToLocation(Vector3 destination)
    {
        if (_currentState == ActionState.Idle)
        {
            _navAgent.SetDestination(destination);
            _currentState = ActionState.Working;
        }
        else
        {
            if (Vector3.Distance(_navAgent.pathEndPosition, destination) > _destReachBuffer)
            {
                _currentState = ActionState.Idle;
                return Node.Status.Failure;
            }

            if (Vector3.Distance(destination, transform.position) <= _destReachBuffer)
            {
                _currentState = ActionState.Idle;
                return Node.Status.Success;
            }
        }

        return Node.Status.Running;
    }
}
