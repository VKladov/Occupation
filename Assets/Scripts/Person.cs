using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buildings;
using Cysharp.Threading.Tasks;
using ModestTree;
using UnityEngine;
using Zenject;

public class Person : MonoBehaviour
{
	public class Factory : PlaceholderFactory<Person>
	{
		
	}
	
	public event Action<Person> Died;
	public event Action<Person> TookShot;
	public event Action<Building> EnteredBuilding;
	public event Action TargetChanged;

	public string Name { get; private set; }
	public TeamSkinPreset Team { get; private set; }
	public PersonStateMachine StateMachine { get; private set; }
	public Gun Gun { get; private set; }

	[Inject] public readonly Animator Animator;
	[Inject] public readonly PersonMovement Movement;
	[Inject] public readonly PersonSenses Senses;
	[Inject] public readonly PersonHealth Health;
	[Inject] public readonly Storage Storage;
	[Inject] public readonly SkinGenerator SkinGenerator;
	[Inject] public readonly City City;
	[Inject] private VisualEffects _visualEffects;
	[Inject] private readonly List<WeaponSlot> _weaponSlots;
	[Inject] private readonly Gun.Factory _gunFactory;
	[SerializeField] private Transform _weaponContainer;
	
	public Vector3 MiddlePoint => transform.position + Vector3.up;

	public readonly PersonStats Stats = new PersonStats();

	public bool IsAlive => Health.NormalizedValue > 0f;
	public Person[] AllOtherPeople => FindObjectsOfType<Person>().Where(item => item.transform.root != transform.root).ToArray();
	
	private Outline[] _outlines;
	public Objective MainTarget { get; private set; }
	public bool HasMainTarget => MainTarget != null;
	public PersonMovementState MovementState { get; private set; }
	public Hospital Hospital => FindObjectOfType<Hospital>();
	public RestPoint RestPoint => FindObjectOfType<RestPoint>();
	public Building House;

	private Building _currentBuilding;

	public void EnterBuilding(Building building)
	{
		_currentBuilding = building;
		transform.position = building.transform.position;
		Movement.Disable();
	}

	public void ExitBuilding()
	{
		if (_currentBuilding == null)
		{
			return;
		}

		transform.position = _currentBuilding.Enter;
		Movement.Enable();
		_currentBuilding = null;
	}

	public void HandleBuildingEnter(Building building)
	{
		EnteredBuilding?.Invoke(building);
	}

	public void SetMainObjective(Objective target)
	{
		MainTarget = target;
		TargetChanged?.Invoke();
	}

	public void SetMovementState(PersonMovementState state)
	{
		MovementState = state;
		foreach (var movementState in PersonMovementState.All)
		{
			Animator.SetBool(movementState.AnimatorPrefix, MovementState == movementState);
		}

		Movement.MaxSpeed = state.MaxMovementSpeed;
	}

	public void TakeShot(Person attacker, float damage)
	{
		if (!IsAlive)
		{
			return;
		}
		
		_visualEffects.BloodShot(MiddlePoint, transform.position - attacker.transform.position);
		Animator.SetTrigger(MovementState.AnimatorPrefix + "_Infantry_Damage");
		Health.Change(-damage);
		TookShot?.Invoke(attacker);
	}

	public float GetShotChance(Person target)
	{
		var shotPoint = MiddlePoint;
		var targetPoint = target.MiddlePoint;
		var distance = Vector3.Distance(shotPoint, targetPoint);

		var obstacleChances = 1f;
		var hits = Physics.RaycastAll(shotPoint, targetPoint - shotPoint, 1000f);
		foreach (var hit in hits)
		{
			if (hit.collider.gameObject.TryGetComponent(out Obstacle obstacle))
			{
				obstacleChances *= obstacle.GetPenetrationChance(shotPoint);
			}
		}
		
		var isMoving = Movement.NavAgentVelocity.magnitude > 0.2f;
		var occuracy = isMoving ? MovementState.OccuracyScaleInMovement : MovementState.OccuracyScale;
		var opponentDodge = target.Movement.NavAgentVelocity.magnitude > 0.2f
			? target.MovementState.DodgeScaleInMovement
			: target.MovementState.DodgeScale;

		var distanceScale = Mathf.Lerp(1f, 2f, 1f - distance / 20f);
		
		return occuracy * opponentDodge * distanceScale * obstacleChances;
	}

	public void SetTeam(TeamSkinPreset teamID)
	{
		Team = teamID;
		SetSelection(false);
	}

	public void SetSelection(bool active)
	{
		foreach (var outline in GetComponentsInChildren<Outline>())
		{
			outline.enabled = active;
		}
	}

	private void Awake()
	{
		Name = NameGenerator.GenerateUniqName();
		StateMachine = new PersonStateMachine(this);
		SetMovementState(PersonMovementState.Stand);
	}

	private void OnEnable()
	{
		Health.Changed += HealthChanged;
	}

	private void OnDisable()
	{
		Health.Changed -= HealthChanged;
	}

	private void HealthChanged()
	{
		if (IsAlive)
		{
			return;
		}

		StateMachine.SwitchToState(new DeathState());
		StateMachine.Dispose();
		StateMachine = null;
		Movement.Dispose();
		Died?.Invoke(this);
		enabled = false;
	}

	private void Update()
	{
		Health.Update(Time.deltaTime);
		StateMachine?.Update();

		var velocityScale = Movement.NavAgentVelocity.magnitude / Movement.MaxSpeed;
		var velocityDirection = Movement.NavAgentVelocity;
		var directionX = Vector3.Dot(transform.right, velocityDirection) * velocityScale;
		var directionY = Vector3.Dot(transform.forward, velocityDirection) * velocityScale;
		
		Animator.SetFloat("DirectionX", directionX);
		Animator.SetFloat("DirectionY", directionY);
	}

	public async UniTask RotateTo(Transform target, float duration = 0.5f)
	{
		var rotation = transform.rotation;
		var time = 0f;
		while (time < duration)
		{
			var targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
			transform.rotation = Quaternion.Lerp(rotation, targetRotation, time / duration);
			await UniTask.WaitForEndOfFrame(this);
			time += Time.deltaTime;
		}

		transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position);
	}

	public void AddWeapon(Gun weapon)
	{
		var slot = _weaponSlots.FirstOrDefault(slot => slot.Type == weapon.Type);
		if (slot == null)
		{
			throw new Exception("Can't find slot for type " + weapon.Type);
		}
		slot.Put(_gunFactory.Create(weapon.gameObject));
	}

	public async UniTask<bool> TakeWeapon()
	{
		var fullSlots = _weaponSlots.Where(slot => slot.IsEmpty == false).OrderBy(slot =>
		{
			switch (slot.Type)
			{
				case WeaponType.Handgun:
					return 2;
				case WeaponType.Rifle:
					return 1;
			}

			return 0;
		});

		if (fullSlots.IsEmpty())
		{
			return false;
		}

		Gun = fullSlots.First().Take();
		Gun.transform.SetParent(_weaponContainer);
		Gun.transform.localPosition = Vector3.zero;
		Gun.transform.localRotation = Quaternion.identity;
		Animator.SetTrigger("SwitchTo" + Gun.Type);
		await Task.Delay(500);
		return true;
	}

	public async Task PutWeaponBack()
	{
		if (Gun == null)
		{
			return;
		}
		Animator.SetTrigger("SwitchToEmpty");
		await Task.Delay(600);
		
		_weaponSlots.FirstOrDefault(slot => slot.Type == Gun.Type)?.Put(Gun);
		Gun = null;
	}
}