using System;
using System.Collections;
using Signals;
using UnityEngine;
using Zenject;

public class PlayerMovement : MonoBehaviour
{
    public float currentSpeed;
    
    [Inject] private readonly SignalBus _signalBus;
    
    [Inject] private MobileInputSystem _inputSystem;
    [Inject] private Settings _settings;
    private Rigidbody _rigidbody;
    private CapsuleCollider _capsuleCollider;
    

    public Vector3 Position => transform.position;
    
    public PlayerStates PlayerState { get; private set; }
    
    public bool IsStunned { get; private set; }
    private float stunTimer;
    
    private Vector3 _startPos;
    private int _currentLane;
    private int _targetLane;

    private float _stunLeft;
    private bool _isAirbone;
    
    private void OnCollisionStay(Collision other)
    {
        _isAirbone = false;
    }

    private void OnCollisionExit(Collision other)
    {
        _isAirbone = true;
    }
    
    public bool IsPlaying
    {
        get => PlayerState == PlayerStates.Playing;
        set => PlayerState = value ? PlayerStates.Playing : PlayerStates.Awaiting;
    }
    
    void Start()
    {
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _rigidbody = GetComponent<Rigidbody>();
        _startPos = transform.position;

    }

    private void OnEnable()
    {
        _signalBus.Subscribe<PlayerDiedSignal>(OnPlayerDied);
        _signalBus.Subscribe<StartedGameSignal>(OnStartedGame);
        _signalBus.Subscribe<StartedAwaitingSignal>(OnStartedAwaiting);
        _signalBus.Subscribe<PlayerHitBorderSignal>(OnHitBorder);
        _inputSystem.OnSwipeUpOccured += Jump;
        _inputSystem.OnSwipeLeftOccured += MoveLeft;
        _inputSystem.OnSwipeRightOccured += MoveRight;
        _inputSystem.OnSwipeDownOccured += Prone;
    }

    private void OnStartedGame()
    {
        _targetLane = _currentLane = 0;
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
        _signalBus.Unsubscribe<PlayerHitBorderSignal>(OnHitBorder);
        _inputSystem.OnSwipeUpOccured -= Jump;
        _inputSystem.OnSwipeLeftOccured -= MoveLeft;
        _inputSystem.OnSwipeRightOccured -= MoveRight;
        _inputSystem.OnSwipeDownOccured -= Prone;

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

    private void FixedUpdate()
    {
        ApplyForwardMovement();
        ApplyLaneMovement();
    }
    

    void ApplyForwardMovement()
    {
        if (IsPlaying)
        {
            _rigidbody.position += currentSpeed * Time.fixedDeltaTime * Vector3.forward;
        }
    }

    void ApplyLaneMovement()
    {
        if (_currentLane !=_targetLane)
        {
            var currentPos = _rigidbody.position;
            var targetPos = currentPos;
            targetPos.x =_targetLane * _settings.lanesXStep;
            _rigidbody.position = Vector3.Lerp(currentPos,targetPos, Time.fixedDeltaTime * 5f);
            if (Mathf.Abs(_rigidbody.position.x - targetPos.x) < 0.03f)
            {
                Debug.Log("It's OK");
                _currentLane = _targetLane;
            }
        }
    }

    void OnHitBorder()
    {

        var tmp = _targetLane;
        _targetLane = _currentLane;
        _currentLane = tmp;
        if (IsStunned)
        {
            _signalBus.Fire<PlayerDiedSignal>();
            return;
        }
        StartCoroutine(StunRoutine());
    }

    IEnumerator StunRoutine()
    {
        IsStunned = true;
        yield return  new WaitForSeconds(_settings.stunSpan);
        _signalBus.Fire<StunExpiredSignal>();
        IsStunned = false;
    }

    void MoveLeft()
    {
        if (_targetLane >= -1)
        {
            _targetLane--;
        }
    }

    void MoveRight()
    {
        if (_targetLane <= 1)
        {
            _targetLane++;
        }
    }

    private bool isProne;
    
    void Prone()
    {
        StartCoroutine(ProneRoutine());
    }

    IEnumerator ProneRoutine()
    {
        float elapsedTime = 0f;
        if (!isProne)
        {
            isProne = true;
            _capsuleCollider.height = 1f;
            _capsuleCollider.center = new Vector3(0,0.5f, 0);
            while (elapsedTime < 0.8f)
            {
                var velocity = _rigidbody.velocity;
                velocity.y = Mathf.Min(velocity.y, 0);
               _rigidbody.velocity = velocity;
               elapsedTime += Time.deltaTime;
               yield return null;
            }
            _capsuleCollider.height = 2f;
            _capsuleCollider.center = new Vector3(0,1f, 0);
            isProne = false;
        }
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
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Border"))
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
        public float lanesXStep;
    }
    
}

public enum PlayerStates
{
   Awaiting,
   Playing,
   Dead
    
}
