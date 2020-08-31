using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.UI.Scroller.SlotTurnover;
using Game.Util;
using Game.Util.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Scroller.Core
{
    public class Scroller<TContent, TContentData> : MonoBehaviour
        where TContent : ScrollContent<TContentData>
        where TContentData : ScrollContent.ScrollContentData
    {

        #region variable
        
        [SerializeField] [Tooltip("중앙을 제외하고 양쪽에 보여야되는 콘탠츠 갯수")] [Range(0, 50)]
        protected int _contentCount = 1;

        protected int TotalContentCount => _contentCount;

        protected int CreateContentCount =>
            Mathf.Clamp(TotalContentCount + (_contentCount == 0 ? 0 : 1), 0, ContentDatas?.Count ?? 0);
        protected int MiddleContentIndex => _contentCount / 2;

        [SerializeField] [Tooltip("지금 콘텐츠 위치")]
        protected int _currentContent;

        protected Action<int> OnCurrentContentChange = null;
        protected int CurrentContent
        {
            get => _currentContent;
            set
            {
                OnCurrentContentChange?.Invoke(value);
                
                _currentContent = value;
            }
        }

        protected int CurrentContentForCalculate =>
            _currentContent < MiddleContentIndex ? _currentContent :
            _currentContent < ContentDatas.Count - MiddleContentIndex ? MiddleContentIndex :
            _currentContent - (ContentDatas.Count - MiddleContentIndex * 2 - 1);

        protected List<TContentData> ContentDatas;

        [SerializeField] [Tooltip("컨텐츠 디자인 프리펩")]
        protected TContent _contentPrefab = null;
        
        [SerializeField] [Tooltip("컨텐츠 표시 사이즈.")]
        protected Vector2 _contentSize;

        public Button.ButtonClickedEvent _buttonsEvents;

        [SerializeField] [Tooltip("넘어갈 시 움직이는 속도")]
        protected float _scrollSpeed = 2.0f;

        [SerializeField] [Tooltip("내용들이 루프를 할지 말지 결정.")]
        protected bool _loop = false;

        protected float ScrollSpeedPerFixedDeltaTime
        {
            get
            {
                return Time.fixedDeltaTime * _scrollSpeed;
            }
        }

        public List<TContent> _contentsList;
        
        public GameObject _contents;
        
        #endregion

        #region static Func

        protected static IEnumerator Destroy(GameObject obj)
        {
            yield return YieldManager.GetWaitForEndOfFrame();

            DestroyImmediate(obj);
        }

        protected static void InitializeRectTransform(Transform trans, Vector2 deltaSize, Vector2 position, Vector2 scale,
            Button.ButtonClickedEvent buttonClickedEvent)
        {
            var rectTransform = (RectTransform) trans;
            
            rectTransform.anchorMax = new Vector2(0.5f, 1);
            rectTransform.anchorMin = new Vector2(0.5f, 1);
            rectTransform.sizeDelta = deltaSize;
            rectTransform.anchoredPosition = position;
            rectTransform.localScale = scale;

            trans.GetComponent<Button>().onClick = buttonClickedEvent;
        }

        #endregion

        #region Initialize

        public virtual void InitiateContents()
        {
            _currentContent = _loop ? MiddleContentIndex : 0;

            if (CreateContentCount < _contentsList.Count)
            {
                for (var i = _contentsList.Count; i > CreateContentCount; i--)
                {
                    StartCoroutine(Destroy(_contentsList[i - 1].gameObject));
                    _contentsList.RemoveAt(i - 1);
                }

                foreach (var obj in _contentsList)
                {
                    obj.gameObject.SetActive(true);
                }
            }
            else if (CreateContentCount >= _contentsList.Count)
            {
                for (var i = 0; i < CreateContentCount; i++)
                {
                    TContent obj;
                    if (_contentsList.Count <= i)
                    {
                        if (_contentPrefab == null)
                        {
                            obj = new GameObject("content", typeof(Image), typeof(Button), typeof(TContent))
                                .GetComponent<TContent>();

                            _contentsList.Add(obj);
                        }
                        else
                        {
                            obj = Instantiate(_contentPrefab, _contents.transform, false);

                            _contentsList.Add(obj);
                        }
                    }
                    else
                    {
                        obj = _contentsList[i];
                    }

                    InitializeRectTransform(obj.transform, _contentSize, CalculatePosition(i), CalculateSize(i),
                        _buttonsEvents);
                    _contentsList[i].gameObject.SetActive(true);
                }
            }

            if (_contentsList.Count == 0) return;
            _contentsList.Last().gameObject.SetActive(false);

            if (_loop)
            {
                for (var i = -MiddleContentIndex; i <= MiddleContentIndex; i++)
                {
                    _contentsList.GetIndex(i + MiddleContentIndex).SetContentData(ContentDatas.GetIndex(i));
                }
            }
            else
            {
                for (var i = 0; i < _contentCount; i++)
                {
                    _contentsList[i].SetContentData(ContentDatas[i]);
                }
            }
        }

        #endregion

        #region Calculate Position & Size

        protected virtual Vector2 CalculatePosition(int index, bool indexIsRelative = false)
        {
            return VerticalCalculatePosition(index, indexIsRelative);
        }

        protected virtual Vector2 CalculateSize(int index, bool indexIsRelative = false)
        {
            return VerticalCalculateSize(index, indexIsRelative);
        }
        
        protected virtual Vector2 VerticalCalculatePosition(int index, bool indexIsRelative = false)
        {
            var returnPos = Vector2.zero;
            returnPos.y = -1 * _contentSize.y / 2;
            var relativeIndex = indexIsRelative ? index : CurrentContentForCalculate - index;
            if (relativeIndex == 0)
            {
                return returnPos;
            }

            returnPos.y = relativeIndex * _contentSize.y - _contentSize.y / 2;
            
            return returnPos;
        }

        protected virtual Vector2 VerticalCalculateSize(int index, bool indexIsRelative = false)
        {
            return Vector2.one;
        }

        #endregion

        #region Mono

        protected virtual void OnValidate()
        {
            if (_contents == null)
            {
                if (0 < transform.childCount)
                {
                    if (transform.GetChild(0).name == "Contents")
                    {
                        _contents = transform.GetChild(0).gameObject;
                    }
                }
                else
                {
                    _contents = new GameObject("Contents", typeof(Mask), typeof(Image));
                    _contents.transform.SetParent(transform);
                    ((RectTransform) _contents.transform).sizeDelta = ((RectTransform) transform).sizeDelta;
                    ((RectTransform) _contents.transform).localPosition = Vector3.zero;
                    ((RectTransform) _contents.transform).localScale = Vector2.one;
                    _contents.GetComponent<Image>().color = new Color(1, 1, 1, 0.01f);
                }
            }

            if (_contentsList == null)
            {
                _contentsList = new List<TContent>();
            }

            _currentContent = Mathf.Clamp(_currentContent, 0, ContentDatas?.Count ?? 0);
            _contentCount += _contentCount != 0 && _contentCount % 2 == 0 ? 1 : 0;

            InitiateContents();
            _currentContent = 0;
        }

        #endregion
    }
}