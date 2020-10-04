using System;
using Signals;
using UnityEngine;
using Zenject;

namespace DefaultNamespace
{
    public class PlayerAnimation : MonoBehaviour
    {
        [Inject] readonly SignalBus _signalBus;
        [SerializeField]private MeshRenderer _renderer;
        [SerializeField]private Material playerActive;
        [SerializeField]private Material playerStunned;
        [SerializeField]private Material playerDead;

        private void Start()
        {
            _signalBus.Subscribe<PlayerDiedSignal>(OnPlayerDied);
            _signalBus.Subscribe<StartedAwaitingSignal>(OnPlayerAlive);
            _signalBus.Subscribe<StartedGameSignal>(OnPlayerAlive);
           // _signalBus.Subscribe<CollectedCoinSignal>();
        }

        void OnPlayerDied()
        {
            _renderer.material = playerDead;
        }

        void OnPlayerAlive()
        {
            _renderer.material = playerActive;
        }

        void OnPlayerHitBarrier()
        {
            _renderer.material = playerStunned;
        }
    }
}