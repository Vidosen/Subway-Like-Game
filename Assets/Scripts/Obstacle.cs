using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Obstacle : MonoBehaviour
{
    public Transform coinSpawn;

    public ObstacleType obstacleType;


    private void Reset(Transform parent)
    {
        Transform _transform;
        (_transform = transform).SetParent(parent);
        _transform.localPosition = Vector3.zero;
    }

    public enum ObstacleType
    {
        Train,
        Ramp,
        HighBarrier,
        MiddleBarrier,
        LowBarrier,
        None,
        Count
    }

    public abstract class ObstaclePool : MonoMemoryPool<Transform, Obstacle>
    {
        protected override void Reinitialize(Transform parent, Obstacle item)
        {
            item.Reset(parent);
        }
    }
    
    public class TrainPool : ObstaclePool { }
    public class RampPool : ObstaclePool { }
    public class HighBarrierPool : ObstaclePool { }
    public class MiddleBarrierPool : ObstaclePool { }
    public class LowBarrierPool : ObstaclePool { }
    public class NonePool : ObstaclePool { }
}

