using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float _minPenetrationChance;
    [SerializeField] private float _maxPenetrationChance;

    private const float MinPenetrationDistance = 10f;

    public float GetPenetrationChance(Vector3 position)
    { 
	    var distance = Vector3.Distance(transform.position, position); 
	    return Mathf.Lerp(_maxPenetrationChance, _minPenetrationChance, distance / MinPenetrationDistance);
    }
}
