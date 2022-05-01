using DefaultNamespace;
using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    [SerializeField] private GameObject _controlsPrefab;
    [SerializeField] private GameObject _personSpawnerPrefab;
    public override void InstallBindings()
    {
        Container.Bind<ClickPicker>().FromComponentInNewPrefab(_controlsPrefab).AsSingle();
        Container.Bind<SpawnPerson>().FromComponentInNewPrefab(_personSpawnerPrefab).AsSingle().NonLazy();
        Container.Bind<Canvas>().FromComponentInHierarchy().AsSingle();
        Container.Bind<RectSelectionView>().FromComponentInHierarchy().AsSingle();
        Container.Bind<SendPersonToPoint>().FromNew().AsSingle().NonLazy();
        Container.Bind<PersonSelection>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.Bind<SelectedPersonHighlight>().FromNew().AsSingle().NonLazy();
    }
}