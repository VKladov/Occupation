using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PersonMovement : MonoBehaviour
{
	private NavMeshAgent _navMeshAgent;

	public Vector3 NavAgentVelocity => _navMeshAgent.velocity;

	public float MaxSpeed
	{
		set => _navMeshAgent.speed = value;
		get => _navMeshAgent.speed;
	}

	private void Awake()
	{
		_navMeshAgent = GetComponent<NavMeshAgent>();
	}

	private void OnEnable()
	{
		_navMeshAgent.enabled = true;
	}

	private void OnDisable()
	{
		_navMeshAgent.enabled = false;
	}

	public void MoveTo(Vector3 position)
	{
		if (_navMeshAgent.enabled == false)
		{
			return;
		}
		
		_navMeshAgent.isStopped = false;
		_navMeshAgent.SetDestination(position);
	}

	public void Stop()
	{
		_navMeshAgent.isStopped = true;
		_navMeshAgent.ResetPath();
	}
}