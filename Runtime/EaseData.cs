using System;
using DG.Tweening;
using UnityEngine;

namespace ManeFunction.DOTweenExtensions
{
    [Serializable]
    internal class EaseData
    {
        [SerializeField] private bool _useCurve = true;
        [SerializeField] private AnimationCurve _curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField] private Ease _ease = Ease.Linear;

        public void Apply(Tween tween)
        {
            if (_useCurve)
                tween.SetEase(_curve);
            else
                tween.SetEase(_ease);
        }
    }
}
