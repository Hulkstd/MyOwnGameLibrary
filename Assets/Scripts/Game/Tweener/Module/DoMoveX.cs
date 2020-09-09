using Game.Tweener.TweenData;
using Game.Util.Extensions;

namespace Game.Tweener.Module
{
    public class DoMoveX : DoTweener<FloatTweenData, float>
    {
        private void Awake()
        {
            Tweener = transform.DoMoveX(_endValue, _duration);
            OnValidate();
            Tweener.Play();
        }
    }
}