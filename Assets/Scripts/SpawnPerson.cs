using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SpawnPerson : ITickable
{
	public event Action<Person> PersonDied;
	
	[Inject] private ClickPicker _clickPicker;
	[Inject] private Person.Factory _factory;
	
	public List<Person> People = new List<Person>();

	public void Tick()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			Spawn(0);
		}
		else if (Input.GetKeyDown(KeyCode.E))
		{
			Spawn(1);
		}
	}

	private void Spawn(int teamID)
	{
		var person = _factory.Create();
		person.transform.position = _clickPicker.CheckClick().GroundPoint;
		person.SetTeam(teamID);
		person.Died += PersonOnDied;
		People.Add(person);
	}

	private void PersonOnDied(Person person)
	{
		person.Died -= PersonOnDied;
		PersonDied?.Invoke(person);
	}
}