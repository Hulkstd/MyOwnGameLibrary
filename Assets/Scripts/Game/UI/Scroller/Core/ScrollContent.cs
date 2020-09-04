using System;
using UnityEngine;

namespace Game.UI.Scroller.Core
{
    public static class ScrollContent
    {
        [Serializable]
        public class ScrollContentData
        {
            public ScrollContentData()
            {
            }
        }
    }

    public class ScrollContent<TData> : MonoBehaviour where TData : ScrollContent.ScrollContentData
    {
        private TData _data;
        public RectTransform RectTransform { get; private set; }

        protected virtual void Awake()
        {
            RectTransform = (RectTransform)transform;
        }
        
        public virtual void SetContentData(TData tdata)
        {
            _data = tdata;
        }
    }
}
