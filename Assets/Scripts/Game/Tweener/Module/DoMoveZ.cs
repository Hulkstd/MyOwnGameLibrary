using Game.Tweener.TweenData;
using Game.Util.Extensions;

namespace Game.Tweener.Module
{
    public class DoMoveZ : DoTweener<FloatTweenData, float>
    {
        private void Awake()
        {
            Tweener = transform.DoMoveZ(_endValue, _duration);
            OnValidate();
            Tweener.Play();
        }
    }
}