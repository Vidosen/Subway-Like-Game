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
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<BlockManager>().AsSingle();
            
            Container.BindMemoryPool<RoadBlock, RoadBlock.Pool>()
                .WithInitialSize(30)
                .FromComponentInNewPrefab(blockPrefabs)
                .UnderTransformGroup("Pooled RoadBlocks");
            Container.BindMemoryPool<Coin, Coin.Pool>()
                .WithInitialSize(100)
                .FromComponentInNewPrefab(coinPrefab)
                .UnderTransformGroup("Pooled Coins");
        }
    }
}