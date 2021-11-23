using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatronBehaviour : BTAgent
{
    [Header("Robber")]
    [SerializeField] private Transform _frontdoor;
    [SerializeField] private Transform _base;
    [SerializeField] private Transform[] _artworks;
    [SerializeField] private float _boredomIncrement = 50f;

    private float _boredom;
    private Coroutine _boredomIncrementRoutine;
    private Coroutine _boredomDecrementRoutine;

    private float _currentBoredomLowerBound;
    private Transform _currentArt;

    protected override void Start()
    {
        Sequence visitSequence = new Sequence("Visit Gallery");
        visitSequence.AddChild(new Leaf("Is Bored", IsBored));
        visitSequence.AddChild(new Leaf("Move to Frontdoor", MoveToFrontdoor));
        visitSequence.AddChild(new Leaf("Move To Art", MoveToArt, _artworks.Length));
        visitSequence.AddChild(new Leaf("Look At Art", LookAtArt));
        visitSequence.AddChild(new Leaf("Move To Base", MoveToBase));
        
        Selector decisionSelector = new Selector("Decision Selector");
        decisionSelector.AddChild(visitSequence);
        decisionSelector.AddChild(new Leaf("Move To Base", MoveToBase));

        _tree.AddChild(decisionSelector);
        base.Start();

        _boredom = Random.Range(0, 25) * 20f;
        _boredomIncrementRoutine = StartCoroutine(IncreaseBoredomRoutine());
    }
    
    private Node.Status IsBored()
    {
        if (_boredom >= 500)
            return Node.Status.Success;

        return Node.Status.Failure;
    }

    private Node.Status MoveToArt(int itemIndex)
    {
        if (_boredomIncrementRoutine != null)
        {
            StopCoroutine(_boredomIncrementRoutine);
            _boredomIncrementRoutine = null;
        }

        if (_currentArt || _artworks[itemIndex])
        {
            if (!_currentArt)
                _currentArt = _artworks[itemIndex];

            return MoveToLocation(_currentArt.position);
        }
        return Node.Status.Failure;
    }

    private Node.Status LookAtArt()
    {
        if (_boredomDecrementRoutine == null)
        {
            _boredomDecrementRoutine = StartCoroutine(DecreaseBoredomRoutine());
            _currentBoredomLowerBound = Random.Range(0, 24) * 20f;
        }

        if (_boredom <= _currentBoredomLowerBound)
        {
            StopCoroutine(_boredomDecrementRoutine);
            _boredomDecrementRoutine = null;
            return Node.Status.Success;
        }

        return Node.Status.Running;
    }

    private Node.Status MoveToFrontdoor()
    {
        return MoveToLocation(_frontdoor.position);
    }
    
    private Node.Status MoveToBase()
    {
        Node.Status status = MoveToLocation(_base.position);
        if (status == Node.Status.Success)
        {
            if (_boredomIncrementRoutine == null)
                _boredomIncrementRoutine = StartCoroutine(IncreaseBoredomRoutine());
        }

        return status;
    }

    private IEnumerator IncreaseBoredomRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            _boredom += _boredomIncrement;
        }
    }
    
    private IEnumerator DecreaseBoredomRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            _boredom -= _boredomIncrement;
        }
    }
}
