using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class RoadBlock : MonoBehaviour
{

    [SerializeField] private Lane[] lanes;
    public Lane LeftLane => lanes[0];
    public Lane MiddleLane => lanes[1];
    public Lane RightLane => lanes[2];

    public Lane this[int key] => lanes[key];

    [Inject]
    public void Construct()
    {
        Reset(transform.position);
    }

    private void Reset(Vector3 pos)
    {

        transform.position = pos;
    }


    public class Pool : MonoMemoryPool<Vector3, RoadBlock>
    {
        protected override void Reinitialize(Vector3 pos, RoadBlock block)
        {
            block.Reset(pos);
        }
    }
}

[Serializable]
public class Lane
{
    
    public Transform laneTransform;
    public Obstacle Obstacle;
}


/*public class ObstacleFactory : IFactory<Lane.LaneParams, IObstacle>
{
    DiContainer _container;
    public ObstacleFactory(DiContainer container)
    {
        _container = container;
    }
    public IObstacle Create(Lane.LaneParams _params)
    {
        if (WillBeTrain(_params))
        {
            return _container.Instantiate<Train>();
        }else if (WillBeRamp(_params))
        {
            return _container.Instantiate<Ramp>();
        }else if (WillBeBarrier(_params))
        {
            switch (Random.Range(0, 3))
            {
                case 0: return _container.Instantiate<HighBarrier>();
                case 1: return _container.Instantiate<MiddleBarrier>();
                case 2: return _container.Instantiate<LowBarrier>();
                default: throw new Exception();
            }
        }
        else
        {
            return _container.Instantiate<FreeSpace>();
        }
    }*/

    /*bool WillBeTrain(Lane.LaneParams _params)
    {
        //Если два соседа поезда, то сам не может быть поездом
        if (_params.neighbor1 != null && _params.neighbor2 != null &&
            _params.neighbor1.Obstacle is Train && _params.neighbor1.Obstacle is Train)
        {
            return false;
        }
        //Если сздали был подъём, то сейчас обязательно должен быть поезд
        if (_params.prevLanes.Last().Obstacle is Ramp)
        {
            return true;
        }

        var fromNearest = _params.prevLanes.Reverse();
        if (fromNearest.First().Obstacle is Train)
        {
            int row = 0;
            foreach (var lane in fromNearest)
            {
                if (!(lane.Obstacle is Train))
                {
                    break;
                }
                row++;
            }

            if (Random.value < 0.6f - 0.1f * row)
            {
                return true;
            }
            
        }
    }*/


