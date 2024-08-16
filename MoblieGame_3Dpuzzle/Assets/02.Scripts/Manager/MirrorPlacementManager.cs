using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MirrorPlacementManager : MonoBehaviour
{
    public GameObject[] mirrorPrefabs; // 거울 프리팹 배열
    public LayerMask placementLayerMask; // 거울을 설치할 수 있는 레이어
    public Button[] mirrorButtons; // 버튼 배열 (ReflectingButton, RefractingButton, AmplifyingButton 등)
    public GameObject rotationGizmoPrefab; // 회전 기즈모 프리팹
    public PlayerMovement playerMovement; // 플레이어 컨트롤러 참조

    private GameObject currentMirrorPreview;
    private GameObject currentGizmo;
    private Camera mainCamera;
    private bool isPlacing = false;
    private bool isRotating = false;
    private bool isDragging = false;
    private Vector3 touchOffset;

    void Start()
    {
        mainCamera = Camera.main;

        // 각 버튼에 이벤트 리스너를 추가
        for (int i = 0; i < mirrorButtons.Length; i++)
        {
            int index = i; // 클로저 문제를 방지하기 위해 로컬 변수로 인덱스를 저장

            // Button 대신 EventTrigger를 사용하여 PointerDown, PointerUp 이벤트 처리
            EventTrigger trigger = mirrorButtons[i].gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
            pointerDownEntry.eventID = EventTriggerType.PointerDown;
            pointerDownEntry.callback.AddListener((data) => StartDraggingMirror(index));
            trigger.triggers.Add(pointerDownEntry);

            EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
            pointerUpEntry.eventID = EventTriggerType.PointerUp;
            pointerUpEntry.callback.AddListener((data) => StopDraggingMirror());
            trigger.triggers.Add(pointerUpEntry);
        }
    }

    void Update()
    {
        if (isDragging)
        {
            HandleMirrorDragging();
        }
        else if (isPlacing)
        {
            HandleMirrorPlacement();
        }
        else if (isRotating)
        {
            HandleMirrorRotation();
        }
    }

    private void StartDraggingMirror(int mirrorIndex)
    {
        if (currentMirrorPreview != null)
        {
            Destroy(currentMirrorPreview);
        }

        playerMovement.DisableMovement();

        currentMirrorPreview = Instantiate(mirrorPrefabs[mirrorIndex]);

        // 거울의 로테이션을 (0, 0, 0)으로 설정
        currentMirrorPreview.transform.rotation = Quaternion.identity;

        // 터치 위치로부터 Ray를 쏴서 터치 지점의 위치를 가져오기
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 touchPosition = hit.point;
            // 거울의 위치를 터치 위치의 X, Z 값과 Y축 4.5로 설정
            currentMirrorPreview.transform.position = new Vector3(touchPosition.x, 4.5f, touchPosition.z);
        }
        else
        {
            // 터치 위치가 설정되지 않은 경우의 기본 위치 처리 (예: 화면 중앙 또는 특정 위치)
            currentMirrorPreview.transform.position = new Vector3(0, 4.5f, 0);
        }

        currentMirrorPreview.GetComponent<Collider>().enabled = false; // 충돌 비활성화 (미리보기 상태)

        // 미리보기 형식으로 파란색과 투명도를 적용
        Renderer renderer = currentMirrorPreview.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = new Color(0f, 0f, 1f, 0.5f); // 파란색 투명도 적용
        }

        isDragging = true;
    }


    private void StopDraggingMirror()
    {
        if (isDragging)
        {
            ConfirmPlacement();
            isDragging = false;
        }
    }

    private void HandleMirrorDragging()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Ray ray = mainCamera.ScreenPointToRay(touch.position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, placementLayerMask))
            {
                if (touch.phase == TouchPhase.Began)
                {
                    touchOffset = currentMirrorPreview.transform.position - hit.point;
                }

                currentMirrorPreview.transform.position = hit.point + touchOffset;

                if (touch.phase == TouchPhase.Ended)
                {
                    ConfirmPlacement();
                }
            }
        }
    }

    private void ConfirmPlacement()
    {
        isPlacing = true;
        currentMirrorPreview.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 1f); // 원래 색으로 복구
    }

    private void HandleMirrorPlacement()
    {
        isPlacing = false;
        isRotating = true;

        // 기즈모 표시를 위한 초기화
        EnableGizmo(currentMirrorPreview);
    }

    private void HandleMirrorRotation()
    {
        if (currentMirrorPreview == null)
            return;

        // 기즈모를 사용해 회전 각도 조절
        if (currentGizmo != null)
        {
            // 회전 로직은 기즈모 스크립트에서 처리됨
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended)
            {
                FinalizePlacement();
            }
        }
    }

    private void FinalizePlacement()
    {
        currentMirrorPreview.GetComponent<Collider>().enabled = true; // 충돌 활성화
        DisableGizmo(currentMirrorPreview); // 기즈모 비활성화
        currentMirrorPreview = null;
        isRotating = false;

        // 회전이 끝난 후에 플레이어의 움직임을 활성화합니다.
        playerMovement.EnableMovement();
    }

    private void EnableGizmo(GameObject mirror)
    {
        currentGizmo = Instantiate(rotationGizmoPrefab, mirror.transform.position, Quaternion.identity);
        currentGizmo.transform.SetParent(mirror.transform, true); // 기즈모를 거울의 자식으로 설정

        RotationGizmo gizmoScript = currentGizmo.GetComponent<RotationGizmo>();
        if (gizmoScript != null)
        {
            gizmoScript.ClearTargets(); // 이전 타겟 삭제
            gizmoScript.AddTarget(mirror.transform); // 현재 타겟 추가
        }
    }

    private void DisableGizmo(GameObject mirror)
    {
        if (currentGizmo != null)
        {
            Destroy(currentGizmo);
        }
    }
}
