using System;
using UnityEngine;

public static class StrategySource
{
	public static PersonStrategy GetAttackStrategy(Person owner)
	{
		return new PersonStrategy(
			owner, 
			owner.MainTarget,
			new PersonStrategy.StrategyEvents
			{
				OnSeeEnemy = person =>
				{
					owner.StateMachine.SwitchToState(new ShotPerson(person));
				},
				OnTakeShot = person =>
				{
					owner.StateMachine.SwitchToState(new ShotPerson(person));
				},
				OnHearShot = enemy =>
				{
					owner.StateMachine.SwitchToState(new ShotPerson(enemy));
				}
			}
		);
	}
	public static PersonStrategy GetDefenceStrategy(Person owner)
	{
		Func<Vector3, PersonStrategy> goRest = null;
		Func<Vector3, PersonStrategy> goToHospital = null;
		Func<Vector3, PersonStrategy> goToPoint = null;
		
		goRest = targetPoint => new PersonStrategy(
			owner, 
			targetPoint,
			new PersonStrategy.StrategyEvents
			{
				OnHealStateChanged = isHealing =>
				{
					if (!isHealing && owner.Health.NormalizedValue < 1f)
					{
						owner.StateMachine.ChangeStrategy(goToHospital(owner.Hospital.transform.position));
					}
					else
					{
						owner.StateMachine.ChangeStrategy(goToPoint(owner.MainTarget));
					}
				},
				OnSeeEnemy = enemy =>
				{
					owner.StateMachine.SwitchToState(new ShotPerson(enemy));
				},
				OnTakeShot = enemy =>
				{
					owner.StateMachine.SwitchToState(new ShotPerson(enemy));
				},
			}
		);
		
		goToHospital = targetPoint => new PersonStrategy(
			owner, 
			targetPoint,
			new PersonStrategy.StrategyEvents
			{
				OnEnterBuilding = building =>
				{
					if (building is Hospital)
					{
						owner.SetMovementState(PersonMovementState.Stand);
						owner.Health.TakeHeal();
						owner.StateMachine.ChangeStrategy(goRest(owner.RestPoint.transform.position));
					}
				},
				OnSeeEnemy = enemy =>
				{
					owner.StateMachine.SwitchToState(new ShotPerson(enemy));
				},
				OnTakeShot = enemy =>
				{
					owner.StateMachine.SwitchToState(new ShotPerson(enemy));
				}
			}
		);
		
		goToPoint = targetPoint => new PersonStrategy(
			owner, 
			targetPoint,
			new PersonStrategy.StrategyEvents
			{
				OnSeeEnemy = person =>
				{
					owner.StateMachine.SwitchToState(new ShotPerson(person));
				},
				OnTakeShot = person =>
				{
					if (owner.Health.NormalizedValue < 0.5f)
					{
						owner.SetMovementState(PersonMovementState.Crouch);
						owner.StateMachine.ChangeStrategy(goToHospital(owner.Hospital.transform.position));
						return;
					}
					owner.StateMachine.SwitchToState(new ShotPerson(person));
				},
				OnHearShot = enemy =>
				{
					owner.StateMachine.SwitchToState(new ShotPerson(enemy));
				}
			}
		);

		return goToPoint(owner.MainTarget);
	}
}