using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private Vector3 _rotation = new Vector3(0, 30, 0);

    void Update()
    {
        transform.Rotate(_rotation * Time.deltaTime);
    }
}
