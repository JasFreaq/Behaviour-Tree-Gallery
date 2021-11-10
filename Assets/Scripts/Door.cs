using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private bool _isLocked;

    public bool IsLocked
    {
        get { return _isLocked; }
    }
}
