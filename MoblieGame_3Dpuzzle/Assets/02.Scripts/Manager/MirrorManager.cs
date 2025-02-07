using UnityEngine;
using UnityEngine.UI;

public class MirrorManager : MonoBehaviour
{
    private Camera mainCamera;
    private PlayerMovement playerMovement; // 플레이어 컨트롤러 참조
    private JoystickHandler joystick;

    public GameObject hammerCursor; // 망치 커서 이미지
    public Button restoreModeButton; // 리스토어 모드를 활성화하는 버튼

    void Start()
    {
        // 조이스틱 할당
        joystick = ObjectManager.Instance.GetJoystick();
        if (joystick == null)
        {
            Debug.LogError("Joystick is not registered in ObjectManager!");
        }

        mainCamera = Camera.main;

        // 플레이어 찾기
        FindPlayer();

        // 리스토어 모드 버튼에 이벤트 리스너 추가
        restoreModeButton.onClick.AddListener(ToggleRestoreMode);

        // 망치 커서를 비활성화된 상태로 시작
        hammerCursor.SetActive(false);
    }

    void Update()
    {
        if (playerMovement == null)
        {
            FindPlayer();
        }

        // 리스토어 모드에서 터치 감지 및 거울 삭제
        if (PlayerModeController.Instance.IsMode(PlayerMode.Restore) && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
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

        // 망치 커서 위치 업데이트
        if (PlayerModeController.Instance.IsMode(PlayerMode.Restore) && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 cursorPosition = mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, mainCamera.nearClipPlane));
            hammerCursor.transform.position = new Vector3(cursorPosition.x, cursorPosition.y, hammerCursor.transform.position.z);
        }
    }

    private void FindPlayer()
    {
        // ObjectManager를 통해 플레이어 참조
        playerMovement = ObjectManager.Instance.player;
    }

    private void ToggleRestoreMode()
    {
        if (PlayerModeController.Instance.IsMode(PlayerMode.Restore))
        {
            //리스토어 모드 종료 → 터렛 모드로 돌아감
            PlayerModeController.Instance.SetMode(PlayerMode.Turret);
            hammerCursor.SetActive(false);

            Time.timeScale = 1f;
        }
        else
        {
            //현재 터렛 모드일 때만 리스토어 모드로 전환 가능
            if (PlayerModeController.Instance.IsMode(PlayerMode.Turret))
            {
                PlayerModeController.Instance.SetMode(PlayerMode.Restore);
                hammerCursor.SetActive(true);

                Time.timeScale = 0f;

                if (joystick != null)
                {
                    joystick.enabled = false;
                }
            }
            else
            {
                Debug.LogWarning("Restore mode can only be activated from Turret mode!");
            }
        }
    }

}
