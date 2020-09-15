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
    public class Tweener<T, TTweenData> : Tween, Utility.IPoolingData
        where TTweenData : ITweenData<T>
    {
        private TweenerSetter<T> _setter;
        private TweenerGetter<T> _getter;
        private TTweenData _tweenFunc;

        private T _startValue;
        private T _endValue;
        private float _durationValue;

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
            Ease = Utility.Curves.Ease.InSine;
            base.From = false;

            _setter = setter;
            _getter = getter;
            _tweenFunc = tweenData;
            _startValue = getter();
            _endValue = endValue;
            Duration = duration;
            _durationValue = 0f;
        }

        public Tweener<T, TTweenData> SetEase(Utility.Curves.Ease ease)
        {
            Ease = ease;
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
            base.Play();
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
        }

        public override void Restart()
        {
            _durationValue = 0f;
            base.Restart();
        }

        public bool IsActive()
        {
            return IsPlaying || IsComplete || IsPausing;
        }

        private object WorkThreadAction(object _)
        {
            var easeValue = Utility.Curves.ExecuteEaseFunc(Ease, Mathf.Min(_durationValue / Duration, 1));
            _durationValue += Stopwatch.ElapsedTicks / 10000000f;
            Stopwatch.Restart();

            return _tweenFunc.Evaluate(_startValue, _endValue, base.From, easeValue);
        }

        private bool MainThreadAction(object obj)
        {
            var value = (T) obj;

            _setter(value);
            OnUpdate?.Invoke();

            if (!(_durationValue > Duration))
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