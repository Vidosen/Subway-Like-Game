using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Signals;
using UnityEngine;
using Zenject;

public class CameraController : IInitializable, IDisposable
{
    [Inject] private readonly SignalBus _signalBus;
    [Inject(Id = "Playing")]
    private CinemachineVirtualCamera _playingCamera;
    [Inject(Id = "Awaiting")]
    private CinemachineVirtualCamera _awaitingCamera;

    private void OnPlayerStarted()
    {
        _playingCamera.Priority = 1;
        _awaitingCamera.Priority = 0;
    }

    private void OnPlayerAwaiting()
    {
        _playingCamera.Priority = 0;
        _awaitingCamera.Priority = 1;
    }

    public void Initialize()
    {
        _signalBus.Subscribe<StartedAwaitingSignal>(OnPlayerAwaiting);
        _signalBus.Subscribe<StartedGameSignal>(OnPlayerStarted);
    }
    public void Dispose()
    {
        _signalBus.Unsubscribe<StartedAwaitingSignal>(OnPlayerAwaiting);
        _signalBus.Unsubscribe<StartedGameSignal>(OnPlayerStarted);
    }
}
