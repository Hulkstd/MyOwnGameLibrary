using UnityEngine;

namespace Game.Tweener.TweenData
{
    public class FloatTweenData : ITweenData<float>
    {
        public float Evaluate(float startValue, float endValue, bool @from, float easeData)
        {
            return @from ?
                Mathf.Lerp(startValue, startValue + endValue, easeData) :
                Mathf.Lerp(startValue, endValue, easeData);
        }
    }
}