using System.Diagnostics;
using Game.Threading;
using Game.Util;

namespace Game.Tweener.Core
{
    public class Tween
    {
        protected int ThreadKey;
        protected Stopwatch Stopwatch;
        
        protected Utility.Curves.Ease Ease;
        protected bool From;
        protected bool Loop;
        protected LoopType LoopType;
        protected float Duration;
        
        public bool IsPlaying { get; protected set; }
        public bool IsPausing { get; protected set; }
        public bool IsComplete { get; protected set; }
        
        public TweenerCallback OnUpdate;
        public TweenerCallback OnComplete;
        
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
    }
}