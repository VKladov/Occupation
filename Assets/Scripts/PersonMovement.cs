using System;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class PersonMovement : IDisposable
{
	[Inject]
	private NavMeshAgent _navMeshAgent;

	public Vector3 NavAgentVelocity => _navMeshAgent.velocity;

	public void Enable()
	{
		_navMeshAgent.enabled = true;
	}

	public void Dispose()
	{
		_navMeshAgent.enabled = false;
	}

	public float MaxSpeed
	{
		set => _navMeshAgent.speed = value;
		get => _navMeshAgent.speed;
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