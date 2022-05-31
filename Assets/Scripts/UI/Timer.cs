using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Timer : MonoBehaviour
{
    public GameData currentGameData;
    public TextMeshProUGUI timerText;
    public float extraTime = 60f;
    
    private float _timeLeft;
    private float _minutes;
    private float _seconds;
    private float _oneSecondDown;
    private bool _timeOut;
    private bool _stopTimers;
    private float _timeToPrompt;
    private float _promptTimer;

    void Start()
    {
        _stopTimers = false;
        _timeOut = false;
        _timeLeft = currentGameData.selectedBoardData.TimeInSeconds;
        _oneSecondDown = _timeLeft - 1f;
        _timeToPrompt = currentGameData.selectedBoardData.TimeToPrompt;
        _promptTimer = _timeToPrompt;

        GameEvents.OnBoardComleted += StopTimer;
        GameEvents.OnUnlockNextCategory += StopTimer;
        GameEvents.OnCorrectWord += ResetPromptTimer;
        GameEvents.OnCorrectExtraWord += ResetPromptTimer;
        GameOverPopup.ContinueWhithExtraTime += RestartTimers;
    }

    private void OnDisable()
    {
        GameEvents.OnBoardComleted -= StopTimer;
        GameEvents.OnUnlockNextCategory -= StopTimer;
        GameEvents.OnCorrectWord -= ResetPromptTimer;
        GameEvents.OnCorrectExtraWord -= ResetPromptTimer;
        GameOverPopup.ContinueWhithExtraTime -= RestartTimers;
    }

    public void StopTimer(bool categoryCompleted) => _stopTimers = true;
    public void StopTimer() => _stopTimers = true;

    public void RestartTimers()
    {
        _stopTimers = false;
        _timeOut = false;
        _timeLeft = extraTime;
        _oneSecondDown = _timeLeft - 1f;
        ResetPromptTimer();
    }

    private void Update()
    {
        SetTimer();
        SetPromptTimer();  
    }

    private void OnGUI()
    {
        if (_timeOut == false)
        {
            if (_timeLeft > 0)
            {
                _minutes = Mathf.Floor(_timeLeft / 60);
                _seconds = Mathf.RoundToInt(_timeLeft % 60);

                timerText.text = _minutes.ToString("00") + ":" + _seconds.ToString("00");
            }
            else
            {
                _stopTimers = true;
                ActivateGameOverGUI();
            }
        }
    }

    private void SetTimer()
    {
        if (_stopTimers == false)
            _timeLeft -= Time.deltaTime;

        if (_timeLeft <= _oneSecondDown)
        {
            _oneSecondDown = _timeLeft - 1f;
        }
    }

    private void SetPromptTimer()
    {
        if (_timeToPrompt != 0 && _stopTimers == false)
            _promptTimer -= Time.deltaTime;

        if (_promptTimer <= 0)
        {
            GameEvents.TimeToPromptMethod();
            _promptTimer = _timeToPrompt;
        }

    }

    private void ActivateGameOverGUI()
    {
        GameEvents.GameOverMethod();
        _timeOut = true;
    }

    private void ResetPromptTimer(string word, List<int> squareIndexes) => ResetPromptTimer();
    private void ResetPromptTimer(List<int> squareIndexes) => ResetPromptTimer();
    private void ResetPromptTimer() => _promptTimer = _timeToPrompt;
}
