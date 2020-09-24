using Game.Tweener.TweenData;
using Game.Util;

namespace Game.Tweener.Core
{
    public static class TweenerManager<T1, TTweenData>
        where TTweenData : ITweenData<T1>
    {
        private static readonly Utility.ObjectPooling<Tweener<T1, TTweenData>> TweenerObjectPooling =
            new Utility.ObjectPooling<Tweener<T1, TTweenData>>();

        public static Tweener<T1, TTweenData> GetTweener()
        {
            return TweenerObjectPooling.PopObject();
        }
    }
}