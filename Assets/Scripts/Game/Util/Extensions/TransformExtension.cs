using System;
using Game.Tweener.Core;
using Game.Tweener.TweenData;
using UnityEngine;

namespace Game.Util.Extensions
{
    public static class TransformExtension
    {
        public static Tweener<Vector3, Vector3TweenData> DoMove(this Transform transform, Vector3 endValue,
            float duration)
        {
            return Tweener<Vector3, Vector3TweenData>.To(
                value => transform.position = value,
                () => transform.position,
                new Vector3TweenData(),
                endValue,
                duration);
        }

        public static Tweener<float, FloatTweenData> DoMoveX(this Transform transform, float endValue, float duration)
        {
            var setter = new TweenerSetter<float>(value =>
            {
                var position = transform.position;
                position.x = value;
                transform.position = position;
            });

            return Tweener<float, FloatTweenData>.To(
                setter,
                () => transform.position.x,
                new FloatTweenData(),
                endValue,
                duration);
        }
        
        public static Tweener<float, FloatTweenData> DoMoveY(this Transform transform, float endValue, float duration)
        {
            var setter = new TweenerSetter<float>(value =>
            {
                var position = transform.position;
                position.y = value;
                transform.position = position;
            });

            return Tweener<float, FloatTweenData>.To(
                setter,
                () => transform.position.y,
                new FloatTweenData(),
                endValue,
                duration);
        }
        
        public static Tweener<float, FloatTweenData> DoMoveZ(this Transform transform, float endValue, float duration)
        {
            var setter = new TweenerSetter<float>(value =>
            {
                var position = transform.position;
                position.z = value;
                transform.position = position;
            });

            return Tweener<float, FloatTweenData>.To(
                setter,
                () => transform.position.z,
                new FloatTweenData(),
                endValue,
                duration);
        }
        
        public static Tweener<Vector3, Vector3TweenData> DoLocalMove(this Transform transform, Vector3 endValue,
            float duration)
        {
            return Tweener<Vector3, Vector3TweenData>.To(
                value => transform.localPosition = value,
                () => transform.localPosition,
                new Vector3TweenData(),
                endValue,
                duration);
        }

        public static Tweener<float, FloatTweenData> DoLocalMoveX(this Transform transform, float endValue, float duration)
        {
            var setter = new TweenerSetter<float>(value =>
            {
                var position = transform.localPosition;
                position.x = value;
                transform.localPosition = position;
            });

            return Tweener<float, FloatTweenData>.To(
                setter,
                () => transform.localPosition.x,
                new FloatTweenData(),
                endValue,
                duration);
        }
        
        public static Tweener<float, FloatTweenData> DoLocalMoveY(this Transform transform, float endValue, float duration)
        {
            var setter = new TweenerSetter<float>(value =>
            {
                var position = transform.localPosition;
                position.y = value;
                transform.localPosition = position;
            });

            return Tweener<float, FloatTweenData>.To(
                setter,
                () => transform.localPosition.y,
                new FloatTweenData(),
                endValue,
                duration);
        }
        
        public static Tweener<float, FloatTweenData> DoLocalMoveZ(this Transform transform, float endValue, float duration)
        {
            var setter = new TweenerSetter<float>(value =>
            {
                var position = transform.localPosition;
                position.z = value;
                transform.localPosition = position;
            });

            return Tweener<float, FloatTweenData>.To(
                setter,
                () => transform.localPosition.z,
                new FloatTweenData(),
                endValue,
                duration);
        }

        public static Tweener<Quaternion, QuaternionTweenData> DoRotate(this Transform transform, Quaternion endValue,
            float duration)
        {
            return Tweener<Quaternion, QuaternionTweenData>.To(
                value => transform.rotation = value,
                () => transform.rotation,
                new QuaternionTweenData(),
                endValue,
                duration);
        }

        public static Tweener<float, FloatTweenData> DoRotateX(this Transform transform, float endValue,
            float duration)
        {
            var setter = new TweenerSetter<float>(value =>
            {
                var eulerAngles = transform.rotation.eulerAngles;
                eulerAngles.x = value;
                transform.rotation = Quaternion.Euler(eulerAngles);
            });

            return Tweener<float, FloatTweenData>.To(
                setter,
                () => transform.rotation.eulerAngles.x,
                new FloatTweenData(),
                endValue,
                duration);
        }
        
        public static Tweener<float, FloatTweenData> DoRotateY(this Transform transform, float endValue,
            float duration)
        {
            var setter = new TweenerSetter<float>(value =>
            {
                var eulerAngles = transform.rotation.eulerAngles;
                eulerAngles.y = value;
                transform.rotation = Quaternion.Euler(eulerAngles);
            });

            return Tweener<float, FloatTweenData>.To(
                setter,
                () => transform.rotation.eulerAngles.y,
                new FloatTweenData(),
                endValue,
                duration);
        }
        
        public static Tweener<float, FloatTweenData> DoRotateZ(this Transform transform, float endValue,
            float duration)
        {
            var setter = new TweenerSetter<float>(value =>
            {
                var eulerAngles = transform.rotation.eulerAngles;
                eulerAngles.z = value;
                transform.rotation = Quaternion.Euler(eulerAngles);
            });

            return Tweener<float, FloatTweenData>.To(
                setter,
                () => transform.rotation.eulerAngles.z,
                new FloatTweenData(),
                endValue,
                duration);
        }

        public static Tweener<Vector3, Vector3TweenData> DoScale(this Transform transform, Vector3 endValue,
            float duration)
        {
            return Tweener<Vector3, Vector3TweenData>.To(
                value => transform.localScale = value,
                () => transform.localScale,
                new Vector3TweenData(),
                endValue,
                duration);
        }
        
        public static Tweener<Vector2, Vector2TweenData> DoScale(this Transform transform, Vector2 endValue,
            float duration)
        {
            return Tweener<Vector2, Vector2TweenData>.To(
                value => transform.localScale = value,
                () => transform.localScale,
                new Vector2TweenData(),
                endValue,
                duration);
        }

        public static Tweener<float, FloatTweenData> DoScaleX(this Transform transform, float endValue, float duration)
        {
            var setter = new TweenerSetter<float>(value =>
            {
                var scale = transform.localScale;
                scale.x = value;
                transform.localScale = scale;
            });

            return Tweener<float, FloatTweenData>.To(
                setter,
                () => transform.localScale.x,
                new FloatTweenData(),
                endValue,
                duration);
        }
        
        public static Tweener<float, FloatTweenData> DoScaleY(this Transform transform, float endValue, float duration)
        {
            var setter = new TweenerSetter<float>(value =>
            {
                var scale = transform.localScale;
                scale.y = value;
                transform.localScale = scale;
            });

            return Tweener<float, FloatTweenData>.To(
                setter,
                () => transform.localScale.y,
                new FloatTweenData(),
                endValue,
                duration);
        }
        
        public static Tweener<float, FloatTweenData> DoScaleZ(this Transform transform, float endValue, float duration)
        {
            var setter = new TweenerSetter<float>(value =>
            {
                var scale = transform.localScale;
                scale.z = value;
                transform.localScale = scale;
            });

            return Tweener<float, FloatTweenData>.To(
                setter,
                () => transform.localScale.z,
                new FloatTweenData(),
                endValue,
                duration);
        }
    }
}