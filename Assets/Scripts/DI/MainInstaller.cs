using Buildings;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.AI;
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
    [SerializeField] private TeamSkinPreset _russianArmy;
    [SerializeField] private TeamSkinPreset _ukranianArmy;
    [SerializeField] private TeamSkinPreset _citizen;
    
    [Header("Effects")]
    [SerializeField] private VisualEffect _shotPrefab;
    [SerializeField] private VisualEffect _bloodShotPrefab;

    [Header("NavMeshAgents")] 
    [SerializeField] private NavMeshAgent _footpath;
    [SerializeField] private NavMeshAgent _everywhere;
    [SerializeField] private NavMeshAgent _road;
    
    public override void InstallBindings()
    {
        Container.Bind<Constants>().FromMethod(CreateConstantsInstance).AsSingle();
        Container.Bind<Camera>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<ClickPicker>().AsSingle();
        Container.BindInterfacesAndSelfTo<SpawnPerson>().AsSingle();
        Container.Bind<Canvas>().FromComponentInHierarchy().AsSingle();
        Container.Bind<RectSelectionView>().FromComponentInHierarchy().AsSingle();
        Container.Bind<SendPersonToPoint>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<PersonSelection>().AsSingle();
        Container.Bind<SelectedPersonHighlight>().AsSingle().NonLazy();
        Container.BindFactory<Person, Person.Factory>().FromSubContainerResolve().ByNewContextPrefab(_personPrefab);
        Container.Bind<City>().AsSingle();
        Container.Bind<House>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<Supermarket>().FromComponentsInHierarchy().AsSingle();

        Container.Bind<VisualEffects>().AsSingle();
        Container.BindMemoryPool<VisualEffect, VisualEffect.Pool>().WithId(ID.ShotEffectPool).FromComponentInNewPrefab(_shotPrefab);
        Container.BindMemoryPool<VisualEffect, VisualEffect.Pool>().WithId(ID.BloodShootEffectPool).FromComponentInNewPrefab(_bloodShotPrefab);
        Container.Bind<TeamSkinPreset>().WithId(Team.RussianArmy).FromInstance(_russianArmy);
        Container.Bind<TeamSkinPreset>().WithId(Team.UkranianArmy).FromInstance(_ukranianArmy);
        Container.Bind<TeamSkinPreset>().WithId(Team.Citizen).FromInstance(_citizen);
    }

    private Constants CreateConstantsInstance()
    {
        return new Constants(
            new Constants.LayerMasksData(_personLayerMask, _groundLayerMask, _buildingLayerMask, _obstaclesLayerMask),
            new Constants.NavMeshAreaMasksData(_footpath.areaMask, _road.areaMask, _everywhere.areaMask)
        );
    }
}