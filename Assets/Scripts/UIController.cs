using System;
using Signals;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

public class UIController : MonoBehaviour
{
    [Inject]
    readonly SignalBus _signalBus;
    [SerializeField] private RectTransform menuCanvas;
    [SerializeField] private RectTransform retryCanvas;
    [SerializeField] private RectTransform gameCanvas;

    [SerializeField] private Button startButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button backInMenuButton;

    private void Start()
    {
        startButton.OnClickAsObservable().Subscribe((_ => _signalBus.Fire<StartedGameSignal>()));
        retryButton.OnPointerClickAsObservable().Subscribe((_ => _signalBus.Fire<StartedGameSignal>()));
        backInMenuButton.OnPointerClickAsObservable().Subscribe((_ => _signalBus.Fire<StartedAwaitingSignal>()));

    }

    public void OnEnable()
    {
        _signalBus.Subscribe<StartedAwaitingSignal>(OnStartedAwaiting);
        _signalBus.Subscribe<PlayerDiedSignal>(OnPlayerDied);
        _signalBus.Subscribe<StartedGameSignal>(OnStartedGame);
    }

    private void OnDisable()
    {
        _signalBus.Unsubscribe<StartedAwaitingSignal>(OnStartedAwaiting);
        _signalBus.Unsubscribe<PlayerDiedSignal>(OnPlayerDied);
        _signalBus.Unsubscribe<StartedGameSignal>(OnStartedGame);
    }


    void OnPlayerDied()
    {
        menuCanvas.gameObject.SetActive(false);
        retryCanvas.gameObject.SetActive(true);
        gameCanvas.gameObject.SetActive(false);
    }

    void OnStartedAwaiting()
    {
        menuCanvas.gameObject.SetActive(true);
        retryCanvas.gameObject.SetActive(false);
        gameCanvas.gameObject.SetActive(false);
    }

    void OnStartedGame()
    {
        menuCanvas.gameObject.SetActive(false);
        retryCanvas.gameObject.SetActive(false);
        gameCanvas.gameObject.SetActive(true);
    }
}