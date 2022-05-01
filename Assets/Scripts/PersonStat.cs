using System;
using UnityEngine;

public class PersonStat : Stat
{
	public float Value { get; private set;  }
	private readonly float _maxValue;
	public readonly float MinValue;
	private Action _changed;

	public class PersonStatState : IStatState
	{
		public string Name { get; set; }
		public Color TextColor { get; }
	}

	public override float MaxValue => _maxValue;
	public override IStatState State => new PersonStatState { Name = Name };

	public PersonStat(string name, float maxValue, float minValue = 0f)
	{
		Name = name;
		Value = maxValue;
		_maxValue = maxValue;
		MinValue = minValue;
	}

	public override void Change(float delta)
	{
		var newValue = Mathf.Clamp(Value + delta, MinValue, MaxValue);
		var changed = Math.Abs(newValue - Value) > 0.00001f;
		Value = newValue;
		
		if (changed)
		{
			Changed?.Invoke();
		}
	}

	public override void Update(float deltaTime)
	{
		
	}
}