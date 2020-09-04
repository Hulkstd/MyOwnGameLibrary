using System;
using Game.Tweener.Core;
using Game.Tweener.TweenData;
using Game.Util;
using UnityEngine;

namespace Game.Tweener.Module
{
    [Serializable]
    public struct LoopProperty
    {
        public bool _loop;
        public LoopType _loopType;
    }
    public class DoTweener<TData, TDataType> : MonoBehaviour
        where TData : ITweenData<TDataType>
    {
        public Utility.Curves.Ease _ease;

        public TDataType _endValue;
        public float _duration;

        public LoopProperty _loopProperty;

        protected Tweener<TDataType, TData> Tweener;

        protected void OnValidate()
        {
            if (_loopProperty._loopType == LoopType.Continue)
                _loopProperty._loopType = LoopType.Normal;
            
            Tweener?.SetLoop(_loopProperty._loop, _loopProperty._loopType)
                .SetDuration(_duration)
                .SetEase(_ease);
        }
    }
}