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
    public class Tweener<T, TTweenData> : Tween
        where TTweenData : ITweenData<T>
    {
        private static float _timeScale;
        
        private TweenerSetter<T> _setter;
        private TweenerGetter<T> _getter;
        private TTweenData _tweenFunc;
        private Utility.Curves.Ease _ease;

        private T _startValue;
        private T _endValue;

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
            ThreadKey = -999;
            Stopwatch = Stopwatch.StartNew();
            
            IsPlaying = false;
            IsPausing = false;
            IsComplete = true;
            _ease = Utility.Curves.Ease.InSine;
            base.From = false;

            _setter = setter;
            _getter = getter;
            _tweenFunc = tweenData;
            _startValue = getter();
            _endValue = endValue;
            Duration = duration;
            DurationValue = 0f;
            
            _timeScale = Time.timeScale;
        }

        public Tweener<T, TTweenData> SetEase(Utility.Curves.Ease ease)
        {
            _ease = ease;
            return this;
        }

        public Tweener<T, TTweenData> SetDuration(float duration)
        {
            Duration = duration;
            return this;
        }

        public Tweener<T, TTweenData> From(T from)
        {
            base.From = true;
            _startValue = from;
            return this;
        }

        public Tweener<T, TTweenData> SetLoop(bool flag, LoopType loopType = LoopType.Normal)
        {
            Loop = flag;
            LoopType = loopType;
            return this;
        }

        public override void Play()
        {
            if (IsPlaying)
                return;
            
            if (ThreadKey != -999)
            {
                if (MonoMultiThread.IsContainThreadWorker(ThreadKey) && MonoMultiThread.IsContainPauseList(ThreadKey))
                {
                    MonoMultiThread.ResumeThreadWorker(ThreadKey);
                    Stopwatch.Start();
                }
                else
                {
                    ThreadKey = MonoMultiThread.InsertThreadWorker(WorkThreadAction, MainThreadAction, _startValue, ThreadKey);
                }
            }
            else
            {
                ThreadKey = MonoMultiThread.InsertThreadWorker(WorkThreadAction, MainThreadAction, _startValue);
            }
            
            base.Play();
        }

        public override void Restart()
        {
            DurationValue = 0f;
            base.Restart();
        }

        private object WorkThreadAction(object _)
        {
            var easeValue = Utility.Curves.ExecuteEaseFunc(_ease, Mathf.Clamp(DurationValue / Duration, 0, 1));
            DurationValue += (Stopwatch.ElapsedTicks / 10000000f) * _timeScale;
            Stopwatch.Restart();

            return _tweenFunc.Evaluate(_startValue, _endValue, base.From, easeValue);
        }

        private bool MainThreadAction(object obj)
        {
            _timeScale = Time.timeScale;
            var value = (T) obj;

            _setter(value);
            OnUpdate?.Invoke();

            if (!(DurationValue > Duration))
            {
                return false;
            }
            
            OnComplete?.Invoke();
            IsComplete = !Loop;

            if (!Loop)
            {
                MonoMultiThread.Instance.StartCoroutine(Disable(this));
                return true;
            }

            switch (LoopType)
            {
                case LoopType.Normal:
                    break;
                case LoopType.PingPong:
                    var tmp = _startValue;
                    _startValue = _endValue;
                    _endValue = tmp;
                    break;
                case LoopType.Continue:
                    if (base.From)
                    {
                        _startValue = _getter();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            DurationValue = 0;
            
            return false;
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