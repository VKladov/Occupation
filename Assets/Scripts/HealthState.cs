using UnityEngine;

[CreateAssetMenu(fileName = "HealthState", menuName = "ScriptableObjects/HealthState", order = 1)]
public class HealthState : ScriptableObject
{
	[SerializeField] private float _minHealth;
	[SerializeField] private float _damagePerSecond;
	[SerializeField] private string _name;
	[SerializeField] private Color _textColor;
	
	public float MinHealth => _minHealth;
	public float DamagePerSecond => _damagePerSecond;
	public string Name => _name;
	public Color TextColor => _textColor;
}