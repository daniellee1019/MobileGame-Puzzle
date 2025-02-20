using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class VillageCamController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCam;
    [SerializeField] private float perspectiveZoomSpeed = 0.5f;
    [SerializeField] private float orthoZoomSpeed = 0.5f;
    [SerializeField] private float moveSpeed = 0.005f;
    [SerializeField] private float smoothTime = 0.3f;
    [SerializeField] private float zoomDuration = 0.2f;

    private Tween moveTween;
    private Tween zoomTween;
    private bool isZooming = false;
    private bool isMoving = false;

    // 카메라 이동 제한 값
    private readonly float minX = -45f;
    private readonly float maxX = 45f;
    private readonly float minZ = -130f;
    private readonly float maxZ = -50f;

    private void Start()
    {
        if (virtualCam == null)
        {
            Debug.LogError("[VillageCamController] Cinemachine Virtual Camera가 할당되지 않았습니다.");
        }
    }

    private void Update()
    {
        HandleTouchPan();
        HandleTouchZoom();
    }

    /// <summary>
    /// 터치 드래그 이동 (한 손가락) - 터치가 없으면 즉시 멈추고, 이동 범위 제한 적용
    /// </summary>
    private void HandleTouchPan()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 touchDelta = touch.deltaPosition;

                // Transform을 직접 이동하여 Cinemachine의 Follow 없이 이동 가능하도록 설정
                Vector3 moveDirection = (-touchDelta.x * moveSpeed * Vector3.right) +
                                        (-touchDelta.y * moveSpeed * Vector3.forward);

                // 기존 이동 애니메이션이 있다면 취소
                moveTween?.Kill();
                isMoving = true;

                // 목표 위치 계산
                Vector3 targetPosition = transform.position + moveDirection;

                // 이동 범위 제한 적용
                targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
                targetPosition.z = Mathf.Clamp(targetPosition.z, minZ, maxZ);

                // DOTween으로 부드러운 이동 (현재 위치에서 이동)
                moveTween = transform.DOMove(targetPosition, smoothTime)
                                     .SetEase(Ease.OutQuad)
                                     .OnComplete(() => isMoving = false);
            }
        }

        // 터치가 없을 경우 이동을 즉시 멈춤
        if (Input.touchCount == 0 && isMoving)
        {
            moveTween?.Kill();
            isMoving = false;
        }
    }

    /// <summary>
    /// 터치 핀치 줌 (두 손가락) - 터치가 없으면 줌을 즉시 멈춤
    /// </summary>
    private void HandleTouchZoom()
    {
        if (virtualCam == null) return;

        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // 줌 동작 중 플래그 활성화
            isZooming = true;

            // 기존 줌 애니메이션이 있다면 취소
            zoomTween?.Kill();

            if (virtualCam.m_Lens.Orthographic)
            {
                float newSize = Mathf.Clamp(virtualCam.m_Lens.OrthographicSize + deltaMagnitudeDiff * orthoZoomSpeed, 1f, 20f);
                zoomTween = DOTween.To(() => virtualCam.m_Lens.OrthographicSize, x => virtualCam.m_Lens.OrthographicSize = x, newSize, zoomDuration)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() => isZooming = false);
            }
            else
            {
                float newFOV = Mathf.Clamp(virtualCam.m_Lens.FieldOfView + deltaMagnitudeDiff * perspectiveZoomSpeed, 10f, 90f);
                zoomTween = DOTween.To(() => virtualCam.m_Lens.FieldOfView, x => virtualCam.m_Lens.FieldOfView = x, newFOV, zoomDuration)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() => isZooming = false);
            }
        }

        // 터치가 없을 경우 줌을 멈춤
        if (Input.touchCount == 0 && isZooming)
        {
            zoomTween?.Kill();
            isZooming = false;
        }

#if UNITY_EDITOR
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            isZooming = true;
            zoomTween?.Kill();

            if (virtualCam.m_Lens.Orthographic)
            {
                float newSize = Mathf.Clamp(virtualCam.m_Lens.OrthographicSize - scroll * orthoZoomSpeed * 10f, 1f, 20f);
                zoomTween = DOTween.To(() => virtualCam.m_Lens.OrthographicSize, x => virtualCam.m_Lens.OrthographicSize = x, newSize, zoomDuration)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() => isZooming = false);
            }
            else
            {
                float newFOV = Mathf.Clamp(virtualCam.m_Lens.FieldOfView - scroll * perspectiveZoomSpeed * 100f, 10f, 90f);
                zoomTween = DOTween.To(() => virtualCam.m_Lens.FieldOfView, x => virtualCam.m_Lens.FieldOfView = x, newFOV, zoomDuration)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() => isZooming = false);
            }
        }

        // 마우스 휠 줌이 끝나면 줌을 멈춤
        if (Mathf.Abs(scroll) < 0.01f && isZooming)
        {
            zoomTween?.Kill();
            isZooming = false;
        }
#endif
    }
}
