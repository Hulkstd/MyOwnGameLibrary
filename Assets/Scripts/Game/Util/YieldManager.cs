using System.Collections.Generic;
using UnityEngine;

namespace Game.Util
{
    public static class YieldManager
    {
        private static readonly Dictionary<float, WaitForSeconds> WaitForSeconds =
            new Dictionary<float, WaitForSeconds>();
        private static readonly Dictionary<float, WaitForSecondsRealtime> WaitForSecondsRealtimes =
            new Dictionary<float, WaitForSecondsRealtime>();
        private static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();
        private static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();

        private static WaitForSeconds PushDataOnWaitForSeconds(float a, WaitForSeconds data) 
        { 
            WaitForSeconds.Add(a, data);
            
            return data;
        }

        public static WaitForSeconds GetWaitForSeconds(float value)
        {
            if (WaitForSeconds.ContainsKey(value))
                return WaitForSeconds[value];
            return PushDataOnWaitForSeconds(value, new WaitForSeconds(value));
        }
        
        private static WaitForSecondsRealtime PushDataOnWaitForSecondRealtime(float a, WaitForSecondsRealtime data) 
        { 
            WaitForSecondsRealtimes.Add(a, data);
            
            return data;
        }
        
        public static WaitForSecondsRealtime GetWaitForSecondsRealtime(float value)
        {
            if (WaitForSecondsRealtimes.ContainsKey(value))
                return WaitForSecondsRealtimes[value];
            return PushDataOnWaitForSecondRealtime(value, new WaitForSecondsRealtime(value));
        }

        public static WaitForFixedUpdate GetWaitForFixedUpdate() => WaitForFixedUpdate;

        public static WaitForEndOfFrame GetWaitForEndOfFrame() => WaitForEndOfFrame;
    }
}
