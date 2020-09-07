using Game.Tweener.TweenData;
using Game.Util.Extensions;

namespace Game.Tweener.Module
{
    public class DoMoveY : DoTweener<FloatTweenData, float>
    {
        private void Awake()
        {
            Tweener = transform.DoMoveY(_endValue, _duration);
            OnValidate();
            Tweener.Play();
        }
    }
}