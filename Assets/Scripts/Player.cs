using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Zenject;

public class Player : MonoBehaviour
{
    [Inject] private MobileInputSystem _inputSystem;
    [Inject] private BlockManager _blockManager;
    //[Inject]
    private Settings _settings;

    [Inject(Id = "Playing")]
    private CinemachineVirtualCamera _playingCamera;
    [Inject(Id = "Awaiting")]
    private CinemachineVirtualCamera _awaitingCamera;
    private Rigidbody _rigidbody;

    public Vector3 Position => transform.position;
    
    
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _startPos = transform.position;
        ChangeState();
    }

    public float Speed = 5f;

    private bool _isPlaying;
    private Vector3 _startPos;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isPlaying = !_isPlaying;
            _blockManager.IsActive = !_blockManager.IsActive; //TODO: Убрать тестовую логику
            ChangeState();
        }

        if (_isPlaying)
        {
            transform.Translate( Vector3.forward * (Speed * Time.deltaTime));
        }

    }


    private void ChangeState()
    {
        if (_isPlaying)
        {
            _playingCamera.Priority = 1;
            _awaitingCamera.Priority = 0;
            
            
        }
        else
        {
            _blockManager.ClearBlocks(); //TODO: Убрать тестовую логику
            
            _rigidbody.velocity = Vector3.zero;
            transform.position = _startPos;
            
            _playingCamera.Priority = 0;
            _awaitingCamera.Priority = 1;
        }
    }

    [Serializable]
    public class Settings
    {
        public float playerSpeed;
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
