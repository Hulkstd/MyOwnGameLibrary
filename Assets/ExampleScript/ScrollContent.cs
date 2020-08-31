using TMPro;

namespace ExampleScript
{
    public class ScrollContent : Game.UI.Scroller.Core.ScrollContent<ScrollContent.ScrollContentData>
    {
        public TextMeshProUGUI text;

        public override void SetContentData(ScrollContentData tdata)
        {
            base.SetContentData(tdata);

            text.text = tdata.index.ToString();
        }

        public class ScrollContentData : Game.UI.Scroller.Core.ScrollContent.ScrollContentData
        {
            public int index;
        }
    }
}