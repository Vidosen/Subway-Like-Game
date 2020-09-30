using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class BlockManager : IInitializable, ITickable
{
     List<RoadBlock> _activeBlocks = new List<RoadBlock>();
     
     [Inject] private Settings _settings;
     [Inject] private RoadBlock.Pool _blockPool;
     [Inject] private Player _player;               //Через mono компонент ZenjectBinding


     public void Initialize()
     {
          SpawnBlocksInFrontOfPlayer(_settings.StartWithPos);
          Debug.Log("Block Manager Started");
     }

     public void Tick()
     {
          DespawnBlocksBehindPlayer();
          SpawnBlocksInFrontOfPlayer(_activeBlocks.Last().transform.position +
                                     Vector3.forward * _settings.BlockDepth);
          //throw new System.NotImplementedException();
     }

     void DespawnBlocksBehindPlayer()
     {

               while (_activeBlocks.Count > 0 && _activeBlocks[0].transform.position.z <
                    _player.Position.z - _settings.FartherFromPlayerBehind)
               {
                    _blockPool.Despawn(_activeBlocks[0]);
                    _activeBlocks.RemoveAt(0);
               }

     }

     void SpawnBlocksInFrontOfPlayer(Vector3 startPos)
     {
          var pos = startPos;
          while ( pos.z < _player.Position.z + _settings.FartherFromPlayerForward)
          {
               var block = _blockPool.Spawn(pos, null, null, null);
               _activeBlocks.Add(block);
               pos += Vector3.forward * _settings.BlockDepth;
          }
     }
     
     [Serializable]
     public class Settings
     {
          public Vector3 StartWithPos;
          public float BlockDepth;
          public float FartherFromPlayerForward;
          public float FartherFromPlayerBehind;
          
     }
}