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

     private void DespawnBlocksBehindPlayer()
     {
          while (_activeBlocks.Count > 0 && _activeBlocks[0].transform.position.z <
                    _player.Position.z - _settings.fartherFromPlayerBehind)
               {
                    //Возврщаем препятствия
                    DespawnObstacles(_activeBlocks[0]);
                    //Возвращаем блок
                    _blockPool.Despawn(_activeBlocks[0]);
                    _activeBlocks.RemoveAt(0);
               }
     }

     private void DespawnObstacles(RoadBlock block)
     {
          _obstacleManager.ReturnObstacle(block.LeftLane.Obstacle);
          _obstacleManager.ReturnObstacle(block.MiddleLane.Obstacle);
          _obstacleManager.ReturnObstacle(block.RightLane.Obstacle);
     }

     private void SpawnBlocksInFrontOfPlayer(Vector3 startPos)
     {
          var pos = startPos;
          while ( pos.z < _player.Position.z + _settings.fartherFromPlayerForward)
          {
               var block = _blockPool.Spawn(pos);
               Obstacle.ObstacleType prevObstacle;
               for (int i = 0, streak; i < 3; i++)
               {
                    TryGetLaneLastObstacles(i, out prevObstacle, out streak);
                    
                    block[i].Obstacle =
                         _obstacleManager.ChooseAndGetObstacle(prevObstacle, streak, block[i].laneTransform);
               }
               _activeBlocks.Add(block);
               pos += Vector3.forward * _settings.blockDepth;
          }
     }
     

     private void TryGetLaneLastObstacles(int lane, out Obstacle.ObstacleType oType, out int streak)
     {
          streak = 1;
          if (_activeBlocks.Count > 0)
          {
               var laneObstacles = _activeBlocks.Select(b => b[lane].Obstacle.obstacleType).Reverse().ToArray();
               oType = laneObstacles.First();
               foreach (var obstacle in laneObstacles.Skip(1))
               {
                    if (obstacle == oType)
                         streak++;
                    else
                         break;
               }
          }
          else
               oType = Obstacle.ObstacleType.None;
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