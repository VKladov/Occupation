using System;
using UnityEngine;

public class PersonStateMachine : IDisposable
{
	private PersonStrategy _strategy;
	private PersonState _state;
	private readonly Person _person;
	
	public PersonStateMachine(Person person, PersonStrategy strategy)
	{
		_person = person;
		_strategy = strategy;
	}

	public void Update()
	{
		if (_state != null)
		{
			_state?.Update();
		}
		else
		{
			_strategy?.Update();
		}
	}

	public void SwitchToState(PersonState nextState)
	{
		if (_state != null && nextState.GetType() == _state.GetType())
		{
			return;
		}
		
		_state?.Stop();
		_state = nextState;
		_state.Enter(_person, OnStateComplete);
	}

	public void SetObjective(Vector3 objective)
	{
		_strategy?.SetTargetPoint(objective);
	}

	public void ChangeStrategy(PersonStrategy strategy)
	{
		_state?.Stop();
		_strategy?.Dispose();
		_strategy = strategy;
	}

	private void OnStateComplete()
	{
		_state = null;
	}

	public void Dispose()
	{
		_state?.Stop();
		_strategy?.Dispose();
	}
}