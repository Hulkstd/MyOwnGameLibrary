using System.Collections;
using System.Diagnostics;
using Game.Threading;
using Game.Util;

namespace Game.Tweener.Core
{
    public class Tween : Utility.IPoolingData
    {
        protected int ThreadKey;
        protected Stopwatch Stopwatch;
        protected float DurationValue;
        
        public bool From { get; protected set; }
        public bool Loop { get; protected set; }
        public LoopType LoopType { get; protected set; }
        public float Duration { get; protected set; }
        public bool IsPlaying { get; protected set; }
        public bool IsPausing { get; protected set; }
        public bool IsComplete { get; protected set; }
        
        public TweenerCallback OnUpdate;
        public TweenerCallback OnComplete;

        #region Static Function

        protected static IEnumerator Disable(Tween sequence)
        {
            yield return YieldManager.GetWaitForSeconds(2.0f);

            sequence.IsPlaying = false;
            sequence.IsComplete = false;
            sequence.IsPausing = false;
            
            yield break;
        }

        #endregion

        public virtual void Play()
        {
            if (IsPlaying)
                return;

            IsPlaying = true;
            IsPausing = false;
            IsComplete = false;
            Stopwatch.Reset();
            Stopwatch.Start();
        }

        public virtual void Stop()
        {
            if(MonoMultiThread.IsContainPauseList(ThreadKey))
                MonoMultiThread.ResumeThreadWorker(ThreadKey);
            if(MonoMultiThread.IsContainThreadWorker(ThreadKey))
                MonoMultiThread.DeleteThreadWorker(ThreadKey);

            IsPlaying = false;
            IsComplete = false;
            IsPausing = false;
        }

        public virtual void Restart()
        {
            Play();
        }

        public virtual void Pause()
        {
            MonoMultiThread.PauseThreadWorker(ThreadKey);
            IsPlaying = false;
            IsPausing = true;
            Stopwatch.Stop();
        }

        protected internal void SetDurationValue(float value)
        {
            DurationValue = value;
        }

        public virtual bool IsActive()
        {
            return IsPlaying || IsComplete || IsPausing;
        }
    }
}