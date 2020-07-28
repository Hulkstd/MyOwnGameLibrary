namespace Game.Tweener.TweenData
{
    public interface ITweenData<T1>
    {
        T1 Evaluate(T1 startValue, T1 endValue, bool from, float easeData);
    }
}