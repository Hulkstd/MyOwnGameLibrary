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

        public virtual void SetContentData(TData tdata)
        {
            _data = tdata;
        }
    }
}
