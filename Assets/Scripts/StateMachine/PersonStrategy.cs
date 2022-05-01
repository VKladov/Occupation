using System;
using System.Linq;
using UnityEngine;

public class PersonStrategy : IDisposable
{
	public class StrategyEvents
	{
		public Action OnSeeTargetPoint;
		public Action OnReachTargetPoint;
		public Action<Person> OnSeeEnemy;
		public Action<Person> OnTakeShot;
		public Action<Person> OnHearShot;
		public Action<Building> OnEnterBuilding;
		public Action<bool> OnHealStateChanged;
	}
	
	private Person _owner;
	private Vector3 _targetPoint;
	private StrategyEvents _events;
	public bool IsEnable { get; private set; } = true;

	public PersonStrategy(
		Person owner,
		Vector3 targetPoint,
		StrategyEvents events)
	{
		_owner = owner;
		_targetPoint = targetPoint;
		_events = events;
		
		Subscribe();
		
		_owner.Movement.MoveTo(targetPoint);
	}

	public void SetTargetPoint(Vector3 point)
	{
		_targetPoint = point;
		_owner.Movement.MoveTo(point);
	}

	public void Pause()
	{
		IsEnable = false;
	}

	public void Resume()
	{
		IsEnable = true;
	}
	
	private void Subscribe()
	{
		_owner.TookShot += OwnerOnTookShot;
		_owner.Senses.ReceivedSound += SensesOnReceivedSound;
		_owner.EnteredBuilding += OwnerOnEnteredBuilding;
		_owner.Health.HealStateChanged += HealthOnHealStateChanged;
		_owner.Health.StateChanged += HealthStateChanged;
	}

	public void Dispose()
	{
		_owner.TookShot -= OwnerOnTookShot;
		_owner.Senses.ReceivedSound -= SensesOnReceivedSound;
		_owner.EnteredBuilding -= OwnerOnEnteredBuilding;
		_owner.Health.HealStateChanged -= HealthOnHealStateChanged;
		_owner.Health.StateChanged -= HealthStateChanged;
	}

	public void Update()
	{
		if (_events.OnSeeTargetPoint != null)
		{
			if (_owner.Senses.CanSeePoint(_targetPoint))
			{
				_events.OnSeeTargetPoint?.Invoke();
				return;
			}	
		}

		if (_events.OnReachTargetPoint != null)
		{
			if (Vector3.Distance(_owner.transform.position, _targetPoint) < 1f)
			{
				_events.OnReachTargetPoint?.Invoke();
				return;
			}
		}
		
		if (_events.OnSeeEnemy != null)
		{
			var allEnemies = _owner.AllOtherPeople.Where(item => item.IsAlive && item.TeamID != _owner.TeamID);
			var finePeople = allEnemies.Where(item => item.Health.NormalizedValue >= 0.5f);
			foreach (var person in finePeople)
			{
				if (_owner.Senses.CanSeePerson(person))
				{
					_events.OnSeeEnemy?.Invoke(person);
					return;
				}
			}
		
			var injuredPeople = allEnemies.Where(item => item.Health.NormalizedValue < 0.5f);
			foreach (var person in injuredPeople)
			{
				if (_owner.Senses.CanSeePerson(person))
				{
					_events.OnSeeEnemy?.Invoke(person);
					return;
				}
			}
		}
	}

	private void HealthOnHealStateChanged(bool isHealing)
	{
		_events.OnHealStateChanged?.Invoke(isHealing);
	}

	private void OwnerOnEnteredBuilding(Building building)
	{
		_events.OnEnterBuilding?.Invoke(building);
	}

	private void SensesOnReceivedSound(Person source)
	{
		if (source.TeamID == _owner.TeamID)
		{
			return;
		}
		
		_events.OnHearShot?.Invoke(source);
	}

	private void OwnerOnTookShot(Person attacker)
	{
		_events.OnTakeShot?.Invoke(attacker);
	}

	private void HealthStateChanged()
	{
		
	}
}