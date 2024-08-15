using UnityEngine;
using UnityEngine.UI;

public class MirrorManager : MonoBehaviour
{
    public GameObject hammerCursor; // 망치 커서 이미지
    public Button restoreModeButton; // 리스토어 모드를 활성화하는 버튼
    private bool isRestoreMode = false; // 리스토어 모드 활성화 여부
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        // 리스토어 모드 버튼에 이벤트 리스너 추가
        restoreModeButton.onClick.AddListener(ToggleRestoreMode);

        // 망치 커서를 비활성화된 상태로 시작
        hammerCursor.SetActive(false);
    }

    void Update()
    {
        if (isRestoreMode && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // 터치 위치에서 Ray를 쏴서 거울을 탐지
                Ray ray = mainCamera.ScreenPointToRay(touch.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    Mirror mirror = hit.collider.GetComponent<Mirror>();
                    if (mirror != null)
                    {
                        Destroy(mirror.gameObject); // 거울 삭제
                    }
                }
            }
        }

        // 망치 커서 위치를 터치 위치로 업데이트
        if (isRestoreMode && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 cursorPosition = mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, mainCamera.nearClipPlane));
            hammerCursor.transform.position = new Vector3(cursorPosition.x, cursorPosition.y, hammerCursor.transform.position.z);
        }
    }

    private void ToggleRestoreMode()
    {
        isRestoreMode = !isRestoreMode;

        if (isRestoreMode)
        {
            // 망치 커서를 활성화
            hammerCursor.SetActive(true);
        }
        else
        {
            // 망치 커서를 비활성화
            hammerCursor.SetActive(false);
        }
    }
}
