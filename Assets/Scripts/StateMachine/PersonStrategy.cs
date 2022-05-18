using System;
using System.Linq;
using UnityEngine;

public abstract class PersonStrategy : IDisposable
{
	protected Person Owner;
	protected Vector3 TargetPoint;

	protected PersonStrategy(Person owner, Vector3 targetPoint)
	{
		Owner = owner;
		TargetPoint = targetPoint;

		Subscribe();
		Owner.Movement.MoveTo(targetPoint);
	}

	public void SetTargetPoint(Vector3 point)
	{
		TargetPoint = point;
		Owner.Movement.MoveTo(point);
	}

	public virtual void OnSeeTargetPoint()
	{
	}

	public virtual void OnReachTargetPoint()
	{
	}

	public virtual void OnSeeEnemy(Person enemy)
	{
	}

	public virtual void OnTakeShot(Person attacker)
	{
	}

	public virtual void OnHearShot(Person attacker)
	{
	}

	public virtual void OnEnterBuilding(Building building)
	{
	}

	public virtual void OnHealStateChanged(bool isHealing)
	{
	}

	public virtual void OnHealthStateChanged()
	{
	}

	public abstract void Unsubscribe();

	private void Subscribe()
	{
		Owner.TookShot += OnTakeShot;
		Owner.Senses.ReceivedSound += OnHearShot;
		Owner.EnteredBuilding += OnEnterBuilding;
		Owner.Health.HealStateChanged += OnHealStateChanged;
		Owner.Health.StateChanged += OnHealthStateChanged;
	}

	public void Dispose()
	{
		Owner.TookShot -= OnTakeShot;
		Owner.Senses.ReceivedSound -= OnHearShot;
		Owner.EnteredBuilding -= OnEnterBuilding;
		Owner.Health.HealStateChanged -= OnHealStateChanged;
		Owner.Health.StateChanged += OnHealthStateChanged;
		Unsubscribe();
	}

	public void Update()
	{
		if (Owner.Senses.CanSeePoint(TargetPoint))
		{
			OnSeeTargetPoint();
		}
		
		if (Vector3.Distance(Owner.transform.position, TargetPoint) < 1f)
		{
			OnReachTargetPoint();
		}
		
		var allEnemies = Owner.AllOtherPeople.Where(item => item.IsAlive && item.Team != Owner.Team);
		var finePeople = allEnemies.Where(item => item.Health.NormalizedValue >= 0.5f);
		foreach (var person in finePeople)
		{
			if (Owner.Senses.CanSeePerson(person))
			{
				OnSeeEnemy(person);
				return;
			}
		}

		var injuredPeople = allEnemies.Where(item => item.Health.NormalizedValue < 0.5f);
		foreach (var person in injuredPeople)
		{
			if (Owner.Senses.CanSeePerson(person))
			{
				OnSeeEnemy(person);
				return;
			}
		}
	}
}