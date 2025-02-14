using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;

public class TimeBarController : MonoBehaviour
{
    [Header("UI References")]
    // 좌우로 늘어선 타임바의 Image (Fill 방식)
    public Image timeBarFill;
    // 경고 알림 패널 (보스 웨이브 시작 예고)
    public GameObject warningPanel;
    // 현재 시간을 텍스트로 표시 (선택사항)
    public TextMeshProUGUI timeText;

    [Header("UI Slide Settings")]
    // 시간바 UI 패널 (예: Canvas의 자식으로 배치된 RectTransform)
    public RectTransform timeBar;
    // 토글 버튼 (시간바가 숨겨졌을 때 좌측에 표시될 버튼)
    public GameObject toggleButton;

    [Header("Animation Settings")]
    public float slideDuration = 0.5f;  // 슬라이드 애니메이션 지속 시간
    public float shownX = 0f;           // 시간바가 보일 때의 anchoredPosition.x (예: 0)
    public float hiddenX = -300f;       // 시간바가 숨겨질 때의 anchoredPosition.x (UI 디자인에 따라 조정)

    // 현재 시간바가 보이는지 여부
    private bool isShown = true;
    // 경고를 한 번만 표시하기 위한 플래그
    private bool warningShown = false;

    private void Update()
    {
        if (TimeManager.Instance == null)
            return;

        // 전체 하루 시간: dayDuration (예: 720초)
        // 현재 진행 시간: currentDayTime (0 ~ 720)
        float fill = TimeManager.Instance.currentDayTime / TimeManager.Instance.dayDuration;
        timeBarFill.fillAmount = fill;

        // 선택사항: 현재 시간을 텍스트로 갱신 (예: "HH:MM" 형식)
        if (timeText != null)
        {
            timeText.text = TimeManager.Instance.GetFormattedTime();
        }

        // 중앙 원(12시 위치)은 전체 시간의 50% 지점 (720 * 0.5 = 360초)
        // 현재 시간이 360초 이상이고 아직 경고를 표시하지 않았다면 경고 알림 활성화
        if (TimeManager.Instance.currentDayTime >= 360f && !warningShown)
        {
            ShowWarning();
        }

        /*
        // 터치 입력 감지: 시간바 UI 위에서 터치 후 왼쪽으로 스와이프하면 숨기기 처리
        if (Input.touchCount > 0)
        {
            Debug.Log("Touched");
            Touch touch = Input.GetTouch(0);
            // 터치가 끝날 때(또는 끝나기 직전에) 스와이프 방향 판단
            if (touch.phase == TouchPhase.Ended)
            {
                // x축 스와이프 거리(음수이면 왼쪽 스와이프)
                if (IsTouchOverSpecificUI("timeBar"))
                {
                    // 시간바가 보이는 상태라면 슬라이드하여 숨기기
                    if (isShown)
                    {
                        ToggleTimeBar(false);
                    }
                }
            }
        }
        */
    }

    /// <summary>
    /// 경고 알림을 활성화하고, 일정 시간 후에 자동으로 숨깁니다.
    /// </summary>
    private void ShowWarning()
    {
        warningShown = true;
        if (warningPanel != null)
        {
            warningPanel.SetActive(true);
            // 예: 3초 후 경고를 숨김
            Invoke("HideWarning", 3f);
        }
    }

    /// <summary>
    /// 경고 알림을 숨깁니다.
    /// </summary>
    private void HideWarning()
    {
        if (warningPanel != null)
        {
            warningPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 외부에서 타임바 관련 경고 플래그를 리셋할 필요가 있을 경우 호출합니다.
    /// 예를 들어, 새로운 Day가 시작될 때.
    /// </summary>
    public void ResetWarning()
    {
        warningShown = false;
        if (warningPanel != null)
        {
            warningPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 시간바 UI의 상태를 토글합니다.
    /// </summary>
    /// <param name="show">true면 보이고, false면 숨깁니다.</param>
    public void ToggleTimeBar(bool show)
    {
        isShown = show;
        if (show)
        {
            // 보일 때: anchoredPosition.x를 shownX로 슬라이드 애니메이션
            timeBar.DOAnchorPosX(shownX, slideDuration).SetEase(Ease.OutCubic);
            // 토글 버튼은 숨김
            toggleButton.SetActive(false);
        }
        else
        {
            // 숨길 때: anchoredPosition.x를 hiddenX로 슬라이드 애니메이션
            timeBar.DOAnchorPosX(hiddenX, slideDuration).SetEase(Ease.OutCubic);
            // 슬라이드 애니메이션 중 바로 토글 버튼 활성화 (원하는 경우 약간의 딜레이 후 활성화 가능)
            toggleButton.SetActive(true);
        }
    }

    /// <summary>
    /// 토글 버튼 클릭 이벤트 핸들러.
    /// </summary>
    public void OnToggleButtonPressed()
    {
        ToggleTimeBar(true);
    }

    /// <summary>
    /// 입력 터치가 특정 UI 요소 위에서 발생했는지 확인합니다.
    /// </summary>
    /// <param name="uiElementName">확인할 UI 요소의 이름</param>
    /// <returns>해당 UI 요소 위에서 터치가 발생하면 true 반환</returns>
    private bool IsTouchOverSpecificUI(string uiElementName)
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = touch.position
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (var result in results)
            {
                if (result.gameObject.name == uiElementName)
                {
                    return true;
                }
            }
        }
        return false;
    }

}
