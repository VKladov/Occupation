using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "TeamSkinPreset", menuName = "ScriptableObjects/TeamSkinPreset", order = 1)]
public class TeamSkinPreset : ScriptableObject
{
	[SerializeField] private PersonSkinSource _skinPrefab;
	[SerializeField] private Gun _rifle;
	[SerializeField] private Gun _handgun;

	public PersonSkinSource SkinPrefab => _skinPrefab;
	public Gun Rifle => _rifle;
	public Gun Handgun => _handgun;
}