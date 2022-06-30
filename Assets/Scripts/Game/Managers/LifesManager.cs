using System;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.SceneManagement;

public class LifesManager : MonoBehaviour
{
    public const string LifesKey = "Lifes";
    public const string TimeKey = "Time";
    public const string DateTimeKey = "LastSavedTime";
    private const string LifesFull = "FULL";
    private const int ExitCost = 1;

    [SerializeField] private BuyLifesPopup _buyLifesPopup;
    [SerializeField] private Button _showPopupButton;
    [SerializeField] private Text _lifesText;
    [SerializeField] private TextMeshProUGUI _timerTMP;
    [SerializeField] private int _defaultLifesAmount = 5;
    [SerializeField] private float _restorationTime = 900;

    private static bool _haveLifes;
    private bool _lifesFull;

    private float _timeLeft;
    private float _minutes;
    private float _seconds;

    public int Lifes { get; private set; }

    public static Action<int> LifesAmountChanged;
    public static Action<int> TryChangeLifesAmount;
    public static Action<string> LifesAmountChangeImpossible;

    private void Start()
    {
        LoadData();

        _showPopupButton.onClick.AddListener(ShowBuyLifesPopup);

        TryChangeLifesAmount += ChangeLifesAmount;
        BuyLifesPopup.ContinueWhithExtraLifes += ChangeLifesAmount;
    }

    private void OnDestroy()
    {
        SaveData();

        TryChangeLifesAmount -= ChangeLifesAmount;
        BuyLifesPopup.ContinueWhithExtraLifes -= ChangeLifesAmount;
    }

    private void OnApplicationQuit()
    {
#if UNITY_IOS || UNITY_IPHONE
        if (SceneManager.GetActiveScene().name.Equals(Literal.Scene_GameScene))
        {
            int amount = -ExitCost;
            TryChangeLifesAmountMethod(amount);
        }
#endif
    }


    private void FixedUpdate()
    {
        UpdateTimer();
    }

    private void OnGUI()
    {
        if (_lifesFull == false)
        {
            if (_timeLeft > 0)
            {
                _minutes = Mathf.Floor(_timeLeft / 60);
                _seconds = Mathf.RoundToInt(_timeLeft % 60);
                if(_seconds == 60) _seconds = 59;

                _timerTMP.text = _minutes.ToString("00") + ":" + _seconds.ToString("00");
                if(_buyLifesPopup != null)
                    _buyLifesPopup.TimerText.text = _minutes.ToString("00") + ":" + _seconds.ToString("00");
            }
            else
            {
                SetLifes(_defaultLifesAmount);
            }
        }
    }

    private void UpdateTimer()
    { 
        if(!_lifesFull)
            _timeLeft -= Time.deltaTime;
    }

    private void SetLifes(int lifes)
    { 
        Lifes = lifes;
        UpdateText();
        CheckLifes();
    }

    private void CheckLifes()
    {
        _lifesFull = Lifes >= _defaultLifesAmount
            ? true : false;

        if (_lifesFull)
        {
            _timerTMP.text = LifesFull;
            StopTimer();
        }
        else
        {
            _lifesFull = false;
        }
    }

    private void StopTimer()
    {
        _timeLeft = _restorationTime;
        _lifesFull = true;
    }

    private void ShowBuyLifesPopup()
    {
        if (_lifesFull)
            return;
        _buyLifesPopup.ShowPopup(true);
    }

    private void UpdateText()
    { 
        _lifesText.text = Lifes.ToString();
        if (_buyLifesPopup != null)
            _buyLifesPopup.LifesText.text = Lifes.ToString();
    }

    [Button]
    [HideInEditorMode]
    private void ChangeLifesAmount(int amount)
    { 
        var newAmount = Lifes + amount;
        if (newAmount < 0)
        {
            _haveLifes = false;
            LifesAmountChangeImpossible?.Invoke(Literal.AnimName_NoLifes);
        }
        else
        {
            _haveLifes = true;
            SetLifes(newAmount);
            LifesAmountChanged?.Invoke(amount);
        }
    }

    private void SetStartLifes()
    {
        var startLifes = DataSaver.HasKey(LifesKey)
            ? DataSaver.LoadIntData(LifesKey)
            : _defaultLifesAmount;
        SetLifes(Mathf.Max(0, startLifes));
    }

    private void SetTime()
    {
        _timeLeft = DataSaver.LoadFloatData(TimeKey);

        DateTime lastSavedTime = DataSaver.LoadDateTime(DateTimeKey, DateTime.UtcNow);
        TimeSpan timePassed = DateTime.UtcNow - lastSavedTime;
        float secondsPassed = (float)timePassed.TotalSeconds;
        float secondsInWeek = 7f * 24f * 60f * 60f;
        secondsPassed = Mathf.Clamp(secondsPassed, 0f, secondsInWeek);

        if (secondsPassed >= _timeLeft)
        {
            SetLifes(_defaultLifesAmount);
        }
        else
        {
            _timeLeft -= secondsPassed;
        }
    }

    private void LoadData()
    {
        SetTime();
        SetStartLifes();
    }

    private void SaveData()
    {
        DataSaver.SaveIntData(LifesKey, Lifes);
        DataSaver.SaveFloatData(TimeKey, _timeLeft);
        DataSaver.SaveDateTime(DateTimeKey, DateTime.UtcNow);
    }

    public static bool TryChangeLifesAmountMethod(int amount)
    {
        TryChangeLifesAmount?.Invoke(amount);
        return _haveLifes;
    }
}
