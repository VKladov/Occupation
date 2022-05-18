using UnityEngine;
using Zenject;

public class VisualEffects
{
	[Inject(Id = ID.ShotEffectPool)] private VisualEffect.Pool _shotPool;
	[Inject(Id = ID.BloodShootEffectPool)] private VisualEffect.Pool _bloodShotPool;

	public void Shot(Vector3 position, Vector3 direction) => ShowEffect(_shotPool, position, direction);
	public void BloodShot(Vector3 position, Vector3 direction) => ShowEffect(_bloodShotPool, position, direction);

	private static void ShowEffect(VisualEffect.Pool pool, Vector3 position, Vector3 direction)
	{
		var effect = pool.Spawn();
		effect.Play(position, direction);
	}
}