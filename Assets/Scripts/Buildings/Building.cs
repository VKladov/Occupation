using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] protected Transform _enter;
    
    public readonly Storage Storage = new Storage();
    public Vector3 Enter => _enter.position;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Person person))
        {
            person.HandleBuildingEnter(this);
        }
    }
}
