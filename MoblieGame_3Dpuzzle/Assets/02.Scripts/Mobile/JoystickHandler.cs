using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;

public class JoystickHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Joystick Settings")]
    [SerializeField] private RectTransform joystickBackground;
    [SerializeField] private RectTransform joystickKnob;
    [SerializeField] private float joystickRange = 100f;

    public Vector2 InputDirection { get; private set; } = Vector2.zero;
    private Vector2 originalBackgroundPosition;

    private bool hasTouchedScreen = false;

    private void Start()
    {
        InitializeJoystick();
    }

    /// <summary>
    /// 이 섹션은 조이스틱과 초기화 작업을 처리합니다.
    /// 
    /// 조이스틱의 초기 위치를 저장하고 기본 비활성화 상태를 설정합니다.
    /// </summary>
    #region Initialization
    private void InitializeJoystick()
    {
        originalBackgroundPosition = joystickBackground.anchoredPosition; // 조이스틱 초기 위치 저장
        SetJoystickActive(false); // 조이스틱 비활성화
    }
    #endregion

    /// <summary>
    /// 이 섹션은 조이스틱의 터치, 드래그, 해제를 처리합니다.
    /// 
    /// 사용자의 입력에 따라 조이스틱 위치와 방향을 업데이트하며,
    /// 처음 터치 시 UI 비활성화 및 타임라인 종료를 처리합니다.
    /// </summary>
    #region Event Handlers
    public void OnPointerDown(PointerEventData eventData)
    {

        SetJoystickActive(true);

        if (!hasTouchedScreen) // 처음 터치 여부 확인
        {
            hasTouchedScreen = true;
        }

        MoveJoystickToTouch(eventData.position); // 터치 위치로 조이스틱 이동
        UpdateJoystickPosition(eventData.position); // 입력 방향 업데이트
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateJoystickPosition(eventData.position); // 드래그 위치 업데이트
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ResetJoystick(); // 조이스틱 초기화
    }
    #endregion

    /// <summary>
    /// 이 섹션은 조이스틱의 움직임 및 초기화를 처리합니다.
    /// 
    /// 조이스틱의 위치, 입력 방향, 활성화/비활성화를 제어합니다.
    /// </summary>
    #region Joystick Logic
    private void UpdateJoystickPosition(Vector2 position)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground, position, null, out Vector2 localPoint);

        InputDirection = Vector2.ClampMagnitude(localPoint / joystickRange, 1.0f); // 방향 계산
        joystickKnob.anchoredPosition = InputDirection * joystickRange; // 조이스틱 위치 설정
    }

    private void MoveJoystickToTouch(Vector2 position)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground.parent as RectTransform, position, null, out Vector2 localPoint);

        joystickBackground.anchoredPosition = localPoint; // 배경 위치 이동
    }

    private void ResetJoystick()
    {
        InputDirection = Vector2.zero; // 입력 초기화
        joystickKnob.anchoredPosition = Vector2.zero; // 조이스틱 버튼 초기화
        joystickBackground.anchoredPosition = originalBackgroundPosition; // 배경 위치 초기화
        SetJoystickActive(false); // 조이스틱 비활성화
    }

    private void SetJoystickActive(bool isActive)
    {
        joystickBackground.gameObject.SetActive(isActive); // 조이스틱 활성화/비활성화
    }
    #endregion
}
