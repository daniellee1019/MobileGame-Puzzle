using UnityEngine;
using UnityEngine.UI;

public class MirrorManager : MonoBehaviour
{
    private Camera mainCamera;
    private PlayerMovement playerMovement; // �÷��̾� ��Ʈ�ѷ� ����
    public GameObject hammerCursor; // ��ġ Ŀ�� �̹���
    public Button restoreModeButton; // ������� ��带 Ȱ��ȭ�ϴ� ��ư

    private bool isRestoreMode = false; // ������� ��� Ȱ��ȭ ����

    void Start()
    {
        mainCamera = Camera.main;

        // �÷��̾� ã��
        FindPlayer();

        // ������� ��� ��ư�� �̺�Ʈ ������ �߰�
        restoreModeButton.onClick.AddListener(ToggleRestoreMode);

        // ��ġ Ŀ���� ��Ȱ��ȭ�� ���·� ����
        hammerCursor.SetActive(false);
    }

    void Update()
    {
        if (playerMovement == null)
        {
            FindPlayer();
        }

        if (isRestoreMode && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // ��ġ ��ġ���� Ray�� ���� �ſ��� Ž��
                Ray ray = mainCamera.ScreenPointToRay(touch.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    Mirror mirror = hit.collider.GetComponent<Mirror>();
                    if (mirror != null)
                    {
                        Destroy(mirror.gameObject); // �ſ� ����
                    }
                }
            }
        }

        // ��ġ Ŀ�� ��ġ�� ��ġ ��ġ�� ������Ʈ
        if (isRestoreMode && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 cursorPosition = mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, mainCamera.nearClipPlane));
            hammerCursor.transform.position = new Vector3(cursorPosition.x, cursorPosition.y, hammerCursor.transform.position.z);
        }
    }
    private void FindPlayer()
    {
        // ObjectManager�� ���� �÷��̾� ����
        playerMovement = ObjectManager.Instance.player;
    }

    private void ToggleRestoreMode()
    {
        isRestoreMode = !isRestoreMode;

        if (isRestoreMode)
        {
            // ��ġ Ŀ���� Ȱ��ȭ
            hammerCursor.SetActive(true);
            playerMovement.DisableMovement(); // �÷��̾� �̵� ��Ȱ��ȭ
        }
        else
        {
            // ��ġ Ŀ���� ��Ȱ��ȭ
            hammerCursor.SetActive(false);
            playerMovement.EnableMovement(); // �÷��̾� �̵� Ȱ��ȭ
        }
    }
}
