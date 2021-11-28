using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BTAgent : MonoBehaviour
{
    [Header("BT Agent")]
    [SerializeField] private float _updateFrequency = 0.5f;
    [SerializeField] private float _destReachBuffer = 2f;

    protected Tree _tree;
    protected NavMeshAgent _navAgent;

    protected BTAgentState _currentState = BTAgentState.Idle;
    protected Node.Status _treeStatus;
    
    private WaitForSeconds _updateWaitForSeconds;
    private Vector3 _chaseLocation;

    private void Awake()
    {
        _navAgent = GetComponent<NavMeshAgent>();

        _tree = new Tree();
        _updateWaitForSeconds = new WaitForSeconds(_updateFrequency);
    }

    protected virtual void Start()
    {
        StartCoroutine(ProcessBehaviour());
    }

    private IEnumerator ProcessBehaviour()
    {
        while (true)
        {
            _treeStatus = _tree.Process();
            yield return _updateWaitForSeconds;
        }
    }
    
    protected Node.Status MoveToLocation(Vector3 destination)
    {
        if (_currentState == BTAgentState.Idle)
        {
            _navAgent.SetDestination(destination);
            _currentState = BTAgentState.Running;
        }
        else
        {
            if (Vector3.Distance(_navAgent.pathEndPosition, destination) > _destReachBuffer)
            {
                _currentState = BTAgentState.Idle;
                return Node.Status.Failure;
            }

            if (Vector3.Distance(destination, transform.position) <= _destReachBuffer)
            {
                _currentState = BTAgentState.Idle;
                return Node.Status.Success;
            }
        }

        return Node.Status.Running;
    }

    protected Node.Status CanSee(Vector3 target, string targetTag, float maxDistance, float minDistance, float maxAngle)
    {
        Vector3 directionToTarget = target - transform.position;
        float angle = Vector3.Angle(directionToTarget, transform.forward);

        if (directionToTarget.magnitude <= minDistance ||
            directionToTarget.magnitude <= maxDistance && angle <= maxAngle) 
        {
            if (Physics.Raycast(transform.position, directionToTarget, out RaycastHit hit))
            {
                if (hit.collider.gameObject.CompareTag(targetTag))
                    return Node.Status.Success;
            }
        }

        return Node.Status.Failure;
    }

    protected Node.Status Flee(Vector3 location, float distance)
    {
        return MoveToLocation(transform.position + (transform.position - location).normalized * distance);
    }
    
    protected Node.Status Chase(Vector3 location, float distance)
    {
        if (_currentState == BTAgentState.Idle)
            _chaseLocation = transform.position - (transform.position - location).normalized * distance;

        return MoveToLocation(_chaseLocation);
    }
}
