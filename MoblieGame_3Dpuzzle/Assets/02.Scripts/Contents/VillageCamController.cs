using UnityEngine;
using DG.Tweening;

public class VillageCamController : MonoBehaviour
{
    [SerializeField] private float perspectiveZoomSpeed = 0.5f;
    [SerializeField] private float orthoZoomSpeed = 0.5f;  // 줌 속도 조절
    [SerializeField] private float moveSpeed = 0.005f;
    [SerializeField] private float smoothTime = 0.3f; // 이동 애니메이션 지속 시간
    [SerializeField] private float zoomDuration = 0.2f; // 줌 애니메이션 지속 시간

    private Camera cam;
    private Tween moveTween;
    private Tween zoomTween;
    private Vector3 lastMousePosition;
    private bool isRightMouseHeld = false;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        HandleTouchPan();
        HandleTouchZoom();
        HandleMouseZoomWithRightClick();
    }

    /// <summary>
    /// 터치 드래그 이동 (한 손가락) - 화면 기준으로 수평 이동
    /// DOTween을 사용하여 자연스럽게 이동
    /// </summary>
    void HandleTouchPan()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 touchDelta = touch.deltaPosition;

                // 월드 좌표 기준으로 이동 방향 결정
                Vector3 moveDirection = (-touchDelta.x * moveSpeed * transform.right) +
                                        (-touchDelta.y * moveSpeed * Vector3.forward); // 화면 기준 수평 이동

                // 기존 이동 애니메이션이 있다면 취소
                moveTween?.Kill();

                // DOTween으로 부드러운 이동 (현재 위치에서 이동)
                moveTween = transform.DOMove(transform.position + moveDirection, smoothTime).SetEase(Ease.OutQuad);
            }
        }
    }

    /// <summary>
    /// 터치 핀치 줌 (두 손가락)
    /// DOTween으로 줌 부드럽게 애니메이션 적용
    /// </summary>
    void HandleTouchZoom()
    {
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // 기존 줌 애니메이션이 있다면 취소
            zoomTween?.Kill();

            if (cam.orthographic)
            {
                float newSize = Mathf.Clamp(cam.orthographicSize + deltaMagnitudeDiff * orthoZoomSpeed, 1f, 20f);
                zoomTween = DOTween.To(() => cam.orthographicSize, x => cam.orthographicSize = x, newSize, zoomDuration)
                    .SetEase(Ease.OutQuad);
            }
            else
            {
                float newFOV = Mathf.Clamp(cam.fieldOfView + deltaMagnitudeDiff * perspectiveZoomSpeed, 10f, 90f);
                zoomTween = DOTween.To(() => cam.fieldOfView, x => cam.fieldOfView = x, newFOV, zoomDuration)
                    .SetEase(Ease.OutQuad);
            }
        }
    }

    /// <summary>
    /// 마우스 우클릭 후 이동 시 줌 (휠 대신 사용)
    /// </summary>
    void HandleMouseZoomWithRightClick()
    {
        if (Input.GetMouseButtonDown(1)) // 우클릭 시작
        {
            isRightMouseHeld = true;
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(1)) // 우클릭 해제
        {
            isRightMouseHeld = false;
        }

        if (isRightMouseHeld)
        {
            Vector3 currentMousePosition = Input.mousePosition;
            float deltaX = currentMousePosition.x - lastMousePosition.x; // 좌우 이동량 계산
            lastMousePosition = currentMousePosition;

            if (Mathf.Abs(deltaX) > 1f) // 너무 작은 움직임 무시
            {
                // 기존 줌 애니메이션이 있다면 취소
                zoomTween?.Kill();

                float zoomAmount = deltaX * perspectiveZoomSpeed; // 이동량을 줌 값으로 변환

                if (cam.orthographic)
                {
                    float newSize = Mathf.Clamp(cam.orthographicSize - zoomAmount, 1f, 20f);
                    zoomTween = DOTween.To(() => cam.orthographicSize, x => cam.orthographicSize = x, newSize, zoomDuration)
                        .SetEase(Ease.OutQuad);
                }
                else
                {
                    float newFOV = Mathf.Clamp(cam.fieldOfView - zoomAmount, 10f, 90f);
                    zoomTween = DOTween.To(() => cam.fieldOfView, x => cam.fieldOfView = x, newFOV, zoomDuration)
                        .SetEase(Ease.OutQuad);
                }
            }
        }
    }
}
