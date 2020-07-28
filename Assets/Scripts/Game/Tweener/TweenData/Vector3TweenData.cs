using UnityEngine;

namespace Game.Tweener.TweenData
{
    public class Vector3TweenData : ITweenData<Vector3>
    {
        public Vector3 Evaluate(Vector3 startValue, Vector3 endValue, bool @from, float easeData)
        {
            return @from ? 
                Vector3.Lerp(startValue, startValue + endValue, easeData) : 
                Vector3.Lerp(startValue, endValue, easeData);
        }
    }
}