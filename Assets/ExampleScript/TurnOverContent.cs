using Game.UI.Scroller.Core;
using Game.UI.Scroller.SlotTurnover;
using UnityEngine.UI;

namespace ExampleScript
{
    public class TurnOverContent : ScrollContent<TurnOverContent.TurnoverContentData>
    {
        public Text text;
        public class TurnoverContentData : ScrollContent.ScrollContentData
        {
            public string title;
        }

        public override void SetContentData(TurnoverContentData tdata)
        {
            base.SetContentData(tdata);

            text.text = tdata.title;
        }
    }
}
