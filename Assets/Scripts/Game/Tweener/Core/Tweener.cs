using System;
using System.Collections;
using System.Diagnostics;
using Game.Threading;
using Game.Tweener.TweenData;
using Game.Util;
using UnityEngine;

namespace Game.Tweener.Core
{
    /*todo
     Play(), Stop(), Restart(), Pause()
     */
    public class Tweener<T, TTweenData> : Utility.IPoolingData
        where TTweenData : ITweenData<T>
    {
        private int _threadKey;
        private Stopwatch _stopwatch;
        private TweenerSetter<T> _setter;
        private TweenerGetter<T> _getter;
        private TTweenData _tweenFunc;

        private Utility.Curves.Ease _ease;
        private bool _from;
        private bool _loop;
        private LoopType _loopType;
        private float _duration;

        private T _startValue;
        private T _endValue;
        private float _durationValue;

        public bool IsPlaying { get; private set; }
        public bool IsPausing { get; private set; }
        public bool IsComplete { get; private set; }
        public TweenerCallback OnUpdate;
        public TweenerCallback OnComplete;

        public static Tweener<T, TTweenData> To(TweenerSetter<T> setter, TweenerGetter<T> getter,
            TTweenData tweenData, T endValue, float duration)
        {
            var tweener = TweenerManager<T, TTweenData>.GetTweener();
            tweener.Initialize(setter, getter, tweenData, endValue, duration);
            return tweener;
        }

        private void Initialize(TweenerSetter<T> setter, TweenerGetter<T> getter, TTweenData tweenData, T endValue,
            float duration)
        {
            _threadKey = -999;
            _stopwatch = Stopwatch.StartNew();
            
            IsPlaying = false;
            IsPausing = false;
            IsComplete = true;
            _ease = Utility.Curves.Ease.InSine;
            _from = false;

            _setter = setter;
            _getter = getter;
            _tweenFunc = tweenData;
            _startValue = getter();
            _endValue = endValue;
            _duration = duration;
            _durationValue = 0f;
        }

        public Tweener<T, TTweenData> SetEase(Utility.Curves.Ease ease)
        {
            _ease = ease;
            return this;
        }

        public Tweener<T, TTweenData> From(T from)
        {
            _from = true;
            _startValue = from;
            return this;
        }

        public Tweener<T, TTweenData> SetLoop(bool flag, LoopType loopType = LoopType.Normal)
        {
            _loop = flag;
            _loopType = loopType;
            return this;
        }

        public void Play()
        {
            if (IsPlaying)
                return;

            IsPlaying = true;
            IsPausing = false;
            IsComplete = false;
            _stopwatch.Reset();
            _stopwatch.Start();
            if (_threadKey != -999)
            {
                if (MonoMultiThread.IsContainThreadWorker(_threadKey) && MonoMultiThread.IsContainPauseList(_threadKey))
                {
                    MonoMultiThread.ResumeThreadWorker(_threadKey);
                }
                else
                {
                    _threadKey = MonoMultiThread.InsertThreadWorker(WorkThreadAction, MainThreadAction, _startValue, _threadKey);
                }
            }
            else
            {
                _threadKey = MonoMultiThread.InsertThreadWorker(WorkThreadAction, MainThreadAction, _startValue);
            }
        }

        public void Stop()
        {
            if(MonoMultiThread.IsContainPauseList(_threadKey))
                MonoMultiThread.ResumeThreadWorker(_threadKey);
            if(MonoMultiThread.IsContainThreadWorker(_threadKey))
                MonoMultiThread.DeleteThreadWorker(_threadKey);

            IsPlaying = false;
            IsComplete = false;
            IsPausing = false;
        }

        public void Restart()
        {
            _durationValue = 0f;
            Play();
        }

        public void Pause()
        {
            MonoMultiThread.PauseThreadWorker(_threadKey);
        }

        public bool IsActive()
        {
            return IsPlaying || IsComplete || IsPausing;
        }

        private object WorkThreadAction(object _)
        {
            var easeValue = Utility.Curves.ExecuteEaseFunc(_ease, Mathf.Min(_durationValue / _duration, 1));
            _durationValue += _stopwatch.ElapsedTicks / 10000000f;
            _stopwatch.Restart();

            return _tweenFunc.Evaluate(_startValue, _endValue, _from, easeValue);
        }

        private bool MainThreadAction(object obj)
        {
            var value = (T) obj;

            _setter(value);
            OnUpdate?.Invoke();

            if (!(_durationValue > _duration))
            {
                return false;
            }
            
            OnComplete?.Invoke();
            IsComplete = !_loop;

            if (!_loop)
            {
                MonoMultiThread.instance.StartCoroutine(Disable(this));
                return true;
            }

            switch (_loopType)
            {
                case LoopType.Normal:
                    break;
                case LoopType.PingPong:
                    var tmp = _startValue;
                    _startValue = _endValue;
                    _endValue = tmp;
                    break;
                case LoopType.Continue:
                    if (_from)
                    {
                        _startValue = _getter();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            _durationValue = 0;
            
            return false;
        }

        private static IEnumerator Disable(Tweener<T,TTweenData> tweener)
        {
            yield return YieldManager.GetWaitForSeconds(2.0f);
            
            tweener.IsPlaying = false;
            tweener.IsComplete = false;
            tweener.IsPausing = false;
            
            yield break;
        }
    }

    public delegate void TweenerSetter<in T>(T value);
    public delegate T TweenerGetter<out T>();
    public delegate void TweenerCallback();

    public enum LoopType
    {
        Normal,
        PingPong,
        Continue
    }
}