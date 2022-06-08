using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterAnimationController : MonoBehaviour
{
    [SerializeField] private IAnimatableObjectParent _animatableObjectParent;
    [SerializeField] private WordAnimation _wordAnimation;
    [SerializeField] private LetterFactory _letterFactory;
    [SerializeField] private GameObject _letterToAnimate;
    [SerializeField] private GameObject _target;

    void Start()
    {
        _animatableObjectParent = GetComponent<IAnimatableObjectParent>();
        _animatableObjectParent.OnAnimationInitialize += OnAnimationInitialize;
    }

    private void OnDestroy()
    {
        _animatableObjectParent.OnAnimationInitialize -= OnAnimationInitialize;
    }

    private void OnAnimationInitialize()
    {
        var letter = _letterFactory.Create(_letterToAnimate);
        _wordAnimation.Play(letter, _target, () => _animatableObjectParent.AnimationFinishCallback());
    }
}
