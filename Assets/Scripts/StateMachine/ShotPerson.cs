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
		Owner.Animator.SetBool("Aim", true);

		await Owner.RotateTo(_target.transform, 0.1f);
		_lockRotationToTarget = true;

		while (_target.IsAlive && !CancellationToken.IsCancellationRequested)
		{
			if (Owner.Gun == null)
			{
				if (!await Owner.TakeWeapon())
				{
					Owner.StateMachine.SwitchToState(new IdleState());
					return;
				}
			}
			
			if (Owner.Gun.NeedReload())
			{
				Owner.Animator.SetTrigger("Reload");
				if (await Delay(Random.Range(3.5f, 4f)))
				{
					Complete();
					return;
				}
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
			Owner.Animator.SetTrigger("Shot");
			GlobalSounds.Sound?.Invoke(Owner);
			if (willHit)
			{
				_target.TakeShot(Owner, Owner.Gun.Damage);
			}
			await Delay( Owner.Gun.ShotDelay * Random.Range(1f, 1.1f));
		}
		Complete();
	}

	public override void Update()
	{
		if (_lockRotationToTarget)
		{
			Owner.transform.rotation = Quaternion.LookRotation(_target.transform.position - Owner.transform.position);
		}
	}

	public override void Exit()
	{
		
	}
}