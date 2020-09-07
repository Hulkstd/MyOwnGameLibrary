using Game.Tweener.TweenData;
using Game.Util.Extensions;
using UnityEngine;

namespace Game.Tweener.Module
{
    public class DoMove : DoTweener<Vector3TweenData, Vector3>
    {
        private void Awake()
        {
            Tweener = transform.DoMove(_endValue, _duration);
            OnValidate();
            Tweener.Play();
        }
    }
}