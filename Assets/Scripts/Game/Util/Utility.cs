using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Util
{
    public static class Utility
    {
        public interface IPoolingData
        {
            bool IsActive();
        }
        
        public class MonoObjectPooling<T> where T : Component
        {
            private readonly T _originalPrefabs;
            private readonly Transform _parent;
            private readonly Queue<T> _objects;

            public MonoObjectPooling(T prefabs, Transform parent = null)
            {
                _originalPrefabs = prefabs;
                _parent = parent;
                _objects = new Queue<T>();
            }
            
            public MonoObjectPooling(T prefabs, Transform parent = null, int initializeCount = 0)
            {
                _originalPrefabs = prefabs;
                _parent = parent;
                _objects = new Queue<T>();

                for (var i = 0; i < initializeCount; i++)
                {
                    var obj = Object.Instantiate(_originalPrefabs);
                    _objects.Enqueue(obj);
                }
            }

            public T PopObject()
            {
                if (_objects.Count == 0 || _objects.Peek().gameObject.activeSelf)
                {
                    var returnObject = Object.Instantiate(_originalPrefabs);
                    if (_parent)
                        returnObject.transform.SetParent(_parent);
                    _objects.Enqueue(returnObject);
                    if (_objects.Count != 0) _objects.Enqueue(_objects.Dequeue());

                    return returnObject;
                }

                var returnValue = _objects.Peek();
                _objects.Enqueue(_objects.Dequeue());
                returnValue.gameObject.SetActive(true);

                return returnValue;
            }
        }

        public class ObjectPooling<T> where T : IPoolingData, new()
        {
            private readonly Queue<T> _objects;

            public ObjectPooling()
            {
                _objects = new Queue<T>();
            }
            
            public ObjectPooling(int initializeCount = 0)
            {
                _objects = new Queue<T>();

                for (var i = 0; i < initializeCount; i++)
                {
                    var obj = new T();
                    _objects.Enqueue(obj);
                }
            }

            public T PopObject()
            {
                if (_objects.Count != 0 && _objects.Any(obj => !obj.IsActive())) 
                    return _objects.FirstOrDefault(obj => !obj.IsActive());
                
                var returnObject = new T();
                _objects.Enqueue(returnObject);

                return returnObject;
            }
        }
        
        public class SortQueue<T> : ICloneable, IEnumerable<T>
        {
            public readonly List<T> List;
            private readonly System.Comparison<T> _comp;

            public SortQueue()
            {
                List = new List<T>();
                _comp = null;
            }

            public SortQueue(System.Comparison<T> comparison)
            {
                List = new List<T>();
                _comp = comparison;
            }

            private SortQueue(List<T> list, System.Comparison<T> comparison = null)
            {
                List = list.Count != 0 ? list.GetRange(0, list.Count) : new List<T>();

                _comp = comparison;
            }

            public T Top => List[0];

            public int Length => List.Count;

            public void Push(T value)
            {
                List.Add(value);

                if (_comp == null)
                    List.Sort();
                else
                    List.Sort(_comp);
            }

            public T Pop()
            {
                if (Length <= 0) throw new System.Exception("SortQueue empty");
                var ret = Top;
                List.RemoveAt(0);

                return ret;
            }

            public void Pop(T val)
            {
                if(Length <= 0) throw new System.Exception("SortQueue empty");

                List.Remove(val);
            }

            public void Sort() => List.Sort();

            public T this[int index] => List[index];

            public object Clone()
            {
                return new SortQueue<T>(List, _comp);
            }

            public IEnumerator<T> GetEnumerator()
            {
                return List.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public static class Curves
        {
            private static float Sin(float x) => Mathf.Sin(x);
            private static float Cos(float x) => Mathf.Cos(x);
            private static float Pow(float x, float y) => Mathf.Pow(x, y);
            private static float Sqrt(float x) => Mathf.Sqrt(x);
            private static float PI => Mathf.PI;
            private static float C1 => 1.70158f;
            private static float C2 => C1 * 1.525f;
            private static float C3 => C1 + 1;
            private static float C4 => (2 * PI) / 3f;
            private static float C5 => (2 * PI) / 4.5f;
            private static float N1 => 7.5625f;
            private static float D1 => 2.75f;
            
            public enum Ease
            {
                InSine,
                OutSine,
                InOutSine,
                InQuad,
                OutQuad,
                InOutQuad,
                InCubic,
                OutCubic,
                InOutCubic,
                InQuart,
                OutQuart,
                InOutQuart,
                InQuint,
                OutQuint,
                InOutQuint,
                InExpo,
                OutExpo,
                InOutExpo,
                InCirc,
                OutCirc,
                InOutCirc,
                InBack,
                OutBack,
                InOutBack,
                InElastic,
                OutElastic,
                InOutElastic,
                InBounce,
                OutBounce,
                InOutBounce,
            }

            public static float ExecuteEaseFunc(Ease ease, float x)
            {
                var value = 0f;

                switch (ease)
                {
                    case Ease.InSine:
                        value = EaseInSine(x);
                        break;
                    case Ease.OutSine:
                        value = EaseOutSine(x);
                        break;
                    case Ease.InOutSine:
                        value = EaseInOutSine(x);
                        break;
                    case Ease.InQuad:
                        value = EaseInQuad(x);
                        break;
                    case Ease.OutQuad:
                        value = EaseOutQuad(x);
                        break;
                    case Ease.InOutQuad:
                        value = EaseInOutQuad(x);
                        break;
                    case Ease.InCubic:
                        value = EaseInCubic(x);
                        break;
                    case Ease.OutCubic:
                        value = EaseOutCubic(x);
                        break;
                    case Ease.InOutCubic:
                        value = EaseInOutCubic(x);
                        break;
                    case Ease.InQuart:
                        value = EaseInQuart(x);
                        break;
                    case Ease.OutQuart:
                        value = EaseOutQuart(x);
                        break;
                    case Ease.InOutQuart:
                        value = EaseInOutQuart(x);
                        break;
                    case Ease.InQuint:
                        value = EaseInQuint(x);
                        break;
                    case Ease.OutQuint:
                        value = EaseOutQuint(x);
                        break;
                    case Ease.InOutQuint:
                        value = EaseInOutQuint(x);
                        break;
                    case Ease.InExpo:
                        value = EaseInExpo(x);
                        break;
                    case Ease.OutExpo:
                        value = EaseOutExpo(x);
                        break;
                    case Ease.InOutExpo:
                        value = EaseInOutExpo(x);
                        break;
                    case Ease.InCirc:
                        value = EaseInCirc(x);
                        break;
                    case Ease.OutCirc:
                        value = EaseOutCirc(x);
                        break;
                    case Ease.InOutCirc:
                        value = EaseInOutCirc(x);
                        break;
                    case Ease.InBack:
                        value = EaseInBack(x);
                        break;
                    case Ease.OutBack:
                        value = EaseOutBack(x);
                        break;
                    case Ease.InOutBack:
                        value = EaseInOutBack(x);
                        break;
                    case Ease.InElastic:
                        value = EaseInElastic(x);
                        break;
                    case Ease.OutElastic:
                        value = EaseOutElastic(x);
                        break;
                    case Ease.InOutElastic:
                        value = EaseInOutElastic(x);
                        break;
                    case Ease.InBounce:
                        value = EaseInBounce(x);
                        break;
                    case Ease.OutBounce:
                        value = EaseOutBounce(x);
                        break;
                    case Ease.InOutBounce:
                        value = EaseInOutBounce(x);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(ease), ease, null);
                }
                
                return value;
            }
            
            private static float EaseInSine(float x)
            {
                return 1 - Cos((x * PI) / 2);
            }
            private static float EaseOutSine(float x)
            {
                return Sin((x * PI) / 2);
            }
            private static float EaseInOutSine(float x)
            {
                return -(Cos(PI * x) - 1) / 2;
            }

            private static float EaseInQuad(float x)
            {
                return x * x;
            }
            private static float EaseOutQuad(float x)
            {
                return 1 - (1 - x) * (1 - x);
            }
            private static float EaseInOutQuad(float x)
            {
                return x < 0.5 ? 
                    2 * x * x : 
                    1 - Pow(-2 * x + 2, 2) / 2;
            }
            
            private static float EaseInCubic(float x)
            {
                return x * x * x;
            }
            private static float EaseOutCubic(float x)
            {
                return 1 - Pow(1 - x, 3);
            }
            private static float EaseInOutCubic(float x)
            {
                return x < 0.5 ? 
                    4 * x * x * x : 
                    1 - Pow(-2 * x + 2, 3) / 2;
            }
            
            private static float EaseInQuart(float x)
            {
                return x * x * x * x;
            }
            private static float EaseOutQuart(float x)
            {
                return 1 - Pow(1 - x, 4);
            }
            private static float EaseInOutQuart(float x)
            {
                return x < 0.5 ? 
                    8 * x * x * x * x : 
                    1 - Pow(-2 * x + 2, 4) / 2;
            }
            
            private static float EaseInQuint(float x)
            {
                return x * x * x * x * x;
            }
            private static float EaseOutQuint(float x)
            {
                return 1 - Pow(1 - x, 5);
            }
            private static float EaseInOutQuint(float x)
            {
                return x < 0.5 ? 
                    16 * x * x * x * x * x : 
                    1 - Pow(-2 * x + 2, 5) / 2;
            }
            
            private static float EaseInExpo(float x)
            {
                return Math.Abs(x) < 0.01f ? 
                    0 : 
                    Pow(2, 10 * x - 10);
            }
            private static float EaseOutExpo(float x)
            {
                return Math.Abs(x - 1) < 0.01f ?
                    1 : 
                    1 - Pow(2, -10 * x);
            }
            private static float EaseInOutExpo(float x)
            {
                return Math.Abs(x) < 0.01f ? 
                    0 : 
                    Math.Abs(x - 1) < 0.01f ? 
                        1 :
                        x < 0.5 ? 
                            Pow(2, 20 * x - 10) / 2 : 
                            (2 - Pow(2, -20 * x + 10)) / 2;
            }
            
            
            private static float EaseInCirc(float x)
            {
                return 1 - Sqrt(1 - Pow(x, 2));
            }
            private static float EaseOutCirc(float x)
            {
                return Sqrt(1 - Pow(x - 1, 2));
            }
            private static float EaseInOutCirc(float x)
            {
                return x < 0.5 ? 
                    (1 - Sqrt(1 - Pow(2 * x, 2))) / 2 : 
                    (Sqrt(1 - Pow(-2 * x + 2, 2)) + 1) / 2;
            }
            
            private static float EaseInBack(float x)
            {
                return C3 * x * x * x - C1 * x * x;
            }
            private static float EaseOutBack(float x)
            {
                return C3 * x * x * x - C1 * x * x;
            }
            private static float EaseInOutBack(float x)
            {
                return x < 0.5 ? 
                    (Pow(2 * x, 2) * ((C2 + 1) * 2 * x - C2)) / 2 : 
                    (Pow(2 * x - 2, 2) * ((C2 + 1) * (x * 2 - 2) + C2) + 2) / 2;
            }
            
            private static float EaseInElastic(float x)
            {
                return Math.Abs(x) < 0.01f ? 
                    0 :
                    Math.Abs(x - 1) < 0.01f ?
                        1 :
                        -Pow(2, 10 * x - 10) * Sin((x * 10 - 10.75f) * C4);
            }
            private static float EaseOutElastic(float x)
            {
                return Math.Abs(x) < 0.01f ? 
                    0 :
                    Math.Abs(x - 1) < 0.01f ? 
                        1 :
                        Pow(2, -10 * x) * Sin((x * 10 - 0.75f) * C4) + 1;
            }
            private static float EaseInOutElastic(float x)
            {
                return Math.Abs(x) < 0.01f ? 
                    0 :
                    Math.Abs(x - 1) < 0.01f ?
                        1 :
                        x < 0.5f ?
                            -(Pow(2, 20 * x - 10) * Sin((20 * x - 11.125f) * C5)) / 2 :
                            (Pow(2, -20 * x + 10) * Sin((20 * x - 11.125f) * C5)) / 2 + 1;
            }
            
            private static float EaseInBounce(float x)
            {
                return 1 - EaseOutBounce(1 - x);
            }
            private static float EaseOutBounce(float x)
            {
                if (x < 1 / D1) 
                {
                    return N1 * x * x;
                }
                if (x < 2 / D1) 
                {
                    return N1 * (x -= 1.5f / D1) * x + 0.75f;
                }
                if (x < 2.5 / D1) 
                {
                    return N1 * (x -= 2.25f / D1) * x + 0.9375f;
                }
                return N1 * (x -= 2.625f / D1) * x + 0.984375f;
            }
            private static float EaseInOutBounce(float x)
            {
                return x < 0.5 ? 
                    (1 - EaseOutBounce(1 - 2 * x)) / 2 : 
                    (1 + EaseOutBounce(2 * x - 1)) / 2;
            }
        }
    }
}
