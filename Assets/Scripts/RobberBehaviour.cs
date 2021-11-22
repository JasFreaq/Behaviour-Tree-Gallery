using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class RobberBehaviour : BTAgent
{
    [Header("Robber")]
    [SerializeField] private Door _frontdoor;
    [SerializeField] private Door _backdoor;
    [SerializeField] private Transform[] _itemsToSteal;
    [SerializeField] private Transform _van;
    [SerializeField] private Transform _cop;
    [SerializeField] private float _copDetectDistance = 50f;
    [SerializeField] private float _copDetectAngle = 60f;
    [SerializeField] private float _fleeDistance = 75f;
    [SerializeField] [Range(0, 1000)] private int _money = 800;

    private Transform _currentStolenItem;
    private int _stolenItemCounter;

    protected override void Start()
    {
        PrioritySelector operDoorPSelector = new PrioritySelector("Open Door", true);
        operDoorPSelector.AddChild(new Leaf("Move To Front Door", MoveToFrontdoor, 2));
        operDoorPSelector.AddChild(new Leaf("Move To Backdoor", MoveToBackdoor, 1));
        
        Sequence stealDepSequence = new Sequence("Steal Dependancy");
        stealDepSequence.AddChild(new Leaf("Has No Money", HasMoney, true));
        stealDepSequence.AddChild(new Leaf("Can't See Cop", CanSeeCop, true));

        DepSequence stealSequence = new DepSequence("Steal Something", stealDepSequence, _navAgent);
        stealSequence.AddChild(new Leaf("Has No Money", HasMoney, true));
        stealSequence.AddChild(operDoorPSelector);
        stealSequence.AddChild(new Leaf("Steal Item", StealItem, _itemsToSteal.Length));
        stealSequence.AddChild(new Leaf("Move To Van", MoveToVan));

        Selector stealSelector = new Selector("Steal Selection");
        stealSelector.AddChild(stealSequence);
        stealSelector.AddChild(new Leaf("Move To Van", MoveToVan));

        Sequence bewareCopSequence = new Sequence("Beware Of Cop");
        bewareCopSequence.AddChild(new Leaf("Can See Cop", CanSeeCop));
        bewareCopSequence.AddChild(new Leaf("Flee From Cop", FleeFromCop));

        Selector baseSelector = new Selector("Base Selector");
        baseSelector.AddChild(bewareCopSequence);
        baseSelector.AddChild(stealSelector);

        _tree.AddChild(baseSelector);

        //_tree.PrintTree();
        base.Start();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            Time.timeScale = 2.5f;
    }

    private Node.Status HasMoney()
    {
        if (_money >= 500)
            return Node.Status.Success;

        return Node.Status.Failure;
    }

    private Node.Status StealItem(int itemIndex)
    {
        if (_stolenItemCounter < _itemsToSteal.Length && (_currentStolenItem || _itemsToSteal[itemIndex])) 
        {
            if (!_currentStolenItem)
                _currentStolenItem = _itemsToSteal[itemIndex];

            Node.Status status = MoveToLocation(_currentStolenItem.position);
            if (status == Node.Status.Success)
                _currentStolenItem.SetParent(transform);

            return status;
        }
        return Node.Status.Failure;
    }

    private Node.Status MoveToFrontdoor()
    {
        return MoveToDoor(_frontdoor);
    }
    
    private Node.Status MoveToBackdoor()
    {
        return MoveToDoor(_backdoor);
    }

    private Node.Status MoveToVan()
    {
        Node.Status status = MoveToLocation(_van.position);
        if (status == Node.Status.Success)
        {
            Destroy(_currentStolenItem.gameObject);
            _stolenItemCounter++;
            _money += 500;
        }

        return status;
    }

    private Node.Status MoveToDoor(Door door)
    {
        Node.Status status = MoveToLocation(door.transform.position);
        if (status == Node.Status.Success)
        {
            if (!door.IsLocked)
            {
                door.OpenDoor();
                return Node.Status.Success;
            }

            return Node.Status.Failure;
        }

        return status;
    }

    private Node.Status CanSeeCop()
    {
        return CanSee(_cop.transform.position, "Cop", _copDetectDistance, _copDetectAngle);
    }

    private Node.Status FleeFromCop()
    {
        return Flee(_cop.transform.position, _fleeDistance);
    }
}
