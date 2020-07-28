using UnityEngine;

namespace Game.Tweener.TweenData
{
    public class Vector2TweenData : ITweenData<Vector2>
    {
        public Vector2 Evaluate(Vector2 startValue, Vector2 endValue, bool @from, float easeData)
        {
            return @from ? 
                Vector2.Lerp(startValue, startValue + endValue, easeData) :
                Vector2.Lerp(startValue, endValue, easeData);
        }
    }
}