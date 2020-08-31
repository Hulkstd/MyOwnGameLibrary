using System;
using Game.Tweener.Core;
using Game.Tweener.TweenData;
using UnityEngine;

namespace Game.Util.Extensions
{
    public enum AnchorPreset
    {
        Top,
        Middle,
        Bottom,
        Stretch,
    }
    
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

        public static void SetAnchor(this RectTransform rectTransform, AnchorPreset anchorPresetX, AnchorPreset anchorPresetY, (float x, float y) pivot)
        {
            var anchorMax = rectTransform.anchorMax;
            var anchorMin = rectTransform.anchorMin;
            
            switch (anchorPresetX)
            {
                case AnchorPreset.Top:
                    anchorMax.x = 1;
                    anchorMin.x = 1;
                    break;
                case AnchorPreset.Middle:
                    anchorMax.x = 0.5f;
                    anchorMin.x = 0.5f;
                    break;
                case AnchorPreset.Bottom:
                    anchorMax.x = 0;
                    anchorMin.x = 0;
                    break;
                case AnchorPreset.Stretch:
                    anchorMax.x = 1;
                    anchorMin.x = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(anchorPresetX), anchorPresetX, null);
            }

            switch (anchorPresetY)
            {
                case AnchorPreset.Top:
                    anchorMax.y = 1;
                    anchorMin.y = 1;
                    break;
                case AnchorPreset.Middle:
                    anchorMax.y = 0.5f;
                    anchorMin.y = 0.5f;
                    break;
                case AnchorPreset.Bottom:
                    anchorMax.y = 0;
                    anchorMin.y = 0;
                    break;
                case AnchorPreset.Stretch:
                    anchorMax.y = 1;
                    anchorMin.y = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(anchorPresetY), anchorPresetY, null);
            }

            rectTransform.anchorMax = anchorMax;
            rectTransform.anchorMin = anchorMin;
            rectTransform.pivot = pivot.ToVector2();
        }
    }
}