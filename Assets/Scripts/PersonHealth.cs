using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PersonHealth : Stat
{
	public event Action<bool> HealStateChanged; 
	public class HealthState : IStatState
	{
		public float MinHealth;
		public float DamagePerSecond;
		public string Name { get; set; }
		public Color TextColor { get; set; }
	}

	private List<HealthState> States = new List<HealthState>
	{
		new HealthState
		{
			Name = "Здоров",
			MinHealth = 90f,
			DamagePerSecond = 0f,
			TextColor = UIColors.Instance.WellColor
		},
		new HealthState
		{
			Name = "Ранен",
			MinHealth = 50f,
			DamagePerSecond = 0.5f,
			TextColor = UIColors.Instance.WarningColor
		},
		new HealthState
		{
			Name = "Смертельно ранен",
			MinHealth = 0f,
			DamagePerSecond = 1f,
			TextColor = UIColors.Instance.DangerColor
		},
		new HealthState
		{
			Name = "Мертв",
			MinHealth = float.MinValue,
			DamagePerSecond = 0f,
			TextColor = UIColors.Instance.DeadColor
		}
	};

	public override float MaxValue => 100f;
	public override IStatState State => _state;
	public string Name => "Здоровье";

	private float Health
	{
		get => Value;
		set
		{
			var changed = value != Value;
			Value = value;
			if (changed)
				Changed?.Invoke();
		}
	}

	private bool _isHealing;
	public bool IsHealing
	{
		get => _isHealing;
		set
		{
			var changed = _isHealing != value;
			_isHealing = value;
			if (changed)
			{
				HealStateChanged?.Invoke(value);
			}
		}
	}
	
	private HealthState _state;
	private const float RecoveryPerSecond = 5f;
	private float _healTarget;

	public PersonHealth()
	{
		Health = MaxValue;
		UpdateState();
	}

	public void TakeHeal()
	{
		if (Health >= MaxValue)
		{
			return;
		}
		
		IsHealing = true;
		var currentIndex = States.IndexOf(_state);
		var targetIndex = currentIndex - 2;
		_healTarget = targetIndex >= 0 ? (States[targetIndex].MinHealth - 0.1f) : MaxValue;
	}

	public override void Change(float delta)
	{
		Health += delta;
		UpdateState();

		if (delta < 0)
		{
			IsHealing = false;
		}
	}
	public override void Update(float deltaTime)
	{
		if (IsHealing)
		{
			Health += deltaTime * RecoveryPerSecond;
			if (_healTarget <= Health)
			{
				Health = _healTarget;
				IsHealing = false;
			}
			UpdateState();
		}
		else
		{
			Health -= deltaTime * _state.DamagePerSecond;
			if (Health < _state.MinHealth)
			{
				UpdateState();
			}
		}
	}

	private void UpdateState()
	{
		var oldState = _state;
		_state = States.First(state => state.MinHealth <= Health);
		if (oldState != _state)
		{
			StateChanged?.Invoke();
		}
	}
}