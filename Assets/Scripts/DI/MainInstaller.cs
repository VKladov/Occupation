using DefaultNamespace;
using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    [Header("Layers")]
    [SerializeField] private LayerMask _personLayerMask;
    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private LayerMask _buildingLayerMask;
    [SerializeField] private LayerMask _obstaclesLayerMask;
    
    [Header("Person")]
    [SerializeField] private Person _personPrefab;
    public override void InstallBindings()
    {
        // LayerMasks
        Container.Bind<LayerMask>().WithId(StringConstants.PersonLayer).FromInstance(_personLayerMask).CopyIntoAllSubContainers();
        Container.Bind<LayerMask>().WithId(StringConstants.GroundLayer).FromInstance(_groundLayerMask).CopyIntoAllSubContainers();
        Container.Bind<LayerMask>().WithId(StringConstants.BuildingLayer).FromInstance(_buildingLayerMask).CopyIntoAllSubContainers();
        Container.Bind<LayerMask>().WithId(StringConstants.ObstaclesLayer).FromInstance(_obstaclesLayerMask).CopyIntoAllSubContainers();

        Container.Bind<Camera>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<ClickPicker>().AsSingle();
        Container.BindInterfacesAndSelfTo<SpawnPerson>().AsSingle();
        Container.Bind<Canvas>().FromComponentInHierarchy().AsSingle();
        Container.Bind<RectSelectionView>().FromComponentInHierarchy().AsSingle();
        Container.Bind<SendPersonToPoint>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<PersonSelection>().AsSingle();
        Container.Bind<SelectedPersonHighlight>().AsSingle().NonLazy();
        Container.BindFactory<Person, Person.Factory>().FromSubContainerResolve().ByNewContextPrefab(_personPrefab);
    }
}