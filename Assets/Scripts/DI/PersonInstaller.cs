using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class PersonInstaller : MonoInstaller
{
	public override void InstallBindings()
	{
		Container.BindInterfacesAndSelfTo<PersonSenses>().AsSingle();
		Container.BindInterfacesAndSelfTo<PersonMovement>().AsSingle();

		Container.Bind<PersonHealth>().AsSingle();
		Container.Bind<Storage>().AsSingle();
		
		Container.Bind<Animator>().FromComponentOn(gameObject).AsSingle();
		Container.Bind<NavMeshAgent>().FromComponentOn(gameObject).AsSingle();
		Container.Bind<WeaponSlot>().FromComponentsInChildren();
		
		Container.Bind<Person>().FromComponentOn(gameObject).AsSingle().NonLazy();
		Container.Bind<SkinGenerator>().FromComponentOn(gameObject).AsSingle();
		Container.BindFactory<GameObject, Gun, Gun.Factory>().FromFactory<PrefabFactory<Gun>>();
	}
}