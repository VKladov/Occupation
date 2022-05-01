using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementState", menuName = "ScriptableObjects/MovementState", order = 1)]
public class PersonMovementState : ScriptableObject
{
	[SerializeField] private float _maxMovementSpeed;
	[SerializeField] private float _occuracyScale;
	[SerializeField] private float _occuracyScaleInMovement;
	[SerializeField] private float _dodgeScale;
	[SerializeField] private float _dodgeScaleInMovement;
	[SerializeField] private string _animatorPrefix;
	
	public float MaxMovementSpeed => _maxMovementSpeed;
	public float OccuracyScale => _occuracyScale;
	public float OccuracyScaleInMovement => _occuracyScaleInMovement;
	public float DodgeScale => _dodgeScale;
	public float DodgeScaleInMovement => _dodgeScaleInMovement;
	public string AnimatorPrefix => _animatorPrefix;

	private static Dictionary<string, PersonMovementState> AllCached;

	public static Dictionary<string, PersonMovementState> Map
	{
		get
		{
			if (AllCached == null)
			{
				AllCached = new Dictionary<string, PersonMovementState>();
				var all = Resources.LoadAll<PersonMovementState>(string.Empty);
				foreach (var movementState in all)
				{
					AllCached[movementState.name] = movementState;
				}
			}

			return AllCached;
		}
	}

	public static List<PersonMovementState> All => Map.Values.ToList();
	
	public static PersonMovementState Stand => Map["StandState"];
	public static PersonMovementState Prone => Map["ProneState"];
	public static PersonMovementState Crouch => Map["CrouchState"];
}