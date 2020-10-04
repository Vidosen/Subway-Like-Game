using System;
using Signals;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

public class UIController : MonoBehaviour
{
    [Inject] readonly SignalBus _signalBus;
    [SerializeField] private RectTransform menuCanvas;
    [SerializeField] private RectTransform retryCanvas;
    [SerializeField] private RectTransform gameCanvas;

    [SerializeField] private Button startButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button backInMenuButton;

    [SerializeField] private TextMeshProUGUI currentPickupCountText;
    [SerializeField] private TextMeshProUGUI totalPickupCountText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;

    public void UpdateCurrentPickupCount(int newPickupCount)
    {
        currentPickupCountText.text = newPickupCount.ToString();
    }
    
    public void UpdateTotalPickupCount(int newTotalPickupCount)
    {
        totalPickupCountText.text = newTotalPickupCount.ToString();
    }
    public void UpdateCurrentScore(float newScore)
    {
        scoreText.text = Mathf.RoundToInt(newScore).ToString();
    }
    
    public void UpdateBestScore(float newBestScore)
    {
        bestScoreText.text = Mathf.RoundToInt(newBestScore).ToString();
    }

    private void Start()
    {
        startButton.OnClickAsObservable().Subscribe((_ => _signalBus.Fire<StartedGameSignal>()));
        retryButton.OnPointerClickAsObservable().Subscribe((_ => _signalBus.Fire<StartedGameSignal>()));
        backInMenuButton.OnPointerClickAsObservable().Subscribe((_ => _signalBus.Fire<StartedAwaitingSignal>()));

    }

    private void OnEnable()
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


    private void OnPlayerDied()
    {
        menuCanvas.gameObject.SetActive(false);
        retryCanvas.gameObject.SetActive(true);
        gameCanvas.gameObject.SetActive(false);
    }

    private void OnStartedAwaiting()
    {
        menuCanvas.gameObject.SetActive(true);
        retryCanvas.gameObject.SetActive(false);
        gameCanvas.gameObject.SetActive(false);
    }

    private void OnStartedGame()
    {
        menuCanvas.gameObject.SetActive(false);
        retryCanvas.gameObject.SetActive(false);
        gameCanvas.gameObject.SetActive(true);
    }
}