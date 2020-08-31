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
        
        [SerializeField] [Tooltip("컨텐츠 표시 사이즈.")]
        protected Vector2 ContentSize;

        public Button.ButtonClickedEvent ButtonsEvents;

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
        
        public GameObject Contents;
        
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

                    InitializeRectTransform(obj.transform, ContentSize, CalculatePosition(i), CalculateSize(i),
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
            returnPos.y = -1 * ContentSize.y / 2;
            var relativeIndex = indexIsRelative ? index : CurrentContentForCalculate - index;
            if (relativeIndex == 0)
            {
                return returnPos;
            }

            returnPos.y = relativeIndex * ContentSize.y - ContentSize.y / 2;
            
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
            CurrentContent = 0;
        }

        #endregion
    }
}