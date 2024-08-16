using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private LineRenderer lineRenderer;
    private Camera mainCamera;
    private bool canMove = true; // �÷��̾ �̵��� �� �ִ��� ���θ� �����ϴ� �÷���

    private void Awake()
    {
        // ���� ObjectManager�� ���
        ObjectManager.Instance.RegisterPlayer(this);
    }
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        lineRenderer = GetComponent<LineRenderer>();
        mainCamera = Camera.main;

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent�� �÷��̾ �߰����� �ʾҽ��ϴ�.");
        }

        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.red;

        // NavMeshAgent ���� ����
        agent.acceleration = 20f; // ���ӵ��� ���� ��ֹ��� ���� �� �ӵ��� ����
        agent.angularSpeed = 360f; // ȸ�� �ӵ��� ���� ������ ���� ��ȯ
        agent.autoBraking = false; // ������ ��ó������ �ӵ� ����
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance; // ��ֹ� ȸ�� ����
    }

    void Update()
    {
        if (canMove)
        {
            HandleTouchInput();
            UpdatePathLine();
        }
        //UpdatePathLine();
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = mainCamera.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    agent.SetDestination(hit.point);
                    UpdatePathLine();
                }
            }
        }
    }

    private void UpdatePathLine()
    {
        if (agent.pathPending)
            return;

        NavMeshPath path = agent.path;

        if (path.status == NavMeshPathStatus.PathComplete)
        {
            lineRenderer.positionCount = path.corners.Length;
            lineRenderer.SetPositions(path.corners);
        }

        // �̹� �̵��� ��θ� �����ϰ� ���� ��θ� ǥ��
        Vector3[] remainingPath = GetRemainingPath();
        lineRenderer.positionCount = remainingPath.Length;
        lineRenderer.SetPositions(remainingPath);
    }

    private Vector3[] GetRemainingPath()
    {
        if (agent.path.corners.Length == 0)
            return new Vector3[0];

        Vector3 playerPosition = transform.position;
        Vector3[] fullPath = agent.path.corners;

        int startIndex = 0;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < fullPath.Length; i++)
        {
            float distance = Vector3.Distance(playerPosition, fullPath[i]);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                startIndex = i;
            }
        }

        Vector3[] remainingPath = new Vector3[fullPath.Length - startIndex];
        for (int i = startIndex; i < fullPath.Length; i++)
        {
            remainingPath[i - startIndex] = fullPath[i];
        }

        return remainingPath;
    }

    // �÷��̾� �̵��� ����ϴ� �޼���
    public void EnableMovement()
    {
        canMove = true;
        agent.isStopped = false; // NavMeshAgent ���� �簳
    }

    // �÷��̾� �̵��� �����ϴ� �޼���
    public void DisableMovement()
    {
        canMove = false;
        agent.isStopped = true; // NavMeshAgent ���� ����
        lineRenderer.positionCount = 0; // ��� ǥ�� ����
    }
}
