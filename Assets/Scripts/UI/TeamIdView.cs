using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TeamIdView : MonoBehaviour
{
	[SerializeField] private TMP_Text _text;
	private Button _button;

	private void Awake()
	{
		_button = GetComponent<Button>();
		TeamIDOnChanged();
	}

	private void OnEnable()
	{
		TempData.TeamID.Changed += TeamIDOnChanged;
		_button.onClick.AddListener(OnClick);
	}

	private void OnDisable()
	{
		TempData.TeamID.Changed -= TeamIDOnChanged;
		_button.onClick.RemoveListener(OnClick);
	}

	private void TeamIDOnChanged()
	{
		_text.text = $"Team: {TempData.TeamID.Value}";
	}

	private void OnClick()
	{
		TempData.TeamID.Value = TempData.TeamID.Value == 0 ? 1 : 0;
	}
}