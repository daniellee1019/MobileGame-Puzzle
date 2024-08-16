using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private LineRenderer lineRenderer;
    private Camera mainCamera;
    private bool canMove = true; // 플레이어가 이동할 수 있는지 여부를 제어하는 플래그

    private void Awake()
    {
        // 적을 ObjectManager에 등록
        ObjectManager.Instance.RegisterPlayer(this);
    }
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        lineRenderer = GetComponent<LineRenderer>();
        mainCamera = Camera.main;

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent가 플레이어에 추가되지 않았습니다.");
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

        // NavMeshAgent 설정 조정
        agent.acceleration = 20f; // 가속도를 높여 장애물을 피할 때 속도를 유지
        agent.angularSpeed = 360f; // 회전 속도를 높여 빠르게 방향 전환
        agent.autoBraking = false; // 목적지 근처에서도 속도 유지
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance; // 장애물 회피 설정
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

        // 이미 이동한 경로를 제거하고 남은 경로만 표시
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

    // 플레이어 이동을 허용하는 메서드
    public void EnableMovement()
    {
        canMove = true;
        agent.isStopped = false; // NavMeshAgent 동작 재개
    }

    // 플레이어 이동을 차단하는 메서드
    public void DisableMovement()
    {
        canMove = false;
        agent.isStopped = true; // NavMeshAgent 동작 중지
        lineRenderer.positionCount = 0; // 경로 표시 제거
    }
}
