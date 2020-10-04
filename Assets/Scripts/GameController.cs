using System;
using Signals;
using UniRx;
using UnityEngine;
using Zenject;

public class GameController : IInitializable, IDisposable, ITickable
{
    [Inject] private readonly SignalBus _signalBus;
    [Inject] private readonly IGameDataHandler _gameDataHandler;
    [Inject] private readonly UIController _uiController;
    [Inject] private readonly PlayerMovement _playerMovement;
    [Inject] private PlayerMovement.Settings _settings;
    public CurrentSession Session { get; private set; }
    public GameData Data { get; private set; }
    private CompositeDisposable _disposable = new CompositeDisposable();
    
    
    
    private void OnStartedAwaiting()
    {
    }

    private void OnStartedGame()
    {
        Session = new CurrentSession();
        Session.Score.Subscribe(newValue => _uiController.UpdateCurrentScore(newValue)).AddTo(_disposable);
        Session.PickUpCount.Subscribe(newValue => _uiController.UpdateCurrentPickupCount(newValue)).AddTo(_disposable);
    }

    public void Initialize()
    {
        Data = _gameDataHandler.LoadGameData();

        Data.BestScore.Subscribe(newValue => _uiController.UpdateBestScore(newValue));
        Data.TotalPickUpCount.Subscribe(newValue => _uiController.UpdateTotalPickupCount(newValue));

        _signalBus.Subscribe<StartedAwaitingSignal>(OnStartedAwaiting);
        _signalBus.Subscribe<StartedGameSignal>(OnStartedGame);
        _signalBus.Subscribe<PlayerDiedSignal>(OnPlayerDied);
        _signalBus.Subscribe<PickedUpCollectableSignal>(OnPickedUp);
        //Initial state
        _signalBus.Fire<StartedAwaitingSignal>();
    }
    private void OnPickedUp()
    {
        Session.AddPickUp();
    }
    private void OnPlayerDied()
    {
        if (Data.BestScore.Value < Session.Score.Value)
        {
            Data.BestScore.Value = Session.Score.Value;
        }

        Data.TotalPickUpCount.Value += Session.PickUpCount.Value;
    }

    public void Dispose()
    {
        _gameDataHandler.SaveGameData(Data);
        
        _signalBus.Unsubscribe<StartedAwaitingSignal>(OnStartedAwaiting);
        _signalBus.Unsubscribe<StartedGameSignal>(OnStartedGame);
        _signalBus.Unsubscribe<PlayerDiedSignal>(OnPlayerDied);
        _signalBus.Unsubscribe<PickedUpCollectableSignal>(OnPickedUp);
        _disposable.Dispose();
    }

    public void Tick()
    {
        if (_playerMovement.IsPlaying)
        {
            Session.ElapsedTime += Time.deltaTime;
            _playerMovement.currentSpeed = SetSpeedForGivenTime(Session.ElapsedTime);
            Session.Score.Value += _playerMovement.currentSpeed * Time.deltaTime;
        }
    }

    private float SetSpeedForGivenTime(float time)
    {
        return _settings.playerStartSpeed + Mathf.Log10(1f + time);
    }
}

public class CurrentSession
{
    public ReactiveProperty<int> PickUpCount { get; private set; }
    public ReactiveProperty<float> Score { get; private set; }

    public float ElapsedTime;
    public CurrentSession()
    {
        PickUpCount = new ReactiveProperty<int>(0);
        Score = new ReactiveProperty<float>(0f);
    }
    public void AddPickUp()
    {
        PickUpCount.Value++;
    }
}

public class GameData
{
    public ReactiveProperty<int> TotalPickUpCount { get; private set; }
    public ReactiveProperty<float> BestScore { get; private set; }


    
    public GameData(int totalPickUpCount = 0, float bestScore = 0f)
    {
        TotalPickUpCount = new ReactiveProperty<int>(totalPickUpCount);
        BestScore = new ReactiveProperty<float>(bestScore);
    }
}