using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Zenject;


using static Obstacle;

public class ObstacleManager : IInitializable
{
    [Inject]
    private TrainPool _trainPool;
    [Inject]
    private RampPool _rampPool;
    [Inject]
    private HighBarrierPool _highBarrierPool;
    [Inject]
    private MiddleBarrierPool _middleBarrierPool;
    [Inject]
    private LowBarrierPool _lowBarrierPool;
    [Inject]
    private NonePool _nonePool;

    private Dictionary<ObstacleType, ObstaclePool> _pools =
        new Dictionary<ObstacleType, ObstaclePool>();
    
    public void Initialize()
    {
        _pools.Add(ObstacleType.Train,_trainPool);
        _pools.Add(ObstacleType.Ramp, _rampPool);
        _pools.Add(ObstacleType.HighBarrier, _highBarrierPool);
        _pools.Add(ObstacleType.MiddleBarrier, _middleBarrierPool);
        _pools.Add(ObstacleType.LowBarrier, _lowBarrierPool);
        _pools.Add(ObstacleType.None, _nonePool);
    }
    public Obstacle GetObstacle(ObstacleType obstacleType, Transform parent)
    {
        return _pools[obstacleType].Spawn(parent);
    }

    public void ReturnObstacle(Obstacle obstacle)
    {
        var pool = _pools[obstacle.obstacleType];
        pool.Despawn(obstacle);
    }


}
