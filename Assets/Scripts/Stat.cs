using System;
using UnityEngine;

public abstract class Stat
{
	public Action StateChanged;
	public Action Changed;
	public string Name { get; protected set; }
	public float NormalizedValue => Value / MaxValue;
	public float Value { get; protected set; }
	public abstract float MaxValue { get; }
	public abstract IStatState State { get; }
	public abstract void Change(float delta);
	public abstract void Update(float deltaTime);
}

public interface IStatState
{
	string Name { get; }
	Color TextColor { get; }
}