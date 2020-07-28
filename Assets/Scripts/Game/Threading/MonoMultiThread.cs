using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Game.Util;
using UnityEngine;

namespace Game.Threading
{
    public class MonoMultiThread : MonoSingleton<MonoMultiThread>
    {
        private class WorkerThread
        {
            private static MonoMultiThread _mainThread;
            
            private readonly Func<object, object> _update;
            private EventWaitHandle _mainWaitHandle;
            
            public readonly int Key;
            public readonly Func<object, bool> MainThreadUpdate;
            public readonly EventWaitHandle ChildWaitHandle = new EventWaitHandle(true, EventResetMode.ManualReset);
            public object Data;
            public bool DoneUpdate;
            public bool Stop = false;

            public static bool operator ==(WorkerThread workerThread, int key)
            {
                return workerThread != null && workerThread.Key == key;
            }

            public static bool operator !=(WorkerThread workerThread, int key)
            {
                return !(workerThread == key);
            }

            public WorkerThread(MonoMultiThread mono, Func<object, object> update, Func<object, bool> mainThreadUpdate,
                object initializeData, int key)
            {
                Key = key;
                _mainThread = mono;

                Data = initializeData;
                MainThreadUpdate = mainThreadUpdate;
                _update = update;
                DoneUpdate = false;
            }

            public void Update()
            {
                _mainWaitHandle = GetMainWaitHandle(this, ChildWaitHandle);

                while (!Stop)
                {
                    ChildWaitHandle.Reset();
                    DoneUpdate = false;
                    
                    Data = _update.Invoke(Data);

                    DoneUpdate = true;
                    WaitHandle.SignalAndWait(_mainWaitHandle, ChildWaitHandle);
                }
            }
        }

        private static readonly object LockMainThreadPool = new object();
        private static readonly Dictionary<WorkerThread, (EventWaitHandle main, EventWaitHandle child)> MainThreadPool =
            new Dictionary<WorkerThread, (EventWaitHandle main, EventWaitHandle child)>();
        private static readonly List<int> PauseList = new List<int>();
        private static readonly List<WorkerThread> DeleteList = new List<WorkerThread>();
        private static int _workerThreadKey = 0;

        private void Update()
        {
            DeleteList.Clear();

            lock (LockMainThreadPool)
            {
                var list = MainThreadPool.Where(valueTuple => valueTuple.Key.DoneUpdate).ToList();
                list = list.Where(valueTuple => !PauseList.Contains(valueTuple.Key.Key)).ToList();
                foreach (var valueTuple in list)
                {
                    valueTuple.Value.main.WaitOne();
                    valueTuple.Value.main.Reset();

                    // todo: Copy Results out of the thread
                    var data = valueTuple.Key.Data;
                    // todo: Copy pending changes into the thread
                    var flag = valueTuple.Key.MainThreadUpdate.Invoke(data);
                    valueTuple.Key.Data = data;

                    if (flag)
                    {
                        DeleteList.Add(valueTuple.Key);
                    }
                    else
                    {
                        valueTuple.Value.child.Set();
                    }
                }

                foreach (var value in DeleteList)
                {
                    value.Stop = true;

                    var valueTuple = MainThreadPool[value];
                    valueTuple.child.Set();
                    MainThreadPool.Remove(value);
                }
            }
        }

        public static int InsertThreadWorker(Func<object, object> action, Func<object, bool> mainThreadAction,
            object initializeData, int threadKey = -999)
        {
            threadKey = threadKey == -999 ? _workerThreadKey++ : threadKey;
            var workerThread = new WorkerThread(instance, action, mainThreadAction, initializeData, threadKey);

            var thread = new Thread(workerThread.Update);
            thread.Start();

            workerThread.ChildWaitHandle.Set();

            return threadKey;
        }

        public static void PauseThreadWorker(int key)
        {
            PauseList.Add(key);
        }

        public static bool IsContainPauseList(int key)
        {
            return PauseList.Contains(key);
        }

        public static void ResumeThreadWorker(int key)
        {
            if(!PauseList.Remove(key))
                Debug.LogError($"Can`t exist key {key} in Threads");
        }

        public static void DeleteThreadWorker(int key)
        {
            lock (LockMainThreadPool)
            {
                var workerThread = MainThreadPool.Keys.FirstOrDefault(thread => thread == key);
                if(workerThread == null)
                    throw new ArgumentNullException($"MainThreadPool dont has {key}");
                MainThreadPool.Remove(workerThread);
            }
        }

        public static bool IsContainThreadWorker(int key)
        {
            lock (LockMainThreadPool)
            {
                return MainThreadPool.Keys.Any(worker => worker == key);
            }
        }

        private static EventWaitHandle GetMainWaitHandle(WorkerThread workerThread, EventWaitHandle childWaitHandle)
        {
            lock (LockMainThreadPool)
            {
                if (!MainThreadPool.ContainsKey(workerThread))
                {
                    MainThreadPool.Add(workerThread,
                        (new EventWaitHandle(true, EventResetMode.ManualReset), childWaitHandle));
                }

                return MainThreadPool[workerThread].main;
            }
        }
    }
}
