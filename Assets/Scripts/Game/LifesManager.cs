using System;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;

public class LifesManager : MonoBehaviour
{
    public const string LifesKey = "lifes";

    public static Action<int> LifesAmountChanged;
    public static Action<int> TryChangeLifesAmount;
    public static Action<string> LifesAmountChangeImpossible;

    [SerializeField] private BuyLifesPopup _buyLifesPopup;
    [SerializeField] private Text _lifesText;
    [SerializeField] private TextMeshProUGUI _timerTMP;
    [SerializeField] private int _defaultLifesAmount;
    [SerializeField] private int _lifesCost;
    private static bool _haveLifes;

    public int Lifes { get; private set; }

    private void Start()
    {
        var loadedLifes = DataSaver.LoadIntData(LifesKey);
        SetLifes(Mathf.Max(0, loadedLifes));

        TryChangeLifesAmount += ChangeLifesAmount;
        BuyLifesPopup.ContinueWhithExtraLifes += SetLifes;
    }

    private void OnDestroy()
    {
        DataSaver.SaveIntData(LifesKey, Lifes);

        TryChangeLifesAmount -= ChangeLifesAmount;
        BuyLifesPopup.ContinueWhithExtraLifes -= SetLifes;
    }

    private void SetLifes(int lifes)
    { 
        Lifes = lifes;
        UpdateText();
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
