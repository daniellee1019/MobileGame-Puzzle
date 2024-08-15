using UnityEngine;
using UnityEngine.UI;

public class MirrorManager : MonoBehaviour
{
    public GameObject hammerCursor; // ��ġ Ŀ�� �̹���
    public Button restoreModeButton; // ������� ��带 Ȱ��ȭ�ϴ� ��ư
    private bool isRestoreMode = false; // ������� ��� Ȱ��ȭ ����
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        // ������� ��� ��ư�� �̺�Ʈ ������ �߰�
        restoreModeButton.onClick.AddListener(ToggleRestoreMode);

        // ��ġ Ŀ���� ��Ȱ��ȭ�� ���·� ����
        hammerCursor.SetActive(false);
    }

    void Update()
    {
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

    private void ToggleRestoreMode()
    {
        isRestoreMode = !isRestoreMode;

        if (isRestoreMode)
        {
            // ��ġ Ŀ���� Ȱ��ȭ
            hammerCursor.SetActive(true);
        }
        else
        {
            // ��ġ Ŀ���� ��Ȱ��ȭ
            hammerCursor.SetActive(false);
        }
    }
}
