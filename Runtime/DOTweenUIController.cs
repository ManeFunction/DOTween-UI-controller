using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Mane.Unity.DOTweenUIC
{
    /// <summary>
    /// Inspector-driven UI tweens (move, scale, rotate Z, CanvasGroup fade, graphic color)
    /// backed by DOTween. Lifecycle flags on this component (autoplay, pause on disable,
    /// finish on stop) still apply when playback is controlled from code.
    /// </summary>
    [AddComponentMenu("Mane Tools/UI/DOTween UI Controller")]
    public class DOTweenUIController : UIBehaviour
    {
        [SerializeField] private bool _autoplay = true;
        [SerializeField] private bool _createOnStart = true;
        [SerializeField] private bool _restartOnEnable = true;
        [SerializeField] private bool _pauseOnDisable = true;
        [SerializeField] private bool _finishOnStop = true;
        [SerializeField] private bool _unscaledTime;
        
        [SerializeField] private TweenDataFloat _moveX;
        [SerializeField] private TweenDataFloat _moveY;

        [SerializeField] private TweenDataFloat _scaleX;
        [SerializeField] private TweenDataFloat _scaleY;

        [SerializeField] private TweenDataFloat _rotate;

        [SerializeField] private TweenDataFloat _fade;
        [SerializeField] private TweenDataColor _color;

        private readonly List<Tween> _activeTweens = new(7);
        private readonly List<Tween> _pausedOnDisable = new(7);

        private bool _shouldRestartOnEnable;

        private bool RestartOnEnable => _autoplay && _restartOnEnable;

        // Only one-shot autoplay tweens are killed when they finish. Code-driven and restartable
        // tweens stay alive, so Play, PlayBackwards and Restart keep working on them.
        private bool AutoKill => _autoplay && !_restartOnEnable;

        protected override void Start()
        {
            base.Start();

            // Play() may have created the tweens before Start, e.g. right after Instantiate.
            if (_autoplay || _createOnStart)
                TryCreateTweens();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_shouldRestartOnEnable)
            {
                _pausedOnDisable.Clear();
                Restart();
            }
            else if (_pauseOnDisable)
            {
                // Resume only what OnDisable paused, not tweens that were never played or paused on purpose.
                foreach (Tween tween in _pausedOnDisable)
                    if (tween.IsActive())
                        tween.Play();

                _pausedOnDisable.Clear();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _shouldRestartOnEnable = RestartOnEnable;

            if (_pauseOnDisable)
            {
                _pausedOnDisable.Clear();
                foreach (Tween tween in _activeTweens)
                    if (tween.IsActive() && tween.IsPlaying())
                        _pausedOnDisable.Add(tween);

                Pause();
            }
        }

        protected override void OnDestroy()
        {
            StopAndDispose();

            base.OnDestroy();
        }

        /// <summary>
        /// Enables or disables one Inspector tween slot. Does not rebuild tweens that are
        /// already created; dispose them first if a running animation should pick up the change.
        /// </summary>
        /// <param name="tweenType">Slot to toggle.</param>
        /// <param name="isEnabled">Whether that slot should create a tween on the next create.</param>
        public void SetTweenEnabled(TweenType tweenType, bool isEnabled) => GetSlot(tweenType).IsEnable = isEnabled;

        /// <summary>
        /// Component that a tween slot animates. The slot needs it on the same GameObject.
        /// </summary>
        public static Type GetRequiredComponent(TweenType tweenType) =>
            tweenType switch
            {
                TweenType.Fade => typeof(CanvasGroup),
                TweenType.Color => typeof(MaskableGraphic),
                _ => typeof(RectTransform)
            };

        private ITweenSlot GetSlot(TweenType tweenType) =>
            tweenType switch
            {
                TweenType.MoveX => _moveX,
                TweenType.MoveY => _moveY,
                TweenType.ScaleX => _scaleX,
                TweenType.ScaleY => _scaleY,
                TweenType.Rotate => _rotate,
                TweenType.Fade => _fade,
                TweenType.Color => _color,
                _ => throw new ArgumentOutOfRangeException(nameof(tweenType), tweenType, null)
            };

        // Each slot only says how to tween its value to a target. TweenData turns that into a From tween when needed.
        private void CreateTweeners(float additionalDelay = 0f)
        {
            RectTransform rect = transform as RectTransform;
            Add(TweenType.MoveX, _moveX, (value, duration) => rect.DOAnchorPosX(value, duration));
            Add(TweenType.MoveY, _moveY, (value, duration) => rect.DOAnchorPosY(value, duration));
            Add(TweenType.ScaleX, _scaleX, (value, duration) => rect.DOScaleX(value, duration));
            Add(TweenType.ScaleY, _scaleY, (value, duration) => rect.DOScaleY(value, duration));
            Add(TweenType.Rotate, _rotate, (value, duration) =>
            {
                Vector3 euler = rect.localEulerAngles;
                return rect.DOLocalRotate(new Vector3(euler.x, euler.y, value), duration, RotateMode.FastBeyond360);
            });
            Add(TweenType.Fade, _fade, (value, duration) => GetComponent<CanvasGroup>().DOFade(value, duration));
            Add(TweenType.Color, _color, (value, duration) => GetComponent<MaskableGraphic>().DOColor(value, duration));

            void Add<T>(TweenType tweenType, TweenData<T> data, Func<T, float, Tweener> tweenTo)
            {
                if (!data.IsEnable)
                    return;

                Type required = GetRequiredComponent(tweenType);
                if (GetComponent(required) == null)
                {
                    Debug.LogWarning($"{tweenType} tween needs a {required.Name} on '{name}'. It is skipped.", this);
                    return;
                }

                _activeTweens.Add(data.CreateTween(tweenTo, _unscaledTime, _autoplay, AutoKill, additionalDelay));
            }
        }

        /// <summary>
        /// Creates tweens from the Inspector setup if none exist, then plays them forward.
        /// </summary>
        /// <param name="delay">
        /// Extra delay added to each tween's own delay. Used only when tweens are created
        /// on this call; ignored if they already exist.
        /// </param>
        public void Play(float delay = 0f)
        {
            TryCreateTweens(delay);
            PlayTweens();
        }

        /// <summary>
        /// Creates tweens from the Inspector setup if none exist, then plays them backward.
        /// </summary>
        /// <param name="delay">
        /// Extra delay added to each tween's own delay. Used only when tweens are created
        /// on this call; ignored if they already exist.
        /// </param>
        public void PlayBackwards(float delay = 0f)
        {
            TryCreateTweens(delay);
            PlayTweensBackwards();
        }

        /// <summary>
        /// Pauses all active tweens. They stay allocated and can be resumed with
        /// <see cref="Play(float)"/> or <see cref="PlayBackwards(float)"/>.
        /// </summary>
        public void Pause()
        {
            foreach (Tween tween in _activeTweens)
                tween.Pause();
        }

        /// <summary>
        /// Stops all active tweens without disposing them. Completes each tween when
        /// Finish On Stop is enabled; otherwise pauses. Tweens stay in the active list.
        /// </summary>
        public void Stop()
        {
            foreach (Tween tween in _activeTweens)
            {
                if (_finishOnStop)
                    tween.Complete();
                else
                    tween.Pause();
            }
        }

        /// <summary>
        /// Kills all active tweens and clears the list. Completes them first when
        /// Finish On Stop is enabled.
        /// </summary>
        public void StopAndDispose()
        {
            foreach (Tween tween in _activeTweens)
                tween.Kill(_finishOnStop);
            
            _activeTweens.Clear();
        }

        /// <summary>
        /// Rewinds all active tweens to their start, kills them, and clears the list.
        /// Ignores Finish On Stop.
        /// </summary>
        public void UndoAndDispose()
        {
            foreach (Tween tween in _activeTweens)
            {
                tween.Rewind();
                tween.Kill();
            }

            _activeTweens.Clear();
        }

        /// <summary>
        /// Restarts all active tweens from the beginning. If none exist yet, this is
        /// equivalent to <see cref="Play(float)"/>.
        /// </summary>
        public void Restart()
        {
            RemoveKilledTweens();
            if (_activeTweens.Count == 0)
                Play();
            else
                foreach (Tween tween in _activeTweens)
                    tween.Restart();
        }

        /// <summary>
        /// Completes all active tweens, then plays them backward. If none exist yet, this
        /// is equivalent to <see cref="PlayBackwards(float)"/>. One-shot autoplay tweens
        /// (Autoplay on, Restart On Enable off) are killed on completion, so they only complete.
        /// </summary>
        public void RestartBackwards()
        {
            RemoveKilledTweens();
            if (_activeTweens.Count == 0)
                PlayBackwards();
            else
                foreach (Tween tween in _activeTweens)
                {
                    tween.Complete();
                    if (tween.IsActive())
                        tween.PlayBackwards();
                }
        }

        /// <summary>
        /// Rewinds all active tweens to their start without killing them.
        /// </summary>
        public void Rewind()
        {
            foreach (Tween tween in _activeTweens)
                tween.Rewind();
        }

        private void TryCreateTweens(float delay = 0f)
        {
            RemoveKilledTweens();
            if (_activeTweens.Count == 0)
                CreateTweeners(delay);
        }

        // Auto-killed or externally killed tweens are dead handles; drop them so the next call rebuilds.
        private void RemoveKilledTweens() => _activeTweens.RemoveAll(tween => !tween.IsActive());
        
        private void PlayTweens()
        {
            foreach (Tween tween in _activeTweens)
                tween.Play();
        }

        private void PlayTweensBackwards()
        {
            foreach (Tween tween in _activeTweens)
                tween.PlayBackwards();
        }
        
        /// <summary>Inspector tween slot toggled by <see cref="SetTweenEnabled"/>.</summary>
        public enum TweenType
        {
            /// <summary>Anchored position X.</summary>
            MoveX,
            /// <summary>Anchored position Y.</summary>
            MoveY,
            /// <summary>Local scale X.</summary>
            ScaleX,
            /// <summary>Local scale Y.</summary>
            ScaleY,
            /// <summary>Local Euler Z.</summary>
            Rotate,
            /// <summary>CanvasGroup alpha.</summary>
            Fade,
            /// <summary>MaskableGraphic color.</summary>
            Color,
        }
    }
}
