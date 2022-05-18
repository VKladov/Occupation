using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SpawnPerson : ITickable
{
	public event Action<Person> PersonDied;
	
	[Inject] private ClickPicker _clickPicker;
	[Inject] private Person.Factory _factory;
	[Inject(Id = Team.RussianArmy)] private TeamSkinPreset _russianSkinPreset;
	[Inject(Id = Team.UkranianArmy)] private TeamSkinPreset _ukrainianSkinPreset;
	[Inject(Id = Team.Citizen)] private TeamSkinPreset _cityzenSkinPreset;
	
	public List<Person> People = new List<Person>();

	public void Tick()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			Spawn(_russianSkinPreset);
		}
		else if (Input.GetKeyDown(KeyCode.W))
		{
			Spawn(_cityzenSkinPreset);
		}
		else if (Input.GetKeyDown(KeyCode.E))
		{
			Spawn(_ukrainianSkinPreset);
		}
	}

	private void Spawn(TeamSkinPreset team)
	{
		var person = _factory.Create();
		person.transform.position = _clickPicker.CheckClick().GroundPoint;
		person.SkinGenerator.Setup(team);
		person.SetTeam(team);
		
		if (team.Rifle != null)
		{
			person.AddWeapon(team.Rifle);
		}

		if (team.Handgun)
		{
			person.AddWeapon(team.Handgun);
		}
		
		person.Died += PersonOnDied;
		People.Add(person);
	}

	private void PersonOnDied(Person person)
	{
		person.Died -= PersonOnDied;
		PersonDied?.Invoke(person);
	}
}