using System;
using UnityEngine;
using Zenject;

namespace Buildings
{
	public class House : Building
	{
		[SerializeField] private int _startCitizenCount = 2;
		[Inject] private SpawnPerson _spawnPerson;
		[Inject(Id = Team.Citizen)] private TeamSkinPreset _cityzenSkinPreset;

		private void Start()
		{
			for (var i = 0; i < _startCitizenCount; i++)
			{
				var person = _spawnPerson.Spawn(_cityzenSkinPreset, transform.position);
				person.House = this;
				person.EnterBuilding(this);
				person.StateMachine.ChangeStrategy(new HomeShopLoop(person));
			}
		}
	}
}