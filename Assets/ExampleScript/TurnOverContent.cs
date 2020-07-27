using Game.UI.Scroller.SlotTurnover;
using UnityEngine.UI;

namespace ExampleScript
{
    public class TurnOverContent : SlotTurnoverContent<TurnOverContent.TurnoverContentData>
    {
        public Text text;
        public class TurnoverContentData : SlotTurnoverContent.SlotTurnoverContentData
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
