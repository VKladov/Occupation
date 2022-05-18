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
    [SerializeField] private TeamSkinPreset _russianArmy;
    [SerializeField] private TeamSkinPreset _ukranianArmy;
    [SerializeField] private TeamSkinPreset _citizen;
    
    [Header("Effects")]
    [SerializeField] private VisualEffect _shotPrefab;
    [SerializeField] private VisualEffect _bloodShotPrefab;
    
    public override void InstallBindings()
    {
        // LayerMasks
        Container.Bind<LayerMask>().WithId(ID.PersonLayer).FromInstance(_personLayerMask).CopyIntoAllSubContainers();
        Container.Bind<LayerMask>().WithId(ID.GroundLayer).FromInstance(_groundLayerMask).CopyIntoAllSubContainers();
        Container.Bind<LayerMask>().WithId(ID.BuildingLayer).FromInstance(_buildingLayerMask).CopyIntoAllSubContainers();
        Container.Bind<LayerMask>().WithId(ID.ObstaclesLayer).FromInstance(_obstaclesLayerMask).CopyIntoAllSubContainers();

        Container.Bind<Camera>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<ClickPicker>().AsSingle();
        Container.BindInterfacesAndSelfTo<SpawnPerson>().AsSingle();
        Container.Bind<Canvas>().FromComponentInHierarchy().AsSingle();
        Container.Bind<RectSelectionView>().FromComponentInHierarchy().AsSingle();
        Container.Bind<SendPersonToPoint>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<PersonSelection>().AsSingle();
        Container.Bind<SelectedPersonHighlight>().AsSingle().NonLazy();
        Container.BindFactory<Person, Person.Factory>().FromSubContainerResolve().ByNewContextPrefab(_personPrefab);

        Container.Bind<VisualEffects>().AsSingle();
        Container.BindMemoryPool<VisualEffect, VisualEffect.Pool>().WithId(ID.ShotEffectPool).FromComponentInNewPrefab(_shotPrefab);
        Container.BindMemoryPool<VisualEffect, VisualEffect.Pool>().WithId(ID.BloodShootEffectPool).FromComponentInNewPrefab(_bloodShotPrefab);
        Container.Bind<TeamSkinPreset>().WithId(Team.RussianArmy).FromInstance(_russianArmy);
        Container.Bind<TeamSkinPreset>().WithId(Team.UkranianArmy).FromInstance(_ukranianArmy);
        Container.Bind<TeamSkinPreset>().WithId(Team.Citizen).FromInstance(_citizen);
    }
}