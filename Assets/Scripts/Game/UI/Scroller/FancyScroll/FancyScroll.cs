using System;
using System.Linq;
using Game.UI.Scroller.Core;
using Game.Util.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI.Scroller.FancyScroll
{
    public class FancyScroll<TContent, TContentData> : Scroller<TContent, TContentData>, IDragHandler, IScrollHandler
        where TContent : ScrollContent<TContentData>
        where TContentData : ScrollContent.ScrollContentData
    {
        [SerializeField]
        protected float _dragSpeed = 0.5f;
        [SerializeField]
        protected float _maxScrollVelocity = 10f;
        protected float ScrollVelocity = 0;
        [SerializeField]
        protected float ScrollOffset = 0;
        protected TContent RedundantData { get; set; }

        protected int ShowingContentCount =>
            _contentCount - 1;

        protected float DragSpeedPerFixedDeltaTime
        {
            get
            {
                return Time.fixedDeltaTime * _dragSpeed;
            }
        }

        #region Mono
        
        public void OnScroll(PointerEventData eventData)
        {
            ScrollVelocity = 0;
            var scrollDelta = eventData.scrollDelta;
            scrollDelta.y = -scrollDelta.y;
            
            UpdateContentPosition(scrollDelta * _scrollSpeed);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            
            var scrollDelta = eventData.delta;

            ScrollVelocity += scrollDelta.y * _dragSpeed;
            ScrollVelocity = Mathf.Clamp(ScrollVelocity, -_maxScrollVelocity, _maxScrollVelocity);
            ScrollVelocity = Mathf.Clamp(ScrollVelocity, -_contentSize.y, _contentSize.y);
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if(_contentsList.Count > 0)
                RedundantData = _contentsList.Last();
            
            if(RedundantData != null)
                RedundantData.gameObject.SetActive(true);
            
            ((RectTransform) _contents.transform).SetAnchor(AnchorPreset.Middle, AnchorPreset.Top, (0.5f, 1));
        }

        private void Awake()
        {
            OnCurrentContentChange = ChangeContentsData;
        }

        private void FixedUpdate()
        {
            if (Mathf.Abs(ScrollVelocity) > 0.01f)
            {
                UpdateContentPosition((0f, ScrollVelocity).ToVector2());

                ScrollVelocity *= 0.8f;
            }
        }
        
        #endregion

        public override void InitiateContents()
        {
            base.InitiateContents();

            if (_contentsList.Count > 0)
            {
                RedundantData = _contentsList.GetIndex(-1);
                RedundantData.gameObject.SetActive(true);
            }
        }

        private void UpdateContentPosition(Vector2 scrollDelta)
        {
            var toContentPosition = (0f,ScrollOffset).ToVector2() + scrollDelta;
            
            var firstContent = _contentsList.GetIndex(0);
            var lastContent = _contentsList.GetIndex(-1);
            var gainCurrentContent = 0;

            var debugCode = 0;
            
            if (_loop)
            {
                if (lastContent.Equals(RedundantData) && 
                    scrollDelta.y < 0 && 
                    toContentPosition.y < 0)
                {
                    debugCode = 1;
                    _contentsList.Remove(lastContent);
                    _contentsList.Insert(0, lastContent);
                }
                else if (firstContent.Equals(RedundantData) &&
                         scrollDelta.y > 0 &&
                         toContentPosition.y > 0)
                {
                    debugCode = 2;
                    _contentsList.Remove(firstContent);
                    _contentsList.Add(firstContent);
                }
                
                ScrollOffset += scrollDelta.y;
                if (ScrollOffset <= -_contentSize.y)
                {
                    CurrentContent--;
                    ScrollOffset += _contentSize.y;
                }
                if (ScrollOffset >= _contentSize.y)
                {
                    CurrentContent++;
                    ScrollOffset -= _contentSize.y;
                }
            }
            else
            {
                if (CurrentContent + ShowingContentCount < ContentDatas.Count &&
                    CurrentContent != 0 &&
                    lastContent.Equals(RedundantData) &&
                    scrollDelta.y < 0 &&
                    toContentPosition.y < 0)
                {
                    debugCode = 3;
                    _contentsList.Remove(lastContent);
                    _contentsList.Insert(0, lastContent);
                }
                else if (CurrentContent > 0 &&
                         CurrentContent + ShowingContentCount != ContentDatas.Count && 
                         firstContent.Equals(RedundantData) &&
                         scrollDelta.y > 0 &&
                         toContentPosition.y > 0)
                {
                    debugCode = 4;
                    _contentsList.Remove(firstContent);
                    _contentsList.Add(firstContent);
                }


                if ((0 < CurrentContent || (CurrentContent == 0 && scrollDelta.y > 0)) &&
                    (CurrentContent < ContentDatas.Count - ShowingContentCount - 1 ||
                     (CurrentContent == ContentDatas.Count - ShowingContentCount - 1 && scrollDelta.y < 0)))
                {
                    ScrollOffset += scrollDelta.y;
                    if (ScrollOffset <= -_contentSize.y)
                    {
                        gainCurrentContent--;
                        ScrollOffset += _contentSize.y;

                        if (CurrentContent + gainCurrentContent == 0)
                            ScrollOffset = 0;
                    }

                    if (ScrollOffset >= _contentSize.y)
                    {
                        gainCurrentContent++;
                        ScrollOffset -= _contentSize.y;
                        
                        if (CurrentContent + gainCurrentContent == ContentDatas.Count - ShowingContentCount - 1)
                            ScrollOffset = 0;
                    }
                }
            }

            for (var i = 0; i < _contentsList.Count; i++)
            {
                var content = _contentsList[i];
                
                content.SetContentData(ContentDatas.GetIndex(_currentContent + i + (ScrollOffset < 0 ? -1 : 0)));
                content.RectTransform.anchoredPosition = CalculatePosition(i, true) +
                                                         (0f,
                                                             ScrollOffset < 0
                                                                 ? ScrollOffset + _contentSize.y
                                                                 : ScrollOffset).ToVector2();
            }

            if (gainCurrentContent != 0)
                CurrentContent += gainCurrentContent;
            
            Debug.Log(debugCode);
        }

        private void ChangeContentsData(int currentContent)
        {
            for (var i = 0; i < _contentsList.Count; i++)
            {
                var content = _contentsList[i];
                
                content.SetContentData(ContentDatas.GetIndex(currentContent + i));
                content.RectTransform.anchoredPosition = CalculatePosition(i, true) + (0f, ScrollOffset).ToVector2();
            }
        }
    }
}