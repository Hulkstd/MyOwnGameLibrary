using System;
using UnityEngine;

namespace Game.Tweener.TweenData
{
    public class ColorTweenData : ITweenData<Color>
    {
        public Color Evaluate(Color startValue, Color endValue, bool @from, float easeData)
        {
            var resultValue = endValue;

            return Color.Lerp(startValue, resultValue, easeData);
        }
    }
}