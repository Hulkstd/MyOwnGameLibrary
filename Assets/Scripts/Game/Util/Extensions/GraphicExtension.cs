using Game.Tweener.Core;
using Game.Tweener.TweenData;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Util.Extensions
{
    public static class GraphicExtension
    {
        public static Tweener<Color, ColorTweenData> DoAlphaFade(this Graphic image, float alphaEndValue, float duration)
        {
            var color = image.color;
            return Tweener<Color, ColorTweenData>.To(
                obj => image.color = obj,
                () => image.color,
                new ColorTweenData(),
                new Color(color.r, color.g, color.b, alphaEndValue),
                duration);
        }

        public static Tweener<Color, ColorTweenData> DoFade(this Graphic image, Color endValue, float duration)
        {
            return Tweener<Color, ColorTweenData>.To(
                obj => image.color = obj,
                () => image.color,
                new ColorTweenData(),
                endValue,
                duration);
        }
    }
}
