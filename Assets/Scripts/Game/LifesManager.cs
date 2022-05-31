using System;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.SceneManagement;

public class LifesManager : MonoBehaviour
{
    public const string LifesKey = "lifes";
    public const string TimeKey = "time";
    private const string LifesFull = "FULL";
    private const int ExitCost = -1;

    public static Action<int> LifesAmountChanged;
    public static Action<int> TryChangeLifesAmount;
    public static Action<string> LifesAmountChangeImpossible;

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

    private void Start()
    {
        _timeLeft = DataSaver.LoadFloatData(TimeKey);
        var loadedLifes = DataSaver.LoadIntData(LifesKey);
        SetLifes(Mathf.Max(0, loadedLifes));

        _showPopupButton.onClick.AddListener(ShowBuyLifesPopup);
        TryChangeLifesAmount += ChangeLifesAmount;
        BuyLifesPopup.ContinueWhithExtraLifes += SetLifes;
    }

    private void OnDestroy()
    {
        DataSaver.SaveIntData(LifesKey, Lifes);
        DataSaver.SaveFloatData(TimeKey, _timeLeft);

        TryChangeLifesAmount -= ChangeLifesAmount;
        BuyLifesPopup.ContinueWhithExtraLifes -= SetLifes;
    }

#if UNITY_IOS
    private void OnApplicationQuit()
    {
        if (SceneManager.GetActiveScene().name.Equals(Literal.Scene_GameScene))
        {
            TryChangeLifesAmountMethod(ExitCost);
        }
    }
#endif

    private void Update()
    {
        SetTimer();
    }

    private void OnGUI()
    {
        if (_lifesFull == false)
        {
            if (_timeLeft > 0)
            {
                _minutes = Mathf.Floor(_timeLeft / 60);
                _seconds = Mathf.RoundToInt(_timeLeft % 60);

                _timerTMP.text = _minutes.ToString("00") + ":" + _seconds.ToString("00");
            }
            else
            {
                SetLifes(_defaultLifesAmount);
            }
        }
    }

    private void SetTimer()
    { 
        if(!_lifesFull)
            _timeLeft -= Time.deltaTime;
    }

    private void StopTimer()
    {
        _timeLeft = _restorationTime;
        _lifesFull = true;
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

    private void ShowBuyLifesPopup()
    {
        if (_lifesFull)
            return;

        _buyLifesPopup.ShowPopup(true);
    }

    private void UpdateText()
    { 
        _lifesText.text = Lifes.ToString();
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

    public static bool TryChangeLifesAmountMethod(int amount)
    {
        TryChangeLifesAmount.Invoke(amount);
        return _haveLifes;
    }
}
