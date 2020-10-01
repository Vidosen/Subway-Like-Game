using System;
using UnityEngine;
using Zenject;

public class ObstacleInstaller : MonoInstaller
{
    public int InitialSize;
    public GameObject trainPrefabs;
    public GameObject rampPrefab;
    public BarrierPrefabs barrierPrefabs;
    public GameObject nonePool;
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<ObstacleManager>().AsSingle();

        Container.BindMemoryPool<Obstacle, Obstacle.TrainPool>()
            .WithInitialSize(InitialSize)
            .FromComponentInNewPrefab(trainPrefabs)
            .UnderTransformGroup("Trains");
        Container.BindMemoryPool<Obstacle, Obstacle.RampPool>()
            .WithInitialSize(InitialSize)
            .FromComponentInNewPrefab(rampPrefab)
            .UnderTransformGroup("Ramps");
        Container.BindMemoryPool<Obstacle, Obstacle.HighBarrierPool>()
            .WithInitialSize(InitialSize)
            .FromComponentInNewPrefab(barrierPrefabs.highBarrier)
            .UnderTransformGroup("High Barriers");
        Container.BindMemoryPool<Obstacle, Obstacle.MiddleBarrierPool>()
            .WithInitialSize(InitialSize)
            .FromComponentInNewPrefab(barrierPrefabs.middleBarrier)
            .UnderTransformGroup("Middle Barriers");
        Container.BindMemoryPool<Obstacle, Obstacle.LowBarrierPool>()
            .WithInitialSize(InitialSize)
            .FromComponentInNewPrefab(barrierPrefabs.lowBarrier)
            .UnderTransformGroup("Low Barriers");
        Container.BindMemoryPool<Obstacle, Obstacle.NonePool>()
            .WithInitialSize(InitialSize)
            .FromComponentInNewPrefab(nonePool)
            .UnderTransformGroup("Nones");
    }
    
    [Serializable]
    public class BarrierPrefabs
    {
        public GameObject highBarrier;
        public GameObject middleBarrier;
        public GameObject lowBarrier;
    }
}