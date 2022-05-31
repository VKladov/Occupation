using System;
using UnityEngine;
using Zenject;

public class PersonSenses : IDisposable, IInitializable
{
	public event Action<Person> ReceivedSound;
	
	[Inject]
	private readonly Person _owner;
	
	[Inject]
	private readonly Constants _constants;
	
	private const float FieldOfView = 100f;
	public void Initialize()
	{
		GlobalSounds.Sound += OnSound; 
	}

	public void Dispose()
	{
		GlobalSounds.Sound -= OnSound; 
	}

	private void OnSound(Person source)
	{
		ReceivedSound?.Invoke(source);
	}

	public bool CanSeePerson(Person person)
	{
		return CanSeePoint(person.transform.position);
	}

	public bool CanSeePoint(Vector3 point)
	{
		var inFieldOfView = Vector3.Angle(_owner.transform.forward, point - _owner.transform.position) < FieldOfView / 2f;
		if (!inFieldOfView)
		{
			return false;
		}

		return !Physics.Linecast(_owner.transform.position + Vector3.up, point + Vector3.up, _constants.LayerMasks.ObstaclesLayer);
	}
}