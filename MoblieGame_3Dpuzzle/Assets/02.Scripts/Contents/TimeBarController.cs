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
}
