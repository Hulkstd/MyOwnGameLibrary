using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Game.Threading;
using Game.Util;
using UnityEngine;

namespace Game.Tweener.Core
{
    public class Sequence : Tween
    {
        private static readonly Utility.ObjectPooling<Sequence> SequenceObjectPooling = 
            new Utility.ObjectPooling<Sequence>();
     
        private Dictionary<float, List<Tween>> _tweens;
        private Dictionary<float, TweenerCallback> _tweenerCallbacks;

        private float _lastPosition = 0f;
        private float _lastDuration = 0f;
        private float _totalDuration = 0f;
        private float _prevDurationValue;

        private static float _timeScale = 1;
        
        public Sequence()
        {
            Initialize();
        }
        
        public static Sequence GetSequence()
        {
            return SequenceObjectPooling.PopObject();
        }

        private void Initialize()
        {
            ThreadKey = -999;
            Stopwatch = Stopwatch.StartNew();
            
            IsPlaying = false;
            IsPausing = false;
            IsComplete = true;
            From = false;

            _tweens = new Dictionary<float, List<Tween>>();
            _tweenerCallbacks = new Dictionary<float, TweenerCallback>();
            _timeScale = Time.timeScale;
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
                    ThreadKey = MonoMultiThread.InsertThreadWorker(WorkerThreadAction, MainThreadAction, 0, ThreadKey);
                }
            }
            else
            {
                ThreadKey = MonoMultiThread.InsertThreadWorker(WorkerThreadAction, MainThreadAction, 0);
            }
            
            base.Play();
        }

        #region Sequence Function
        
        public Sequence Append(Tween tween)
        {
            if(!_tweens.ContainsKey(_lastPosition + _lastDuration))
                _tweens.Add(_lastPosition + _lastDuration, new List<Tween>());
            
            _tweens[_lastPosition + _lastDuration].Add(tween);
            _lastPosition += _lastDuration;
            _lastDuration = tween.Duration;
            _totalDuration += tween.Duration;
            
            return this;
        }

        public Sequence AppendCallback(TweenerCallback tweenerCallback)
        {
            if (!_tweenerCallbacks.ContainsKey(_lastPosition + _lastDuration))
                _tweenerCallbacks.Add(_lastPosition + _lastDuration, tweenerCallback);
            else
                _tweenerCallbacks[_lastPosition + _lastDuration] += tweenerCallback;
            
            _lastPosition += _lastDuration;
            _lastDuration = 0;
            
            return this;
        }
        
        public Sequence AppendInterval(float interval)
        {
            _lastPosition += _lastDuration;
            _lastDuration = interval;
            _totalDuration += interval;

            return this;
        }

        public Sequence Join(Tween tween)
        {
            if(!_tweens.ContainsKey(_lastPosition))
                _tweens.Add(_lastPosition, new List<Tween>());
            
            _tweens[_lastPosition].Add(tween);
            _lastDuration = tween.Duration;
            _totalDuration += tween.Duration;
            
            return this;
        }

        public Sequence JoinCallback(TweenerCallback tweenerCallback)
        {
            if (!_tweenerCallbacks.ContainsKey(_lastPosition))
                _tweenerCallbacks.Add(_lastPosition, tweenerCallback);
            else
                _tweenerCallbacks[_lastPosition] += tweenerCallback;
            
            _lastDuration = 0;

            return this;
        }

        public Sequence Insert(float atPosition, Tween tween)
        {            
            if(!_tweens.ContainsKey(atPosition))
                _tweens.Add(atPosition, new List<Tween>());
            
            _tweens[atPosition].Add(tween);
            _lastPosition = atPosition;
            _lastDuration = tween.Duration;
            _totalDuration += tween.Duration;
            
            return this;
        }
        
        public Sequence InsertCallback(float atPosition, TweenerCallback tweenerCallback)
        {            
            if (!_tweenerCallbacks.ContainsKey(atPosition))
                _tweenerCallbacks.Add(atPosition, tweenerCallback);
            else
                _tweenerCallbacks[atPosition] += tweenerCallback;
            
            _lastPosition = atPosition;

            return this;
        }
        
        public Sequence Prepend(Tween tween)
        {
            if(!_tweens.ContainsKey(0))
                _tweens.Add(0, new List<Tween>());
            
            _tweens[0].Add(tween);
            _lastPosition = 0;
            _lastDuration = tween.Duration;
            _totalDuration += tween.Duration;
            
            return this;
        }
        
        public Sequence PrependCallback(TweenerCallback tweenerCallback)
        {
            if (!_tweenerCallbacks.ContainsKey(0))
                _tweenerCallbacks.Add(0, tweenerCallback);
            else
                _tweenerCallbacks[0] += tweenerCallback;

            _lastPosition = 0;
            _lastDuration = 0;
            
            return this;
        }
        
        public Sequence PrependInterval(float interval)
        {
            var keys = _tweens.Keys.OrderByDescending(val => val).ToList();

            foreach (var key in keys)
            {
                _tweens[key + interval] = _tweens[key];
                _tweens.Remove(key);
            }
            
            keys = _tweenerCallbacks.Keys.OrderByDescending(val => val).ToList();

            foreach (var key in keys)
            {
                _tweenerCallbacks[key + interval] = _tweenerCallbacks[key];
                _tweenerCallbacks.Remove(key);
            }
            
            _totalDuration += interval;
            
            return this;
        }
        
        #endregion

        private object WorkerThreadAction(object _)
        {
            _prevDurationValue = DurationValue;
            DurationValue += (Stopwatch.ElapsedTicks / 10000000f) * _timeScale;
            Stopwatch.Restart();

            var tweenContents = _tweens
                .Where(valuePair => _prevDurationValue <= valuePair.Key && valuePair.Key <= DurationValue);

            foreach (var keyValuePair in tweenContents)
            {
                foreach (var tween in keyValuePair.Value)
                {
                    tween.SetDurationValue(DurationValue - keyValuePair.Key);
                    tween.Play();
                }
            }
            
            var callBackContents = _tweenerCallbacks
                .Where(valuePair => _prevDurationValue <= valuePair.Key && valuePair.Key <= DurationValue);

            foreach (var keyValuePair in callBackContents)
            {
                keyValuePair.Value.Invoke();
            }

            return DurationValue;
        }

        private bool MainThreadAction(object _)
        {
            _timeScale = Time.timeScale;
            return DurationValue > _totalDuration;
        }
    }
}
