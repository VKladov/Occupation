using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BasicShotStrategy : PersonStrategy
{
	private Person _target;
	private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
	private bool HasTarget => _target != null && _target.IsAlive && Owner.Senses.CanSeePerson(_target);
	public BasicShotStrategy(Person owner) : base(owner)
	{
		
	}

	public override void OnSeeEnemy(Person enemy)
	{
		if (HasTarget)
		{
			return;
		}
		
		Attack(enemy);
	}

	public override void OnHearShot(Person attacker)
	{
		if (attacker.Team == Owner.Team)
		{
			return;
		}
		
		if (HasTarget)
		{
			return;
		}
		
		Attack(attacker);
	}

	public override void OnTakeShot(Person attacker)
	{
		if (Owner.Health.NormalizedValue < 0.5f)
		{
			Owner.SetMovementState(PersonMovementState.Crouch);
		}
		
		Attack(attacker);
	}

	public override void Unsubscribe()
	{
		if (_target != null)
		{
			_target.Died -= TargetOnDied;
		}
	}

	private void Attack(Person target)
	{
		_cancellationTokenSource.Cancel();
		
		if (_target != null)
		{
			_target.Died -= TargetOnDied;
		}
		_target = target;
		target.Died += TargetOnDied;
		Owner.StateMachine.SwitchToState(new ShotPerson(target));
	}

	private void TargetOnDied(Person target)
	{
		target.Died -= TargetOnDied;

		_cancellationTokenSource = new CancellationTokenSource();
		StartTimer();
	}

	private async void StartTimer()
	{
		if (await UniTask.Delay(500, cancellationToken: _cancellationTokenSource.Token).SuppressCancellationThrow())
		{
			return;
		}
		
		Owner.Animator.SetBool("Aim", false);
		if (await UniTask.Delay(500, cancellationToken: _cancellationTokenSource.Token).SuppressCancellationThrow())
		{
			return;
		}
		
		Owner.SetMovementState(PersonMovementState.Stand);
		if (await UniTask.Delay(1000, cancellationToken: _cancellationTokenSource.Token).SuppressCancellationThrow())
		{
			return;
		}

		await Owner.PutWeaponBack();
	}
}
