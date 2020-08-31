using Game.UI.Scroller.Core;
using Game.UI.Scroller.SlotTurnover;
using UnityEngine.UI;

namespace ExampleScript
{
    public class TurnOverContent : ScrollContent<TurnOverContent.TurnoverContentData>
    {
        public Text _text;
        public class TurnoverContentData : ScrollContent.ScrollContentData
        {
            public string Title;
        }

        public override void SetContentData(TurnoverContentData tdata)
        {
            base.SetContentData(tdata);

            _text.text = tdata.Title;
        }
    }
}
