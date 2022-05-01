using Cysharp.Threading.Tasks;
using UnityEngine;

public class ShotPerson : PersonState
{
	private Person _target;
	private bool _lockRotationToTarget;

	public ShotPerson(Person target)
	{
		_target = target;
	}
	
	public override async void Enter()
	{
		_target.Health.StateChanged += TargetHealthStateChanged;
		Owner.Animator.SetBool("Aim", true);

		await Owner.RotateTo(_target.transform, 0.1f);
		_lockRotationToTarget = true;

		while (_target.IsAlive && !CancellationToken.IsCancellationRequested)
		{
			if (Owner.Gun.NeedReload())
			{
				Owner.Animator.SetTrigger("InfantryReload");
				await Delay(Random.Range(3.5f, 4f));
				Owner.Gun.Reload();
			}

			if (CancellationToken.IsCancellationRequested || !_target.IsAlive || !Owner.Senses.CanSeePerson(_target))
			{
				Complete();
				return;
			}
			
			var willHit = Owner.GetShotChance(_target) > Random.Range(0f, 1f);
			var delta = _target.transform.position - Owner.transform.position;
			var direction = delta.normalized;
			if (!willHit)
			{
				direction = Vector3.LerpUnclamped(direction, (Random.Range(0f, 1f) > 0.5f ? 1f : -1f) * Owner.transform.right, Random.Range(0.05f, 0.1f));
			}

			var shotPoint = Owner.transform.position + direction * delta.magnitude + Vector3.up * 1.6f;

			Owner.Gun.TryShoot(shotPoint);
			Owner.Animator.SetTrigger("InfantryShot");
			GlobalSounds.Sound?.Invoke(Owner);
			if (willHit)
			{
				_target.TakeShot(Owner);
			}
			await Delay(Random.Range(0.3f, 0.4f));
		}

		if (!_target.IsAlive)
		{
			Owner.Animator.SetBool("Aim", false);
		}
		
		Complete();
	}

	public override void Exit()
	{
		_target.Health.StateChanged -= TargetHealthStateChanged;
	}

	private void TargetHealthStateChanged()
	{
		if (_target.Health.NormalizedValue < 0.5f)
		{
			Complete();
		}
	}

	public override void Update()
	{
		if (_lockRotationToTarget)
		{
			Owner.transform.rotation = Quaternion.LookRotation(_target.transform.position - Owner.transform.position);
		}
	}
}