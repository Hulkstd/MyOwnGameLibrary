using TMPro;

namespace ExampleScript
{
    public class ScrollContent : Game.UI.Scroller.Core.ScrollContent<ScrollContent.ScrollContentData>
    {
        public TextMeshProUGUI _text;

        public override void SetContentData(ScrollContentData tdata)
        {
            base.SetContentData(tdata);

            _text.text = tdata.Index.ToString();
        }

        public class ScrollContentData : Game.UI.Scroller.Core.ScrollContent.ScrollContentData
        {
            public int Index;
        }
    }
}