using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;

public class UIToggleManager : MonoBehaviour
{
    public static UIToggleManager Instance;

    [Header("Cinemachine Settings")]
    public CinemachineVirtualCamera mainStageCam;
    public CinemachineVirtualCamera villageStageCam;

    [Header("Canvas Settings")]
    public Canvas joystickCanvas;

    [Header("Build Panel Settings")]
    public GameObject constructionPanel; // 건설 패널 (하단 UI)
    public GameObject productionPanel;   // 생산 패널
    public GameObject defensePanel;      // 방어 패널
    public GameObject otherPanel;   // 생산 패널

    [Header("Build Button Settings")]
    public GameObject constructionButton; // 건설 패널 (하단 UI)
    public GameObject productionButton;   // 생산 패널
    public GameObject defenseButton;      // 방어 패널
    public GameObject otherButton;   // 생산 패널

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
                    case "buildBar":
                        HideUIElement("buildBar");
                        break;
                        // 필요시 다른 UI 이름도 추가
                }
            }
        }
    }

    #region 개별 Ui 숨기기/보이기 기능
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

    public void ShowPanelElement(string uiName)
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

            // 패널이 위에서 아래로 내려오며 부드럽게 튕김 효과 적용
            item.uiRectTransform.DOAnchorPos(item.shownPosition, slideDuration).SetEase(Ease.OutBounce);
            item.uiCanvasGroup.DOFade(1f, fadeDuration);
        }
    }
    #endregion

    #region UI 전체 숨기기/보이기 기능
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
    public void ShowAllUI()
    {
        foreach (var item in toggleItems)
        {
            ShowUIElement(item.uiName); // 무조건 실행되도록 수정
        }
    }
    #endregion

    #region 가상 카메라 따른 UI 설정
    public void ChangeToMainCam()
    {
        joystickCanvas.gameObject.SetActive(true);

        // MainStageCam 활성화
        mainStageCam.Priority = 11;
        villageStageCam.Priority = 9;
    }

    public void ChangeToVillageCam()
    {
        joystickCanvas.gameObject.SetActive(false);

        // MainStageCam 활성화
        mainStageCam.Priority = 9;
        villageStageCam.Priority = 11;
    }
    #endregion

    public void ShowPanel(string category)
    {
        // 모든 패널을 숨김
        constructionPanel.SetActive(false);
        productionPanel.SetActive(false);
        defensePanel.SetActive(false);
        otherPanel.SetActive(false);

        // 모든 버튼을 숨김
        constructionButton.SetActive(false);
        productionButton.SetActive(false);
        defenseButton.SetActive(false);
        otherButton.SetActive(false);

        // 선택한 패널만 활성화
        switch (category)
        {
            case "Construction_PN":
                constructionPanel.SetActive(true);
                constructionButton.SetActive(true);
                break;
            case "Production_PN":
                productionPanel.SetActive(true);
                productionButton.SetActive(true);
                break;
            case "Defense_PN":
                defensePanel.SetActive(true);
                defenseButton.SetActive(true);
                break;
            case "Other_PN":
                otherPanel.SetActive(true);
                otherButton.SetActive(true);
                break;
        }
    }
}
