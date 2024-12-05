using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ManeFunction.DOTweenExtensions
{
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

        private readonly List<Tween> _activeTweens = new List<Tween>(7);

        private Lazy<RectTransform> _rectTransform;
        private Lazy<CanvasGroup> _canvasGroup;
        private Lazy<MaskableGraphic> _graphics;
        
        private bool _shouldRestartOnEnable;
        
        private bool RestartOnEnable => _autoplay && _restartOnEnable;

        protected override void Awake()
        {
            base.Awake();

            _rectTransform = new Lazy<RectTransform>(() => transform as RectTransform);
            _canvasGroup = new Lazy<CanvasGroup>(() => gameObject.GetComponent<CanvasGroup>());
            _graphics = new Lazy<MaskableGraphic>(() => gameObject.GetComponent<MaskableGraphic>());
        }

        protected override void Start()
        {
            base.Start();

            if (_autoplay || _createOnStart)
                CreateTweeners();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            if (_shouldRestartOnEnable)
                Restart();
            else if (_pauseOnDisable)
                PlayTweens();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            _shouldRestartOnEnable = RestartOnEnable;
            
            if (_pauseOnDisable)
                Pause();
        }

        protected override void OnDestroy()
        {
            Stop();

            base.OnDestroy();
        }

        public void SetTweenEnabled(TweenType tweenType, bool isEnabled)
        {
            switch (tweenType)
            {
                case TweenType.MoveX:
                    _moveX.IsEnable = isEnabled;
                    break;
                case TweenType.MoveY:
                    _moveY.IsEnable = isEnabled;
                    break;
                case TweenType.ScaleX:
                    _scaleX.IsEnable = isEnabled;
                    break;
                case TweenType.ScaleY:
                    _scaleY.IsEnable = isEnabled;
                    break;
                case TweenType.Rotate:
                    _rotate.IsEnable = isEnabled;
                    break;
                case TweenType.Fade:
                    _fade.IsEnable = isEnabled;
                    break;
                case TweenType.Color:
                    _color.IsEnable = isEnabled;
                    break;
            }
        }

        private void CreateTweeners(float additionalDelay = 0f)
        {
            Tween moveXTween = _moveX.TryCreateTween((direction, value, duration) =>
            {
                Tweener tweener;
                if (direction == Direction.From)
                {
                    float anchoredPositionX = _rectTransform.Value.anchoredPosition.x;
                    _rectTransform.Value.anchoredPosition = new Vector2(
                        value,
                        _rectTransform.Value.anchoredPosition.y);
                    tweener = _rectTransform.Value.DOAnchorPosX(anchoredPositionX, duration);
                }
                else
                {
                    tweener = _rectTransform.Value.DOAnchorPosX(value, duration);
                }

                return tweener;
            }, _unscaledTime, _autoplay, RestartOnEnable, additionalDelay);
            if (moveXTween != null) _activeTweens.Add(moveXTween);

            Tween moveYTween = _moveY.TryCreateTween((direction, value, duration) =>
            {
                Tweener tweener;
                if (direction == Direction.From)
                {
                    float anchoredPositionY = _rectTransform.Value.anchoredPosition.y;
                    _rectTransform.Value.anchoredPosition = new Vector2(
                        _rectTransform.Value.anchoredPosition.x,
                        value);
                    tweener = _rectTransform.Value.DOAnchorPosY(anchoredPositionY, duration);
                }
                else
                {
                    tweener = _rectTransform.Value.DOAnchorPosY(value, duration);
                }

                return tweener;
            }, _unscaledTime, _autoplay, RestartOnEnable, additionalDelay);
            if (moveYTween != null) _activeTweens.Add(moveYTween);

            Tween scaleXTween = _scaleX.TryCreateTween((direction, value, duration) =>
            {
                Tweener tweener;
                if (direction == Direction.From)
                {
                    float localScaleX = _rectTransform.Value.localScale.x;
                    _rectTransform.Value.localScale = new Vector3(
                        value,
                        _rectTransform.Value.localScale.y,
                        _rectTransform.Value.localScale.z);
                    tweener = _rectTransform.Value.DOScaleX(localScaleX, duration);
                }
                else
                {
                    tweener = _rectTransform.Value.DOScaleX(value, duration);
                }

                return tweener;
            }, _unscaledTime, _autoplay, RestartOnEnable, additionalDelay);
            if (scaleXTween != null) _activeTweens.Add(scaleXTween);

            Tween scaleYTween = _scaleY.TryCreateTween((direction, value, duration) =>
            {
                Tweener tweener;
                if (direction == Direction.From)
                {
                    float localScaleY = _rectTransform.Value.localScale.y;
                    _rectTransform.Value.localScale = new Vector3(
                        _rectTransform.Value.localScale.x,
                        value,
                        _rectTransform.Value.localScale.z);
                    tweener = _rectTransform.Value.DOScaleY(localScaleY, duration);
                }
                else
                {
                    tweener = _rectTransform.Value.DOScaleY(value, duration);
                }

                return tweener;
            }, _unscaledTime, _autoplay, RestartOnEnable, additionalDelay);
            if (scaleYTween != null) _activeTweens.Add(scaleYTween);

            Tween rotateTween = _rotate.TryCreateTween((direction, value, duration) =>
            {
                Tweener tweener;
                if (direction == Direction.From)
                {
                    Vector3 rotationEulerAngles = _rectTransform.Value.rotation.eulerAngles;
                    _rectTransform.Value.rotation = Quaternion.Euler(
                        rotationEulerAngles.x,
                        rotationEulerAngles.y,
                        value);
                    tweener = _rectTransform.Value.DORotate(rotationEulerAngles, duration, RotateMode.FastBeyond360);
                }
                else
                {
                    Vector3 rotationEulerAngles = _rectTransform.Value.rotation.eulerAngles;
                    tweener = _rectTransform.Value.DORotate(new Vector3(rotationEulerAngles.x, rotationEulerAngles.y, value),
                        duration, RotateMode.FastBeyond360);
                }

                return tweener;
            }, _unscaledTime, _autoplay, RestartOnEnable, additionalDelay);
            if (rotateTween != null) _activeTweens.Add(rotateTween);
            
            Tween fadeTween = _fade.TryCreateTween((direction, value, duration) =>
            {
                Tweener tweener;
                if (direction == Direction.From)
                {
                    float alpha = _canvasGroup.Value.alpha;
                    _canvasGroup.Value.alpha = value;
                    tweener = _canvasGroup.Value.DOFade(alpha, duration);
                }
                else
                {
                    tweener = _canvasGroup.Value.DOFade(value, duration);
                }

                return tweener;
            }, _unscaledTime, _autoplay, RestartOnEnable, additionalDelay);
            if (fadeTween != null) _activeTweens.Add(fadeTween);
            
            Tween colorTween = _color.TryCreateTween((direction, value, duration) =>
            {
                Tweener tweener;
                if (direction == Direction.From)
                {
                    Color color = _graphics.Value.color;
                    _graphics.Value.color = value;
                    tweener = _graphics.Value.DOColor(color, duration);
                }
                else
                {
                    tweener = _graphics.Value.DOColor(value, duration);
                }

                return tweener;
            }, _unscaledTime, _autoplay, RestartOnEnable, additionalDelay);
            if (colorTween != null) _activeTweens.Add(colorTween);
        }
        
        public void Play(float delay = 0f)
        {
            if (_activeTweens.Count == 0)
                CreateTweeners(delay);
            
            PlayTweens();
        }

        public void Pause()
        {
            foreach (Tween tween in _activeTweens)
                tween.Pause();
        }

        public void Stop()
        {
            foreach (Tween tween in _activeTweens)
                tween.Kill(_finishOnStop);
            
            _activeTweens.Clear();
        }

        public void Restart()
        {
            if (_activeTweens.Count == 0)
                Play();
            else
                foreach (Tween tween in _activeTweens)
                    tween.Restart();
        }
        
        private void PlayTweens()
        {
            foreach (Tween tween in _activeTweens)
                tween.Play();
        }
        
        public enum TweenType
        {
            MoveX,
            MoveY,
            ScaleX,
            ScaleY,
            Rotate,
            Fade,
            Color,
        }
    }
}
