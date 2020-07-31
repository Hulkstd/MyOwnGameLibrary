using Game.Tweener.Core;
using Game.Tweener.TweenData;
using UnityEngine;

namespace Game.Util.Extensions
{
    public static class RectTransformExtension
    {
        public static Tweener<Vector2, Vector2TweenData> DoAnchorMove(this RectTransform rectTransform,
            Vector2 endValue, float duration)
        {
            return Tweener<Vector2, Vector2TweenData>.To(
                value => rectTransform.anchoredPosition = value,
                () => rectTransform.anchoredPosition,
                new Vector2TweenData(),
                endValue,
                duration);
        }

        public static Tweener<float, FloatTweenData> DoAnchorMoveX(this RectTransform rectTransform,
            float endValue, float duration)
        {
            var setter = new TweenerSetter<float>(
                value =>
                {
                    var position = rectTransform.anchoredPosition;
                    position.x = value;
                    rectTransform.anchoredPosition = position;
                });

            return Tweener<float, FloatTweenData>.To(
                setter,
                () => rectTransform.anchoredPosition.x,
                new FloatTweenData(),
                endValue,
                duration);
        }

        public static Tweener<float, FloatTweenData> DoAnchorMoveY(this RectTransform rectTransform,
            float endValue, float duration)
        {
            var setter = new TweenerSetter<float>(
                value =>
                {
                    var position = rectTransform.anchoredPosition;
                    position.y = value;
                    rectTransform.anchoredPosition = position;
                });

            return Tweener<float, FloatTweenData>.To(
                setter,
                () => rectTransform.anchoredPosition.y,
                new FloatTweenData(),
                endValue,
                duration);
        }

        public static Tweener<Vector2, Vector2TweenData> DoSize(this RectTransform rectTransform, Vector2 endValue,
            float duration)
        {
            return Tweener<Vector2, Vector2TweenData>.To(
                value => rectTransform.sizeDelta = value,
                () => rectTransform.sizeDelta,
                new Vector2TweenData(),
                endValue,
                duration);
        }

        public static Tweener<float, FloatTweenData> DoSizeX(this RectTransform rectTransform, float endValue,
            float duration)
        {
            var setter = new TweenerSetter<float>(
                value =>
                {
                    var sizeDelta = rectTransform.sizeDelta;
                    sizeDelta.x = value;
                    rectTransform.sizeDelta = sizeDelta;
                });

            return Tweener<float, FloatTweenData>.To(
                setter,
                () => rectTransform.sizeDelta.x,
                new FloatTweenData(),
                endValue,
                duration);
        }
        
        public static Tweener<float, FloatTweenData> DoSizeY(this RectTransform rectTransform, float endValue,
            float duration)
        {
            var setter = new TweenerSetter<float>(
                value =>
                {
                    var sizeDelta = rectTransform.sizeDelta;
                    sizeDelta.y = value;
                    rectTransform.sizeDelta = sizeDelta;
                });

            return Tweener<float, FloatTweenData>.To(
                setter,
                () => rectTransform.sizeDelta.y,
                new FloatTweenData(),
                endValue,
                duration);
        }
    }
}