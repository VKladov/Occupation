using System;
using UnityEngine;
using Zenject;

public class Gun : MonoBehaviour
{
	public class Factory : PlaceholderFactory<GameObject, Gun>
	{
	}

	public float ShotDelay => _shotDelay;
	public float Damage => _damage;
	public WeaponType Type => _type; 
	
	[SerializeField] private int _magCapacity;
	[SerializeField] private Transform _barrel;
	[SerializeField] private WeaponType _type;
	[SerializeField] private float _shotDelay;
	[SerializeField] private float _damage;
	[Inject] private VisualEffects _visualEffects;

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
		
		_visualEffects.Shot(_barrel.position, target - _barrel.position);
		_bullets -= 1;
		return true;
	}

	public void Reload()
	{
		_bullets = _magCapacity;
	}
}