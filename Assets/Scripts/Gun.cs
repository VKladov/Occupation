using System;
using UnityEngine;

public class Gun : MonoBehaviour
{
	[SerializeField] private int _magCapacity;
	[SerializeField] private Transform _barrel;
	[SerializeField] private ParticleSystem _shotEffect;
	[SerializeField] private Transform _trailEffect;

	private int _bullets;

	private void Awake()
	{
		Reload();
	}

	public bool NeedReload() => _bullets < 1;

	public bool TryShoot(Vector3 target)
	{
		if (_bullets < 1)
		{
			return false;
		}

		_bullets -= 1;
		_trailEffect.transform.rotation = Quaternion.LookRotation(target - _trailEffect.transform.position);
		_shotEffect.Play();
		return true;
	}

	public void Reload()
	{
		_bullets = _magCapacity;
	}
}