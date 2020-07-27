using System;
using UnityEngine;

namespace Game.UI.Scroller.SlotTurnover
{
    public static class SlotTurnoverContent
    {
        [Serializable]
        public class SlotTurnoverContentData
        {
            public SlotTurnoverContentData()
            {
            }
        }
    }

    public class SlotTurnoverContent<TData> : MonoBehaviour where TData : SlotTurnoverContent.SlotTurnoverContentData
    {
        private TData _data;

        public virtual void SetContentData(TData tdata)
        {
            _data = tdata;
        }
    }
}
