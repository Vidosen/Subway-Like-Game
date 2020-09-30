using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public Transform coinSpawn;

    public ObstacleType obstacleType;
    
    public enum ObstacleType
    {
        Train,
        Ramp,
        HighBarrier,
        MiddleBarrier,
        LowBarrier,
        None
    }
}

