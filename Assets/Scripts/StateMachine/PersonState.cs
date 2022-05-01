using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class PersonState
{
	private CancellationTokenSource _cancellationTokenSource;
	protected Person Owner;
	protected Action OnComplete;
	protected CancellationToken CancellationToken;
	
	public void Enter(Person owner, Action complete)
	{
		Owner = owner;
		OnComplete = complete;
		_cancellationTokenSource = new CancellationTokenSource();
		CancellationToken = _cancellationTokenSource.Token;
		Enter();
	}

	public void Stop()
	{
		_cancellationTokenSource.Cancel();
		Exit();
	}

	protected void Complete()
	{
		_cancellationTokenSource.Cancel();
		Exit();
		OnComplete?.Invoke();
	}
	
	public abstract void Enter();
	public abstract void Update();
	public abstract void Exit();

	protected UniTask<bool> Delay(float seconds)
	{
		return UniTask.Delay(Mathf.CeilToInt(seconds * 1000f), cancellationToken: CancellationToken).SuppressCancellationThrow();
	}
}