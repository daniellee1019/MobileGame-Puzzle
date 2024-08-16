using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MirrorPlacementManager : MonoBehaviour
{
    public GameObject[] mirrorPrefabs; // �ſ� ������ �迭
    public LayerMask placementLayerMask; // �ſ��� ��ġ�� �� �ִ� ���̾�
    public Button[] mirrorButtons; // ��ư �迭 (ReflectingButton, RefractingButton, AmplifyingButton ��)
    public GameObject rotationGizmoPrefab; // ȸ�� ����� ������
    public PlayerMovement playerMovement; // �÷��̾� ��Ʈ�ѷ� ����

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

        // �� ��ư�� �̺�Ʈ �����ʸ� �߰�
        for (int i = 0; i < mirrorButtons.Length; i++)
        {
            int index = i; // Ŭ���� ������ �����ϱ� ���� ���� ������ �ε����� ����

            // Button ��� EventTrigger�� ����Ͽ� PointerDown, PointerUp �̺�Ʈ ó��
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

        // �ſ��� �����̼��� (0, 0, 0)���� ����
        currentMirrorPreview.transform.rotation = Quaternion.identity;

        // ��ġ ��ġ�κ��� Ray�� ���� ��ġ ������ ��ġ�� ��������
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 touchPosition = hit.point;
            // �ſ��� ��ġ�� ��ġ ��ġ�� X, Z ���� Y�� 4.5�� ����
            currentMirrorPreview.transform.position = new Vector3(touchPosition.x, 4.5f, touchPosition.z);
        }
        else
        {
            // ��ġ ��ġ�� �������� ���� ����� �⺻ ��ġ ó�� (��: ȭ�� �߾� �Ǵ� Ư�� ��ġ)
            currentMirrorPreview.transform.position = new Vector3(0, 4.5f, 0);
        }

        currentMirrorPreview.GetComponent<Collider>().enabled = false; // �浹 ��Ȱ��ȭ (�̸����� ����)

        // �̸����� �������� �Ķ����� ������ ����
        Renderer renderer = currentMirrorPreview.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = new Color(0f, 0f, 1f, 0.5f); // �Ķ��� ���� ����
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
        currentMirrorPreview.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 1f); // ���� ������ ����
    }

    private void HandleMirrorPlacement()
    {
        isPlacing = false;
        isRotating = true;

        // ����� ǥ�ø� ���� �ʱ�ȭ
        EnableGizmo(currentMirrorPreview);
    }

    private void HandleMirrorRotation()
    {
        if (currentMirrorPreview == null)
            return;

        // ����� ����� ȸ�� ���� ����
        if (currentGizmo != null)
        {
            // ȸ�� ������ ����� ��ũ��Ʈ���� ó����
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
        currentMirrorPreview.GetComponent<Collider>().enabled = true; // �浹 Ȱ��ȭ
        DisableGizmo(currentMirrorPreview); // ����� ��Ȱ��ȭ
        currentMirrorPreview = null;
        isRotating = false;

        // ȸ���� ���� �Ŀ� �÷��̾��� �������� Ȱ��ȭ�մϴ�.
        playerMovement.EnableMovement();
    }

    private void EnableGizmo(GameObject mirror)
    {
        currentGizmo = Instantiate(rotationGizmoPrefab, mirror.transform.position, Quaternion.identity);
        currentGizmo.transform.SetParent(mirror.transform, true); // ����� �ſ��� �ڽ����� ����

        RotationGizmo gizmoScript = currentGizmo.GetComponent<RotationGizmo>();
        if (gizmoScript != null)
        {
            gizmoScript.ClearTargets(); // ���� Ÿ�� ����
            gizmoScript.AddTarget(mirror.transform); // ���� Ÿ�� �߰�
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
