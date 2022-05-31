using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class PersonStrategy : IDisposable
{
	protected Person Owner;
	protected CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

	protected PersonStrategy(Person owner)
	{
		Owner = owner;
		Subscribe();
	}

	protected void Cancel()
	{
		CancellationTokenSource.Cancel();
		CancellationTokenSource = new CancellationTokenSource();
	}

	protected UniTask<bool> Delay(float seconds)
	{
		return UniTask
			.Delay(Mathf.CeilToInt(seconds * 1000f), cancellationToken: CancellationTokenSource.Token)
			.SuppressCancellationThrow();
	}

	public virtual void Start()
	{
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

	private void OwnerOnTargetChanged()
	{
		Owner.ExitBuilding();
		if (Owner.HasMainTarget)
		{
			Owner.Movement.MoveTo(Owner.MainTarget.Position);
		}
	}

	private void Subscribe()
	{
		Owner.TookShot += OnTakeShot;
		Owner.Senses.ReceivedSound += OnHearShot;
		Owner.EnteredBuilding += OnEnterBuilding;
		Owner.Health.HealStateChanged += OnHealStateChanged;
		Owner.Health.StateChanged += OnHealthStateChanged;
		Owner.TargetChanged += OwnerOnTargetChanged;
	}

	public void Dispose()
	{
		Owner.TookShot -= OnTakeShot;
		Owner.Senses.ReceivedSound -= OnHearShot;
		Owner.EnteredBuilding -= OnEnterBuilding;
		Owner.Health.HealStateChanged -= OnHealStateChanged;
		Owner.Health.StateChanged += OnHealthStateChanged;
		Owner.TargetChanged -= OwnerOnTargetChanged;
		Unsubscribe();
	}

	public void Update()
	{
		if (Owner.HasMainTarget)
		{
			if (Owner.Senses.CanSeePoint(Owner.MainTarget.Position))
			{
				OnSeeTargetPoint();
			}
		
			if (Vector3.Distance(Owner.transform.position, Owner.MainTarget.Position) < 1f)
			{
				if (Owner.MainTarget is BuildingObjective buildingObjective)
				{
					Owner.EnterBuilding(buildingObjective.Target);
				}
				OnReachTargetPoint();
			}
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