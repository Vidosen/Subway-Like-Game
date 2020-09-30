using System;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class GameInstaller : MonoInstaller
    {
        public GameObject coinPrefab;
        [Space]
        public GameObject blockPrefabs;
        [Space]
        public GameObject trainPrefabs;
        public GameObject rampPrefab;
        public BarrierPrefabs barrierPrefabs;
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<BlockManager>().AsSingle();
            
            Container.BindMemoryPool<RoadBlock, RoadBlock.Pool>()
                .WithInitialSize(30)
                .FromComponentInNewPrefab(blockPrefabs)
                .UnderTransformGroup("Road Blocks");
            Container.BindMemoryPool<Coin, Coin.Pool>()
                .WithInitialSize(100)
                .FromComponentInNewPrefab(coinPrefab);
        }
        [Serializable]
        public class BarrierPrefabs
        {
            public GameObject highBarrier;
            public GameObject middleBarrier;
            public GameObject lowBarrier;
        }
    }
}