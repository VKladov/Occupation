using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SpawnPerson : ITickable
{
	public event Action<Person> PersonDied;
	
	[Inject] private ClickPicker _clickPicker;
	[Inject] private Person.Factory _factory;
	[Inject] private Constants _constants;
	[Inject(Id = Team.RussianArmy)] private TeamSkinPreset _russianSkinPreset;
	[Inject(Id = Team.UkranianArmy)] private TeamSkinPreset _ukrainianSkinPreset;
	[Inject(Id = Team.Citizen)] private TeamSkinPreset _cityzenSkinPreset;
	
	public List<Person> People = new List<Person>();

	public void Tick()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			var person = Spawn(_russianSkinPreset, _clickPicker.CheckClick().GroundPoint);
			person.Movement.AreaMask = _constants.NavMeshAreaMasks.Everywhere;
			person.Movement.Enable();
			person.StateMachine.ChangeStrategy(new BasicShotStrategy(person));
		}
		else if (Input.GetKeyDown(KeyCode.W))
		{
			var person = Spawn(_cityzenSkinPreset, _clickPicker.CheckClick().GroundPoint);
			person.Movement.AreaMask = _constants.NavMeshAreaMasks.Footpath;
			person.Movement.Enable();
			person.StateMachine.ChangeStrategy(new HomeShopLoop(person));
		}
		else if (Input.GetKeyDown(KeyCode.E))
		{
			var person = Spawn(_ukrainianSkinPreset, _clickPicker.CheckClick().GroundPoint);
			person.Movement.AreaMask = _constants.NavMeshAreaMasks.Everywhere;
			person.Movement.Enable();
			person.StateMachine.ChangeStrategy(new BasicShotStrategy(person));
		}
	}

	public Person Spawn(TeamSkinPreset team, Vector3 position)
	{
		var person = _factory.Create();
		person.transform.position = position;
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
		return person;
	}

	private void PersonOnDied(Person person)
	{
		person.Died -= PersonOnDied;
		PersonDied?.Invoke(person);
	}
}