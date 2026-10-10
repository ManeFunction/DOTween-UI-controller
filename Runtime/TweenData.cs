using System;
using DG.Tweening;
using UnityEngine;

namespace Mane.Unity.DOTween
{
    [Serializable]
    internal class TweenDataFloat : TweenData<float> { }
    
    [Serializable]
    internal class TweenDataColor : TweenData<Color> { }

    [Serializable]
    internal abstract class TweenData<T>
    {
        [SerializeField] private bool _isEnable;
        [SerializeField] private float _delay;
        [SerializeField] private Direction _direction;
        [SerializeField] private T _value;
        [SerializeField] private float _duration = .5f;
        [SerializeField] private int _loopCount;
        [SerializeField] private float _delayBetweenLoops;
        [SerializeField] private LoopType _loopType = LoopType.Restart;
        [SerializeField] private EaseData _easeData;
        
        public bool IsEnable
        {
            get => _isEnable;
            set => _isEnable = value;
        }

        public Tween TryCreateTween(Func<Direction, T, float, Tween> createTweener,
            bool unscaledTime, bool autoplay, bool autoKill, float additionalDelay)
        {
            if (!_isEnable)
                return null;

            Tween tween = createTweener(_direction, _value, _duration);
            _easeData.Apply(tween);

            float delay = _delay + additionalDelay;
            if (_loopCount != 0)
            {
                if (_delayBetweenLoops > 0f)
                {
                    tween = DG.Tweening.DOTween.Sequence().Append(tween).AppendInterval(_delayBetweenLoops)
                        .SetDelay(delay, false)
                        .SetLoops(_loopCount, _loopType);
                }
                else
                {
                    tween.SetDelay(delay).SetLoops(_loopCount, _loopType);
                }
            }
            else
                tween.SetDelay(delay);

            // Applied to the outermost tween: DOTween ignores these settings on a tween nested in a sequence.
            tween.SetUpdate(unscaledTime);
            tween.SetAutoKill(autoKill && _loopCount == 0);
            if (!autoplay)
                tween.Pause();

            return tween;
        }
    }

    internal enum Direction
    {
        From,
        To,
    }
}
