using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PersonMovement))]
[RequireComponent(typeof(PersonGenerator))]
[RequireComponent(typeof(Animator))]
public class Person : MonoBehaviour
{
	public event Action<Person> Died;
	public event Action<Person> TookShot;
	public event Action<Building> EnteredBuilding;

	[SerializeField] private float _hungerSpeed;
	[SerializeField] private float _hungerDamageSpeed;
	[SerializeField] private Building _house;
	[SerializeField] private LayerMask _obstaclesLayer;
	[SerializeField] private ParticleSystem _bloodEffect;

	public string Name { get; private set; }
	public int TeamID { get; private set; }
	public PersonMovement Movement { get; private set; }
	public Animator Animator { get; private set; }
	public PersonStateMachine StateMachine { get; private set; }
	public PersonSenses Senses { get; private set; }
	public Vector3 MiddlePoint => transform.position + Vector3.up;

	public PersonHealth Health  { get; private set; }
	public readonly PersonStats Stats = new PersonStats();
	public readonly Storage Bag = new Storage();

	public bool IsAlive => Health.NormalizedValue > 0f;
	public Person[] AllOtherPeople => FindObjectsOfType<Person>().Where(item => item.transform.root != transform.root).ToArray();
	public Gun Gun;

	private PersonGenerator _personGenerator;
	private Outline[] _outlines;
	public Vector3 MainTarget { get; private set; }
	public bool HasMainTarget { get; set; }
	public PersonMovementState MovementState { get; private set; }
	public Hospital Hospital => FindObjectOfType<Hospital>();
	public RestPoint RestPoint => FindObjectOfType<RestPoint>();

	private PersonState _task;

	public void HandleBuildingEnter(Building building)
	{
		EnteredBuilding?.Invoke(building);
	}

	public void SetMainObjective(Vector3 targetPosition)
	{
		MainTarget = targetPosition;
		HasMainTarget = true;
		StateMachine?.SetObjective(targetPosition);
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

	public void TakeShot(Person attacker)
	{
		if (!IsAlive)
		{
			return;
		}
		
		_bloodEffect.transform.rotation = Quaternion.LookRotation(transform.position - attacker.transform.position);
		_bloodEffect.Play();
		Animator.SetTrigger(MovementState.AnimatorPrefix + "_Infantry_Damage");
		Health.Change(-10);
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

	public void SetTeam(int teamID)
	{
		TeamID = teamID;
		_personGenerator.GenerateForTeam(teamID);
		_outlines = GetComponentsInChildren<Outline>();
		SetSelection(false);
	}

	public void SetSelection(bool active)
	{
		foreach (var outline in _outlines)
		{
			outline.enabled = active;
		}
	}

	private void Awake()
	{
		Movement = GetComponent<PersonMovement>();
		Animator = GetComponent<Animator>();
		_personGenerator = GetComponent<PersonGenerator>();
		Name = NameGenerator.GenerateUniqName();
		Health = new PersonHealth();
	}

	private void OnEnable()
	{
		Health.Changed += HealthChanged;
		Health.StateChanged += HealthStateChanged;
	}

	private void OnDisable()
	{
		Health.Changed -= HealthChanged;
		Health.StateChanged -= HealthStateChanged;
	}

	private void HealthStateChanged()
	{
		
	}

	private void HealthChanged()
	{
		if (IsAlive)
		{
			return;
		}

		StateMachine.SwitchToState(new DeathState());
		Movement.enabled = false;
		Died?.Invoke(this);
		enabled = false;
	}

	private void Start()
	{
		Senses = new PersonSenses(this, _obstaclesLayer);
		Gun = GetComponentsInChildren<Gun>().FirstOrDefault(item => item.isActiveAndEnabled);
		SetMainObjective(transform.position);
		StateMachine = new PersonStateMachine(this, TeamID == 0 ?
				StrategySource.GetDefenceStrategy(this) :
				StrategySource.GetAttackStrategy(this));
		SetMovementState(PersonMovementState.Stand);
	}

	private void Update()
	{
		Health.Update(Time.deltaTime);
		StateMachine?.Update();
		
		Animator.SetFloat("DirectionX", Vector3.Dot(transform.right, Movement.NavAgentVelocity));
		Animator.SetFloat("DirectionY", Vector3.Dot(transform.forward, Movement.NavAgentVelocity));
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

	public async UniTask RotateTo(Vector3 targetDirection, float duration = 0.5f)
	{
		var rotation = transform.rotation;
		var targetRotation = Quaternion.LookRotation(targetDirection);
		var time = 0f;
		while (time < duration)
		{
			transform.rotation = Quaternion.Lerp(rotation, targetRotation, time / duration);
			await UniTask.WaitForEndOfFrame(this);
			time += Time.deltaTime;
		}
		
		transform.rotation = targetRotation;
	}
}