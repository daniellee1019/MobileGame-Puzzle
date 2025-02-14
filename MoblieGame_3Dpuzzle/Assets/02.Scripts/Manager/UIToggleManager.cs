using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using DG.Tweening;

public class UIToggleManager : MonoBehaviour
{
    public static UIToggleManager Instance;

    [System.Serializable]
    public class UIToggleItem
    {
        public string uiName; // UI의 고유 이름 (예: "TimeBarUI")
        public CanvasGroup uiCanvasGroup; // 주 UI의 CanvasGroup (애니메이션에 사용)
        public RectTransform uiRectTransform; // 주 UI의 RectTransform (슬라이드 애니메이션용)
        public Vector2 shownPosition; // 주 UI가 보일 때의 anchoredPosition
        public Vector2 hiddenPosition; // 주 UI가 숨길 때의 anchoredPosition
        public GameObject toggleButton; // UI가 숨겨졌을 때 나타나는 토글 버튼
    }

    [Header("Toggle Items")]
    public List<UIToggleItem> toggleItems = new List<UIToggleItem>();

    [Header("Animation Settings")]
    public float slideDuration = 0.5f; // 슬라이드 애니메이션 지속 시간
    public float fadeDuration = 0.5f;  // 페이드 애니메이션 지속 시간

    // 재사용 가능한 필드: 매 프레임마다 새로 할당하지 않도록 함
    private PointerEventData pointerEventData;
    private List<RaycastResult> raycastResults = new List<RaycastResult>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void HideUIElement(string uiName)
    {
        UIToggleItem item = toggleItems.Find(x => x.uiName == uiName);
        if (item != null && item.uiCanvasGroup.gameObject.activeSelf)
        {
            item.uiRectTransform.DOAnchorPos(item.hiddenPosition, slideDuration).SetEase(Ease.InCubic);
            item.uiCanvasGroup.DOFade(0f, fadeDuration).OnComplete(() =>
            {
                item.uiCanvasGroup.gameObject.SetActive(false);
                if (item.toggleButton != null)
                {
                    item.toggleButton.SetActive(true);
                }
            });
        }
    }

    public void ShowUIElement(string uiName)
    {
        UIToggleItem item = toggleItems.Find(x => x.uiName == uiName);
        if (item != null)
        {
            if (item.toggleButton != null)
            {
                item.toggleButton.SetActive(false);
            }
            item.uiCanvasGroup.gameObject.SetActive(true);
            item.uiRectTransform.anchoredPosition = item.hiddenPosition;
            item.uiCanvasGroup.alpha = 0f;
            item.uiRectTransform.DOAnchorPos(item.shownPosition, slideDuration).SetEase(Ease.OutCubic);
            item.uiCanvasGroup.DOFade(1f, fadeDuration);
        }
    }

    public void HideAllUI()
    {
        foreach (var item in toggleItems)
        {
            if (item.uiCanvasGroup.gameObject.activeSelf)
            {
                HideUIElement(item.uiName);
            }
        }
    }

    // Update에서 재사용 가능한 raycastResults와 pointerEventData를 이용하여, 터치된 UI에 따라 처리합니다.
    private void Update()
    {
        // 터치나 마우스 클릭 시 처리 (여기서는 터치 입력에 집중)
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // pointerEventData 재사용 (매번 새로 생성하지 않고, 재할당)
            if (pointerEventData == null)
                pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.GetTouch(0).position;

            // 재사용 가능한 리스트 초기화 후 RaycastAll 호출
            raycastResults.Clear();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            // 각 결과에 대해 스위치문을 사용해 처리합니다.
            foreach (var result in raycastResults)
            {
                switch (result.gameObject.name)
                {
                    case "timeBar":
                        HideUIElement("timeBar");
                        break;
                    case "currencyText":
                        HideUIElement("currencyText");
                        break;
                    case "timeText":
                        HideUIElement("timeText");
                        break;
                        // 필요시 다른 UI 이름도 추가
                }
            }
        }
    }
}
