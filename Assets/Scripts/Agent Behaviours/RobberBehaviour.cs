using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class RobberBehaviour : BTAgent
{
    [Header("Robber")]
    [SerializeField] private Transform _frontdoor;
    [SerializeField] private Transform _backdoor;
    [SerializeField] private Transform[] _itemsToSteal;
    [SerializeField] private Transform _van;
    [SerializeField] private Transform _cop;
    [SerializeField] private float _copDetectMaxDistance = 10f;
    [SerializeField] private float _copDetectMinDistance = 5f;
    [SerializeField] private float _copDetectAngle = 60f;
    [SerializeField] private float _fleeDistance = 15f;
    [SerializeField] [Range(0, 1000)] private int _money = 800;
    [SerializeField] private int _moneyDecrement = 50;

    private Transform _currentStolenItem;
    private int _stolenItemCounter;
    private Coroutine _moneyDecrementRoutine;

    protected override void Start()
    {
        PrioritySelector operDoorPSelector = new PrioritySelector("Open Door", true);
        operDoorPSelector.AddChild(new Leaf("Move To Front Door", MoveToFrontdoor, 2));
        operDoorPSelector.AddChild(new Leaf("Move To Backdoor", MoveToBackdoor, 1));
        
        Sequence stealDepSequence = new Sequence("Steal Dependancy");
        stealDepSequence.AddChild(new Leaf("Is Gallery Closed", Blackboard.Instance.IsGalleryOpen, true));
        stealDepSequence.AddChild(new Leaf("Has No Money", HasMoney, true));
        stealDepSequence.AddChild(new Leaf("Can't See Cop", CanSeeCop, true));

        DepSequence stealSequence = new DepSequence("Steal Something", stealDepSequence, _navAgent);
        stealSequence.AddChild(operDoorPSelector);
        stealSequence.AddChild(new Leaf("Steal Item", StealItem, true, _itemsToSteal.Length));
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
        _moneyDecrementRoutine = StartCoroutine(DecreaseMoneyRoutine());
    }
    
    private Node.Status HasMoney()
    {
        if (_money >= 500)
            return Node.Status.Success;

        return Node.Status.Failure;
    }

    private Node.Status StealItem(int itemIndex)
    {
        if (_moneyDecrementRoutine != null)
        {
            StopCoroutine(_moneyDecrementRoutine);
            _moneyDecrementRoutine = null;
        }

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
        return MoveToLocation(_frontdoor.position);
    }
    
    private Node.Status MoveToBackdoor()
    {
        return MoveToLocation(_backdoor.position);
    }

    private Node.Status MoveToVan()
    {
        Node.Status status = MoveToLocation(_van.position);
        if (status == Node.Status.Success && _currentStolenItem)
        {
            if (_moneyDecrementRoutine == null)
                _moneyDecrementRoutine = StartCoroutine(DecreaseMoneyRoutine());

            Destroy(_currentStolenItem.gameObject);
            _stolenItemCounter++;
            _money += 500;
        }

        return status;
    }

    private Node.Status CanSeeCop()
    {
        return CanSee(_cop.transform.position, "Cop", _copDetectMaxDistance, _copDetectMinDistance, _copDetectAngle);
    }

    private Node.Status FleeFromCop()
    {
        return Flee(_cop.transform.position, _fleeDistance);
    }

    private IEnumerator DecreaseMoneyRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            _money = Mathf.Max(0, _money - _moneyDecrement);
        }
    }
}
