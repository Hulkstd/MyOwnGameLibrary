using UnityEngine;

namespace Game.Tweener.TweenData
{
    public class QuaternionTweenData : ITweenData<Quaternion>
    {
        public Quaternion Evaluate(Quaternion startValue, Quaternion endValue, bool @from, float easeData)
        {
            return @from ?
                Quaternion.Lerp(startValue, startValue * endValue, easeData) :
                Quaternion.Lerp(startValue, endValue, easeData);
        }
    }
}