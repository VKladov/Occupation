using UnityEngine;
using Zenject;

[RequireComponent(typeof(Canvas))]
public class GameInterfaceInstaller : MonoInstaller<GameInterfaceInstaller>
{
	public override void InstallBindings()
	{
		Container.Bind<Canvas>().FromInstance(GetComponent<Canvas>());
		Container.Bind<RectSelectionView>().FromComponentInChildren().AsSingle();
	}
}