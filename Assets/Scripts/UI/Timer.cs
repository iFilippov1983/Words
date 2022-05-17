using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public GameData currentGameData;
    public TextMeshProUGUI timerText;

    private float _timeLeft;
    private float _minutes;
    private float _seconds;
    private float _oneSecondDown;
    private bool _timeOut;
    private bool _stopTimer;

    void Start()
    {
        _stopTimer = false;
        _timeOut = false;
        _timeLeft = currentGameData.selectedBoardData.TimeInSeconds;
        _oneSecondDown = _timeLeft - 1f;

        GameEvents.OnBoardComleted += StopTimer;
        GameEvents.OnUnlockNextCategory += StopTimer;
    }

    private void OnDisable()
    {
        GameEvents.OnBoardComleted -= StopTimer;
        GameEvents.OnUnlockNextCategory -= StopTimer;
    }

    public void StopTimer(bool categoryCompleted) => _stopTimer = true;
    public void StopTimer() => _stopTimer = true;

    private void Update()
    {
        if(_stopTimer == false)
            _timeLeft -= Time.deltaTime;

        if (_timeLeft <= _oneSecondDown)
        {
            _oneSecondDown = _timeLeft - 1f;
        }
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
                _stopTimer = true;
                ActivateGameOverGUI();
            }
        }
    }

    private void ActivateGameOverGUI()
    {
        GameEvents.GameOverMethod();
        _timeOut = true;
    }
}