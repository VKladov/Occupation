using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public readonly Storage Storage = new Storage();
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Person person))
        {
            person.HandleBuildingEnter(this);
        }
    }
}
