using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Weapon", order = 1)]
public class Weapon : InventoryItem
{
	[SerializeField] private Gun _prefab;
	[SerializeField] private WeaponType _type;

	public Gun Prefab => _prefab;
	public WeaponType Type => _type;
}