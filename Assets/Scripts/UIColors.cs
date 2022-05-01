using System.Linq;
using UnityEngine;

public enum Colors
{
	
}

[CreateAssetMenu(fileName = "MovementState", menuName = "ScriptableObjects/UIColors", order = 1)]
public class UIColors : ScriptableObject
{
	[SerializeField] private Color _wellColor;
	[SerializeField] private Color _normalColor;
	[SerializeField] private Color _warningColor;
	[SerializeField] private Color _dangerColor;
	[SerializeField] private Color _deadColor;
	
	public Color WellColor => _wellColor;
	public Color NormalColor => _normalColor;
	public Color WarningColor => _warningColor;
	public Color DangerColor => _dangerColor;
	public Color DeadColor => _deadColor;

	private static UIColors _instance;
	public static UIColors Instance
	{
		get
		{
			_instance ??= Resources.LoadAll<UIColors>(string.Empty).First();
			return _instance;
		}
	}
}