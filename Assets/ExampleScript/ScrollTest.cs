using System;
using System.Collections.Generic;
using UnityEngine;

namespace ExampleScript
{
    [ExecuteInEditMode]
    public class ScrollTest : Game.UI.Scroller.FancyScroll.FancyScroll<ScrollContent, ScrollContent.ScrollContentData>
    {
        private void Start()
        {
            var contentDatas = new List<ScrollContent.ScrollContentData>();
            for (var i = 1; i <= 20; i++)
            {
                contentDatas.Add(new ScrollContent.ScrollContentData(){Index = i});
            }
            
            SetData(contentDatas);
        }
    }
}