using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;
using Random = UnityEngine.Random;

public class BlockManager : ITickable
{
     
     List<RoadBlock> _activeBlocks = new List<RoadBlock>();
     
     [Inject] private Settings _settings;
     [Inject] private ObstacleManager _obstacleManager;
     [Inject] private RoadBlock.Pool _blockPool;
     [Inject] private Player _player;               //Через mono компонент ZenjectBinding

     public bool IsActive { get; set; } = false;
     
     
     public void Tick()
     {
          if (IsActive)
          {
               DespawnBlocksBehindPlayer();
               Vector3 nextBlockPos;
               if (_activeBlocks.Count > 0)
                    nextBlockPos = _activeBlocks.Last().transform.position + Vector3.forward * _settings.blockDepth;
               else
                    nextBlockPos = _settings.startWithPos;

               SpawnBlocksInFrontOfPlayer(nextBlockPos);
          }
     }
     

     public void ClearBlocks()
     {
          foreach (var block in _activeBlocks)
          {
               DespawnObstacles(block);
               _blockPool.Despawn(block);
          }
          _activeBlocks.Clear();
     }

     void DespawnBlocksBehindPlayer()
     {
          while (_activeBlocks.Count > 0 && _activeBlocks[0].transform.position.z <
                    _player.Position.z - _settings.fartherFromPlayerBehind)
               {
                    //Возврщаем препятсвтия
                    DespawnObstacles(_activeBlocks[0]);
                    //Возвращаем блок
                    _blockPool.Despawn(_activeBlocks[0]);
                    _activeBlocks.RemoveAt(0);
               }

     }

     void DespawnObstacles(RoadBlock block)
     {
          _obstacleManager.ReturnObstacle(block.leftLane.Obstacle);
          _obstacleManager.ReturnObstacle(block.middleLane.Obstacle);
          _obstacleManager.ReturnObstacle(block.rightLane.Obstacle);
     }

     void SpawnBlocksInFrontOfPlayer(Vector3 startPos)
     {
          var pos = startPos;
          while ( pos.z < _player.Position.z + _settings.fartherFromPlayerForward)
          {
               var block = _blockPool.Spawn(pos);
               block.leftLane.Obstacle =
                    _obstacleManager.GetObstacle((Obstacle.ObstacleType)Random.Range(0, (int)Obstacle.ObstacleType.Count), block.leftLane.laneTransform);
               block.middleLane.Obstacle =
                    _obstacleManager.GetObstacle(Obstacle.ObstacleType.None, block.middleLane.laneTransform);
               block.rightLane.Obstacle =
                    _obstacleManager.GetObstacle((Obstacle.ObstacleType)Random.Range(0, (int)Obstacle.ObstacleType.Count), block.rightLane.laneTransform);
               
               _activeBlocks.Add(block);
               pos += Vector3.forward * _settings.blockDepth;
          }
     }
     
     [Serializable]
     public class Settings
     {
           public Vector3 startWithPos;
          public float blockDepth;
          public float fartherFromPlayerForward;
          public float fartherFromPlayerBehind;
          
     }
}