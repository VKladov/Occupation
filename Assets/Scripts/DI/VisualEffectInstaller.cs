using UnityEngine;
using Zenject;

public class VisualEffectInstaller : MonoInstaller
{
	[SerializeField] private VisualEffect _shotPrefab;
	[SerializeField] private VisualEffect _bloodShotPrefab;

	public override void InstallBindings()
	{
		
	}
}