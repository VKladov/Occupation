using System;
using UnityEngine;

public class PersonSenses : IDisposable
{
	public event Action<Person> ReceivedSound;
	
	private readonly Person _owner;
	private readonly LayerMask _wallsLayerMask;
	private const float FieldOfView = 100f;
	
	public PersonSenses(Person owner, LayerMask wallsLayerMask)
	{
		_owner = owner;
		_wallsLayerMask = wallsLayerMask;
		GlobalSounds.Sound += OnSound; 
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

		return !Physics.Linecast(_owner.transform.position + Vector3.up, point + Vector3.up, _wallsLayerMask);
	}

	public void Dispose()
	{
		GlobalSounds.Sound -= OnSound; 
	}
}