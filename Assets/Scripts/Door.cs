using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour
{
    [SerializeField] private bool _isLocked;
    private NavMeshObstacle _navObstacle;

    private void Awake()
    {
        _navObstacle = GetComponent<NavMeshObstacle>();
    }

    public bool IsLocked
    {
        get { return _isLocked; }
    }

    public void OpenDoor()
    {
        _navObstacle.enabled = false;
    }
}
