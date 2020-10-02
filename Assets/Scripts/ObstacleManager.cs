using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Zenject;


using static Obstacle;
using Random = UnityEngine.Random;

public class ObstacleManager : IInitializable
{
    [Inject] private TrainPool _trainPool;
    [Inject] private RampPool _rampPool;
    [Inject] private HighBarrierPool _highBarrierPool;
    [Inject] private MiddleBarrierPool _middleBarrierPool;
    [Inject] private LowBarrierPool _lowBarrierPool;
    [Inject] private NonePool _nonePool;

    [Inject] private Settings _settings;

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

    public Obstacle ChooseAndGetObstacle(ObstacleType prevObstacle, int streak, Transform parent)
    {
        return GetObstacle(ChooseObstacleType(prevObstacle, streak), parent);
    }
    
    public void ReturnObstacle(Obstacle obstacle)
    {
        var pool = _pools[obstacle.obstacleType];
        pool.Despawn(obstacle);
    }

    public ObstacleType ChooseObstacleType(ObstacleType prevObstacle, int streak)
    {
        float rampChance = 0f,
            trainChance = 0f,
            noneChance = 0f,
            barrierChance = 0f;
        
        switch (prevObstacle)
        {
            case ObstacleType.Ramp:
            {
                return ObstacleType.Train;
                trainChance = 1f;
                break;
            }
            case ObstacleType.Train:
            {
                trainChance = _settings.trainMaxChance - LimitToOne(streak) * _settings.trainMaxChance;
                noneChance = 1f - trainChance;
                break;
            }
            case ObstacleType.HighBarrier:
            case ObstacleType.MiddleBarrier:
            case ObstacleType.LowBarrier:
            {
                return ObstacleType.None;
                noneChance = 1f;
                break;
            }
            case ObstacleType.None:
            {
                noneChance = _settings.noneMaxChance - LimitToOne(streak) * _settings.noneMaxChance;
                var leftOver = 1f - noneChance;
                var coeff = (_settings.trainBaseChance + _settings.rampBaseChance + _settings.barrierBaseChance) /
                            leftOver;
                trainChance = _settings.trainBaseChance / coeff;
                rampChance = _settings.rampBaseChance / coeff;
                barrierChance = _settings.barrierBaseChance / coeff;
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
        var randValue = Random.value;
        
        if (randValue < rampChance)
            return ObstacleType.Ramp;
        
        randValue -= rampChance;
        
        if (randValue < trainChance)
            return ObstacleType.Train;
        
        randValue -= trainChance;
        
        if (randValue < barrierChance)
            return ChooseBarrierType();
        else
            return ObstacleType.None;
    }

    float LimitToOne(int x)
    {
        int maxStreak = 8;
        //return Mathf.Sqrt(Mathf.InverseLerp(1, maxStreak, x));
        return Mathf.Pow(Mathf.InverseLerp(1, maxStreak, x), 1.5f);
    }

    private ObstacleType ChooseBarrierType()
    {
        return (ObstacleType) Random.Range(2, 5);
    }

    [Serializable]
    public class Settings
    {
        public float rampBaseChance;
        [Space]
        public float trainBaseChance;
        public float trainMaxChance;
        [Space]
        public float barrierBaseChance;
        [Space]
        public float noneMaxChance;
    }
}
