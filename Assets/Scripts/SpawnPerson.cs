using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using Zenject;

public class SpawnPerson : MonoBehaviour
{
	public event Action<Person> PersonDied;
	
	[SerializeField] private Person _personPrefab;
	[Inject] private ClickPicker _clickPicker;
	
	public List<Person> People = new List<Person>();

	private void Update()
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
		var person = Instantiate(_personPrefab, _clickPicker.CheckClick().GroundPoint, Quaternion.identity);
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