using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // Inspector에서 건물 프리뷰 프리팹을 할당합니다.
    public GameObject buildingPrefab;

    // 생성된 프리뷰 오브젝트를 참조할 변수
    private GameObject previewInstance;

    // 드래그 시작 시 호출
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (buildingPrefab != null)
        {
            // 프리뷰 오브젝트 생성
            previewInstance = Instantiate(buildingPrefab);
            // 초기 위치 설정(예: 현재 터치 위치에 맞게)
            UpdatePreviewPosition(eventData);
        }
    }

    // 드래그 중 호출
    public void OnDrag(PointerEventData eventData)
    {
        if (previewInstance != null)
        {
            UpdatePreviewPosition(eventData);
        }
    }

    // 드래그 종료 시 호출
    public void OnEndDrag(PointerEventData eventData)
    {
        // 여기서 최종 위치 검증 및 후속 처리 (예: 위치 유효성 검사 후 건설 모드로 전환)
        // 현재는 예시이므로 프리뷰 오브젝트를 그대로 남겨두거나, 필요에 따라 Destroy(previewInstance)를 호출합니다.
        // previewInstance = null; // 상태 초기화 필요 시
    }

    // 터치 이벤트를 월드 좌표로 변환하여 프리뷰 위치를 업데이트하는 함수
    private void UpdatePreviewPosition(PointerEventData eventData)
    {
        // 스크린 좌표를 월드 좌표로 변환 (Z 값은 카메라와의 거리)
        Vector3 screenPos = new Vector3(eventData.position.x, eventData.position.y, Camera.main.nearClipPlane + 1f);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        // Z축은 0으로 설정해 2D 평면 상에서 동작하도록 조정 (3D 환경의 경우 조정 필요)
        worldPos.z = 0f;
        previewInstance.transform.position = worldPos;
    }
}
