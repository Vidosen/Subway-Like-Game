using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Signals;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

public class Player : MonoBehaviour
{
    [Inject] private readonly SignalBus _signalBus;
    
    [Inject] private MobileInputSystem _inputSystem;
    [Inject] private Settings _settings;
    [Inject] private BlockManager.Settings _blockSettings;
    private Rigidbody _rigidbody;
    

    public Vector3 Position => transform.position;
    
    public PlayerStates PlayerState { get; private set; }
    
    public bool IsStunned { get; private set; }
    
    private Vector3 _startPos;
    private int _currentLane;

    private float stunLeft;
    public bool IsPlaying
    {
        get => PlayerState == PlayerStates.Playing;
        set => PlayerState = value ? PlayerStates.Playing : PlayerStates.Awaiting;
    }
    
    void Start()
    {
        _signalBus.Fire<StartedAwaitingSignal>();
        _rigidbody = GetComponent<Rigidbody>();
        _startPos = transform.position;

    }

    private void OnEnable()
    {
        _signalBus.Subscribe<PlayerDiedSignal>(OnPlayerDied);
        _signalBus.Subscribe<StartedGameSignal>(OnStartedGame);
        _signalBus.Subscribe<StartedAwaitingSignal>(OnStartedAwaiting);
        _inputSystem.OnSwipeUpOccured += Jump;
    }

    private void OnStartedGame()
    {
        transform.position = _startPos;
        ChangeState(PlayerStates.Playing);
    }

    private void OnStartedAwaiting()
    {
        transform.position = _startPos;
        ChangeState(PlayerStates.Awaiting);
    }

    private void OnPlayerDied()
    {
        ChangeState(PlayerStates.Dead);
    }

    private void OnDisable()
    {
        _signalBus.Unsubscribe<PlayerDiedSignal>(OnPlayerDied);
        _signalBus.Unsubscribe<StartedGameSignal>(OnStartedGame);
        _signalBus.Unsubscribe<StartedAwaitingSignal>(OnStartedAwaiting);
        _inputSystem.OnSwipeUpOccured -= Jump;

    }

    private bool _isAirbone;

    private void OnCollisionStay(Collision other)
    {
        _isAirbone = false;
    }

    private void OnCollisionExit(Collision other)
    {
        _isAirbone = true;
    }

    private void Jump()
    {
        if (!_isAirbone)
        {
            var velocity = _rigidbody.velocity;
            velocity.y = Mathf.Sqrt(-2* Physics.gravity.y * _settings.jumpHeight);
            _rigidbody.velocity = velocity;
        }
    }

    private void TryDashLeft()
    {
        
    }

    private void TryDashRight()
    {
        
    }
    
    private void Update()
    {
        _rigidbody.velocity = (IsPlaying
            ? Vector3.forward : Vector3.zero) * _settings.playerStartSpeed + _rigidbody.velocity.y * Vector3.up;
    }
    

    private void ChangeState(PlayerStates newState)
    {
        PlayerState = newState;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeadZone"))
        {
            _signalBus.Fire<PlayerDiedSignal>();
        }

        if (other.CompareTag("Border"))
        {
            _signalBus.Fire<PlayerHitBorderSignal>();
        }
    }
    

    [Serializable]
    public class Settings
    {
        public float playerStartSpeed;
        public float jumpHeight;
        public float stunSpan;
    }
    
}

public enum PlayerStates
{
   Awaiting,
   Playing,
   Dead
    
}
