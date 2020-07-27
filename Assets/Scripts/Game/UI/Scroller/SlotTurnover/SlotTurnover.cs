using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Game.Util;
using Game.Util.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Scroller.SlotTurnover
{
    public class SlotTurnover<TContent, TContentData> : MonoBehaviour where TContent : SlotTurnoverContent<TContentData>
        where TContentData : SlotTurnoverContent.SlotTurnoverContentData
    {

        #region enum & structure

        public enum ScrollDirection
        {
            Horizontal,
            Vertical
        }

        #endregion

        #region variable

        [SerializeField] [Tooltip("아이템들 움직이는 방향")]
        protected ScrollDirection Direction = ScrollDirection.Horizontal;

        [SerializeField] [Tooltip("중앙을 제외하고 양쪽에 보여야되는 콘탠츠 갯수")] [Range(0, 50)]
        protected int ContentCount = 1;

        protected int TotalContentCount => ContentCount;

        protected int CreateContentCount =>
            Mathf.Clamp(TotalContentCount + (ContentCount == 0 ? 0 : 1), 0, ContentDatas?.Count ?? 0);
        protected int MiddleContentIndex => ContentCount / 2;

        [SerializeField] [Tooltip("지금 콘텐츠 위치")]
        protected int CurrentContent;

        protected int CurrentContentForCalculate =>
            CurrentContent < MiddleContentIndex ? CurrentContent :
            CurrentContent < ContentDatas.Count - MiddleContentIndex ? MiddleContentIndex :
            CurrentContent - (ContentDatas.Count - MiddleContentIndex * 2 - 1);

        protected List<TContentData> ContentDatas;

        [SerializeField] [Tooltip("컨텐츠 디자인 프리펩")]
        protected TContent ContentPrefab = null;

        [SerializeField] [Tooltip("넘어가는 버튼의 크기")]
        protected Vector2 ButtonSize = Vector2.one * 100;

        [SerializeField] [Tooltip("중앙 콘탠츠 크기")]
        protected Vector2 centerContentSize = Vector2.up * 160 + Vector2.right * 90;

        [SerializeField] [Tooltip("중앙 콘탠츠 양 옆에 표시하는 콘텐츠 크기")]
        protected Vector2 sideContentSize = Vector2.zero;

        [SerializeField] [Tooltip("중앙 콘텐츠 와 옆의 콘텐츠와의 떨어진 거리")]
        protected float Merge = 20.0f;

        [SerializeField] [Tooltip("넘어갈 시 움직이는 속도")]
        protected float ScrollSpeed = 2.0f;

        [SerializeField] [Tooltip("내용들이 루프를 할지 말지 결정.")]
        protected bool loop = false;

        protected float ScrollSpeedPerFixedDeltaTime
        {
            get
            {
                return Time.fixedDeltaTime * ScrollSpeed;
            }
        }

        public List<TContent> ContentsList;

        public Button.ButtonClickedEvent ButtonsEvents;

        public Button NextButton;

        public Button PreviousButton;

        public GameObject Contents;

        private Coroutine action;

        #endregion

        #region Static Func

        private static IEnumerator Destroy(GameObject obj)
        {
            yield return YieldManager.GetWaitForEndOfFrame();

            DestroyImmediate(obj);
        }

        private static void InitializeRectTransform(Transform trans, Vector2 deltaSize, Vector2 position, Vector2 scale,
            Button.ButtonClickedEvent buttonClickedEvent)
        {
            var rectTransform = (RectTransform) trans;
            
            rectTransform.sizeDelta = deltaSize;
            rectTransform.localPosition = position;
            rectTransform.localScale = scale;

            trans.GetComponent<Button>().onClick = buttonClickedEvent;
        }

        #endregion

        #region Initiate

        public virtual void InitiateContents()
        {
            CurrentContent = loop ? MiddleContentIndex : 0;

            if (CreateContentCount < ContentsList.Count)
            {
                for (var i = ContentsList.Count; i > CreateContentCount; i--)
                {
                    StartCoroutine(Destroy(ContentsList[i - 1].gameObject));
                    ContentsList.RemoveAt(i - 1);
                }

                foreach (var obj in ContentsList)
                {
                    obj.gameObject.SetActive(true);
                }
            }
            else if (CreateContentCount >= ContentsList.Count)
            {
                for (var i = 0; i < CreateContentCount; i++)
                {
                    TContent obj;
                    if (ContentsList.Count <= i)
                    {
                        if (ContentPrefab == null)
                        {
                            obj = new GameObject("content", typeof(Image), typeof(Button), typeof(TContent))
                                .GetComponent<TContent>();

                            ContentsList.Add(obj);
                        }
                        else
                        {
                            obj = Instantiate(ContentPrefab, Contents.transform, false);

                            ContentsList.Add(obj);
                        }
                    }
                    else
                    {
                        obj = ContentsList[i];
                    }

                    InitializeRectTransform(obj.transform, centerContentSize, CalculatePosition(i), CalculateSize(i),
                        ButtonsEvents);
                    ContentsList[i].gameObject.SetActive(true);
                }
            }

            if (ContentsList.Count == 0) return;
            ContentsList.Last().gameObject.SetActive(false);

            if (loop)
            {
                for (var i = -MiddleContentIndex; i <= MiddleContentIndex; i++)
                {
                    ContentsList.GetIndex(i + MiddleContentIndex).SetContentData(ContentDatas.GetIndex(i));
                }
            }
            else
            {
                for (var i = 0; i < ContentCount; i++)
                {
                    ContentsList[i].SetContentData(ContentDatas[i]);
                }
            }
        }

        public virtual void InitiateButton()
        {
            if (NextButton == null)
            {
                if (transform.childCount - ContentsList.Count >= 1)
                {
                    if (transform.GetChild(1).name == "NextButton")
                    {
                        NextButton = transform.GetChild(1).GetComponent<Button>();
                    }
                }
                else
                {
                    var obj = new GameObject("NextButton", typeof(Button), typeof(Image));
                    var rectTransform = (RectTransform) obj.transform;
                    NextButton = obj.GetComponent<Button>();

                    rectTransform.SetParent(transform);
                    rectTransform.localScale = Vector3.one;
                    rectTransform.sizeDelta = ButtonSize;
                    rectTransform.localPosition = Vector2.right *
                                                  (((RectTransform) transform).sizeDelta.x * 0.5f + ButtonSize.x +
                                                   Merge);
                }
            }

            if (PreviousButton == null)
            {
                if (transform.childCount - ContentsList.Count >= 2)
                {
                    if (transform.GetChild(2).name == "PreviousButton")
                    {
                        PreviousButton = transform.GetChild(2).GetComponent<Button>();
                    }
                }
                else
                {
                    var obj = new GameObject("PreviousButton", typeof(Button), typeof(Image));
                    var rectTransform = (RectTransform) obj.transform;
                    PreviousButton = obj.GetComponent<Button>();

                    rectTransform.SetParent(transform);
                    rectTransform.localScale = Vector3.one;
                    rectTransform.sizeDelta = ButtonSize;
                    rectTransform.localPosition = Vector2.left *
                                                  (((RectTransform) transform).sizeDelta.x * 0.5f + ButtonSize.x +
                                                   Merge);
                }
            }
        }

        #endregion

        #region Calculate Position and Size

        #region int index

        protected Vector2 CalculatePosition(int index, bool indexIsRelative = false)
        {
            switch (Direction)
            {
                case ScrollDirection.Horizontal:
                {
                    return HorizontalCalculatePosition(index, indexIsRelative);
                }

                case ScrollDirection.Vertical:
                {
                    return VerticalCalculatePosition(index, indexIsRelative);
                }
            }

            return HorizontalCalculatePosition(index, indexIsRelative);
        }

        protected Vector2 CalculateSize(int index, bool indexIsRelative = false)
        {
            switch (Direction)
            {
                case ScrollDirection.Horizontal:
                {
                    return HorizontalCalculateSize(index, indexIsRelative);
                }

                case ScrollDirection.Vertical:
                {
                    return VerticalCalculateSize(index, indexIsRelative);
                }
            }

            return HorizontalCalculateSize(index, indexIsRelative);
        }

        protected Vector2 VerticalCalculatePosition(int index, bool indexIsRelative = false)
        {
            Vector2 returnPos = ((RectTransform) transform).anchoredPosition;
            var relativeIndex = indexIsRelative ? index : index - CurrentContentForCalculate;
            if (relativeIndex == 0)
            {
                return returnPos;
            }

            var currentSize = centerContentSize;
            var relativeScale = sideContentSize / centerContentSize;
            var mergeY = Merge;

            for (var i = 0; i < Mathf.Abs(relativeIndex); i++)
            {
                var nextSize = currentSize * relativeScale;
                returnPos += Vector2.up *
                             ((relativeIndex < 0 ? -1 : 1) * (currentSize.y * 0.5f + nextSize.y * 0.5f + mergeY));

                currentSize = nextSize;
                mergeY *= relativeScale.y;
            }

            return returnPos;
        }

        protected Vector2 VerticalCalculateSize(int index, bool indexIsRelative = false)
        {
            var returnSize = Vector2.one;
            var relativeIndex = indexIsRelative ? index : index - CurrentContentForCalculate;
            if (relativeIndex == 0)
            {
                return returnSize;
            }

            var relativeSize = sideContentSize / centerContentSize;

            for (var i = 0; i < Mathf.Abs(relativeIndex); i++)
            {
                returnSize *= relativeSize;
            }

            return returnSize;
        }

        protected Vector2 HorizontalCalculatePosition(int index, bool indexIsRelative = false)
        {
            var returnPos = ((RectTransform) transform).anchoredPosition;
            var relativeIndex = indexIsRelative ? index : index - CurrentContentForCalculate;
            if (relativeIndex == 0)
            {
                return returnPos;
            }

            var currentSize = centerContentSize;
            var relativeScale = sideContentSize / centerContentSize;
            var mergeX = Merge;

            for (var i = 0; i < Mathf.Abs(relativeIndex); i++)
            {
                var nextSize = currentSize * relativeScale;
                returnPos += Vector2.right * 
                             ((relativeIndex < 0 ? -1 : 1) * (currentSize.x * 0.5f + nextSize.x * 0.5f + mergeX));

                currentSize = nextSize;
                mergeX *= relativeScale.x;
            }

            return returnPos;
        }

        protected Vector2 HorizontalCalculateSize(int index, bool indexIsRelative = false)
        {
            var returnSize = Vector2.one;
            var relativeIndex = indexIsRelative ? index : index - CurrentContentForCalculate;
            if (relativeIndex == 0)
            {
                return returnSize;
            }

            var relativeSize = sideContentSize / centerContentSize;

            for (var i = 0; i < Mathf.Abs(relativeIndex); i++)
            {
                returnSize *= relativeSize;
            }

            return returnSize;
        }

        #endregion

        #endregion

        #region ButtonEvent

        public void NextContent()
        {
            if (!loop && action == null && CurrentContent + 1 < ContentDatas.Count)
            {
                if (CurrentContent + 1 > MiddleContentIndex && CurrentContent + 1 < ContentDatas.Count - MiddleContentIndex)
                {
                    action = StartCoroutine(MoveContentsLoop(1));
                }
                else
                {
                    action = StartCoroutine(MoveContents(CurrentContent + 1));
                }
            }
            else if (loop && action == null && (CurrentContent + 1) % TotalContentCount < TotalContentCount)
            {
                action = StartCoroutine(MoveContentsLoop(1));
            }
        }

        public void PreviousContent()
        {
            if (!loop && action == null && CurrentContent - 1 >= 0)
            {
                if (CurrentContent -1 > MiddleContentIndex - 1 && CurrentContent - 1 < ContentDatas.Count - MiddleContentIndex - 1)
                {
                    action = StartCoroutine(MoveContentsLoop(-1));
                }
                else
                {
                    action = StartCoroutine(MoveContents(CurrentContent - 1));
                }
            }
            else if (loop && action == null && (TotalContentCount + CurrentContent - 1) % TotalContentCount >= 0)
            {
                action = StartCoroutine(MoveContentsLoop(-1));
            }
        }

        #endregion

        #region MoveSystem
        
        protected IEnumerator MoveContents(int to)
        {
            if (!ContentsList.First().gameObject.activeSelf)
            {
                ContentsList.Add(ContentsList.First());
                ContentsList.RemoveAt(0);
            }
            var fromPosition = new List<Vector2>();
            var fromSizeDelta = new List<Vector2>();
            var dir = to - CurrentContent;

            for (var index = 0; index < ContentsList.Count; index++)
            {
                fromPosition.Add(CalculatePosition(index));
                fromSizeDelta.Add(CalculateSize(index));
            }

            CurrentContent = to;

            var toPosition = new List<Vector2>();
            var toSizeDelta = new List<Vector2>();

            for (var index = 0; index < ContentsList.Count; index++)
            {
                toPosition.Add(CalculatePosition(index));
                toSizeDelta.Add(CalculateSize(index));
            }

            for (var i = 0; i < 60 / ScrollSpeed; i++)
            {
                for (var index = 0; index < ContentsList.Count; index++)
                {
                    ((RectTransform) ContentsList[index].transform).localPosition = Vector3.Lerp(fromPosition[index],
                        toPosition[index], i * ScrollSpeedPerFixedDeltaTime);
                    ((RectTransform) ContentsList[index].transform).localScale = Vector3.Lerp(fromSizeDelta[index],
                        toSizeDelta[index], i * ScrollSpeedPerFixedDeltaTime);
                }

                yield return YieldManager.GetWaitForFixedUpdate();
            }

            action = null;
            ButtonActiveSet();
        }

        protected IEnumerator MoveContentsLoop(int dir)
        {
            var fromPosition = new List<Vector2>();
            var fromSizeDelta = new List<Vector2>();

            for (var index = -MiddleContentIndex - 1; index <= MiddleContentIndex + 1; index++)
            {
                fromPosition.Add(CalculatePosition(index, true));
                fromSizeDelta.Add(CalculateSize(index, true));
            }

            if (dir != 1)
            {
                fromPosition.RemoveAt(fromPosition.Count - 1);
                fromSizeDelta.RemoveAt(fromSizeDelta.Count - 1);
            }
            else
            {
                fromPosition.RemoveAt(0);
                fromSizeDelta.RemoveAt(0);
            }

            CurrentContent += dir;
            CurrentContent = (ContentDatas.Count + CurrentContent) % ContentDatas.Count;

            var toPosition = new List<Vector2>();
            var toSizeDelta = new List<Vector2>();

            for (var index = -MiddleContentIndex - 2; index <= MiddleContentIndex + 2; index++)
            {
                toPosition.Add(CalculatePosition(index + dir, true));
                toSizeDelta.Add(CalculateSize(index + dir, true));
            }

            if (dir != 1)
            {
                toPosition.RemoveRange(0, 3);
                toSizeDelta.RemoveRange(0, 3);

                if (!ContentsList.First().gameObject.activeSelf)
                {
                    var obj = ContentsList.First();
                    ContentsList.Remove(obj);
                    ContentsList.Add(obj);
                    obj.gameObject.SetActive(true);
                }
                else
                {
                    var obj = ContentsList.Last();
                    obj.gameObject.SetActive(true);
                }
            }
            else
            {
                toPosition.RemoveRange(toPosition.Count - 3, 3);
                toSizeDelta.RemoveRange(toSizeDelta.Count - 3, 3);

                if (!ContentsList.Last().gameObject.activeSelf)
                {
                    var obj = ContentsList.Last();
                    obj.gameObject.SetActive(true);
                }
                else
                {
                    var obj = ContentsList.First();
                    ContentsList.Remove(obj);
                    ContentsList.Add(obj);
                    obj.gameObject.SetActive(true);
                }
            }

            for (var i = -MiddleContentIndex + CurrentContent; i <= MiddleContentIndex + CurrentContent + 1; i++)
            {
                ContentsList.GetIndex(i + MiddleContentIndex - CurrentContent + (dir < 0 ? 0 : dir))
                    .SetContentData(ContentDatas.GetIndex(i));
            }
            ButtonActiveSet(MiddleContentIndex + (dir == 1 ? 0 : 1), true);

            for (var index = 0; index < ContentsList.Count; index++)
            {
                ((RectTransform) ContentsList[index].transform).localPosition = fromPosition[index];
                ((RectTransform) ContentsList[index].transform).localScale = fromSizeDelta[index];
            }

            yield return YieldManager.GetWaitForFixedUpdate();

            for (var i = 0; i < 60 / ScrollSpeed; i++)
            {
                for (var index = 0; index < ContentsList.Count; index++)
                {
                    ((RectTransform) ContentsList[index].transform).localPosition = Vector3.Lerp(fromPosition[index],
                        toPosition[index], i * ScrollSpeedPerFixedDeltaTime);
                    ((RectTransform) ContentsList[index].transform).localScale = Vector3.Lerp(fromSizeDelta[index],
                        toSizeDelta[index], i * ScrollSpeedPerFixedDeltaTime);
                }

                yield return YieldManager.GetWaitForFixedUpdate();
            }

            action = null;

            if (dir == 1)
            {
                ContentsList.First().gameObject.SetActive(false);
            }
            else
            {
                ContentsList.Last().gameObject.SetActive(false);
            }

            ButtonActiveSet(MiddleContentIndex + (dir == 1 ? 1 : 0));
        }
        
        #endregion

        #region ButtonInteractable
        
        protected void ButtonActiveSet()
        {
            for (var i = 0; i < ContentsList.Count; i++)
            {
                ContentsList[i].GetComponent<Button>().interactable = (CurrentContentForCalculate == i);
            }
        }

        protected void ButtonActiveSet(int index, bool instance = false)
        {
            for (var i = 0; i < ContentsList.Count; i++)
            {
                var button = ContentsList[i].GetComponent<Button>();
                button.interactable = (index == i);

                if (!instance) continue;

                var colors = button.colors;
                button.targetGraphic.CrossFadeColor(colors.disabledColor * colors.colorMultiplier, 0,
                    true, true);
            }
        }

        #endregion

        #region Mono

        private void OnValidate()
        {
            if (Contents == null)
            {
                if (0 < transform.childCount)
                {
                    if (transform.GetChild(0).name == "Contents")
                    {
                        Contents = transform.GetChild(0).gameObject;
                    }
                }
                else
                {

                    Contents = new GameObject("Contents", typeof(Mask), typeof(Image));
                    Contents.transform.SetParent(transform);
                    ((RectTransform) Contents.transform).sizeDelta = ((RectTransform) transform).sizeDelta;
                    ((RectTransform) Contents.transform).localPosition = Vector3.zero;
                    ((RectTransform) Contents.transform).localScale = Vector2.one;
                    Contents.GetComponent<Image>().color = new Color(1, 1, 1, 0.01f);
                }
            }

            if (ContentsList == null)
            {
                ContentsList = new List<TContent>();
            }

            CurrentContent = Mathf.Clamp(CurrentContent, 0, ContentDatas?.Count ?? 0);
            ContentCount += ContentCount != 0 && ContentCount % 2 == 0 ? 1 : 0;

            InitiateContents();
            InitiateButton();
            ButtonActiveSet();
            CurrentContent = 0;
        }

        #endregion

        public void SetData(List<TContentData> list)
        {
            ContentDatas = list;
            
            OnValidate();
        }
    }
}
