using System;
using DG.Tweening;
using UnityEngine;

namespace Mane.Unity.DOTweenUIC
{
    [Serializable]
    internal class TweenDataFloat : TweenData<float> { }
    
    [Serializable]
    internal class TweenDataColor : TweenData<Color> { }

    // A tween slot of DOTweenUIController, whatever its value type.
    internal interface ITweenSlot
    {
        bool IsEnable { get; set; }
    }

    [Serializable]
    internal abstract class TweenData<T> : ITweenSlot
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

        // tweenTo tweens from the current value to the given one. From() reverses it: the target jumps to
        // the value now and tweens back to where it was.
        public Tween CreateTween(Func<T, float, Tweener> tweenTo,
            bool unscaledTime, bool autoplay, bool autoKill, float additionalDelay)
        {
            Tweener tweener = tweenTo(_value, _duration);
            if (_direction == Direction.From)
                tweener.From();

            _easeData.Apply(tweener);
            Tween tween = tweener;

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
