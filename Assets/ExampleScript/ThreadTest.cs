using System;
using System.Diagnostics;
using System.Threading;
using Game.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ExampleScript
{
    public class ThreadTest : MonoBehaviour
    {
        private int k = 0;
        
        private void Start()
        {
            var watch = Stopwatch.StartNew();
            watch.Stop();
            watch.Reset();

            MonoMultiThread.InsertThreadWorker(
                data =>
                {
                var dataToUse = (float) data;
                dataToUse += watch.ElapsedTicks / 10000000f;
                watch.Restart();
                Debug.Log(dataToUse);
                
                return dataToUse;
            }, data =>
            {
                var dataToUse = (float) data;
                
                if (dataToUse >= 10)
                {
//                    Destroy(gameObject);
                }
                else
                    return false;

                return true;
            }, 0f);
        }
    }
}
