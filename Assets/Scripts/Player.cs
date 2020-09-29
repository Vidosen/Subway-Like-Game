using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Zenject;

public class Player : MonoBehaviour
{
    [Inject]
    private MobileInputSystem _inputSystem;
    //[Inject]
    private Settings _settings;

    [Inject(Id = "Playing")]
    private CinemachineVirtualCamera _playingCamera;
    [Inject(Id = "Awaiting")]
    private CinemachineVirtualCamera _awaitingCamera;

    private Rigidbody _rigidbody;
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _startPos = transform.position;
        ChangeState();
    }


    private bool _isPlaying;
    private Vector3 _startPos;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isPlaying = !_isPlaying;
            ChangeState();
        }

        if (_isPlaying)
        {
            transform.Translate( Vector3.forward * (5f * Time.deltaTime));
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
