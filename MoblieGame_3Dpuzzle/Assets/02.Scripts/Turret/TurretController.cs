using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TurretController : MonoBehaviour, IInteractable
{
    public Transform lightOrigin;
    public float lightRange = 100f; // 빛의 최대 범위를 늘립니다.
    public LineRenderer lineRenderer;
    public Transform turretPosition; // 플레이어가 포탑에 탈 때 위치할 자리
    public float dismountCooldown = 3f;

    private bool isMounted = false;
    private bool canMount = true;
    private GameObject player;
    private Camera mainCamera;

    void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.red;

        mainCamera = Camera.main;
    }

    void Update()
    {
        if (isMounted)
        {
            ControlLightDirection();
        }

        ShootLight();
    }

    private void ShootLight()
    {
        Vector3 origin = lightOrigin.position;
        Vector3 direction = lightOrigin.forward;

        Ray ray = new Ray(origin, direction);
        RaycastHit hit;
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, origin);

        while (Physics.Raycast(ray, out hit, lightRange))
        {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);

            Mirror mirror = hit.collider.GetComponent<Mirror>();
            if (mirror != null)
            {
                mirror.ReflectLight(ray, out ray); // 거울에 따라 빛을 반사/굴절/증폭
            }
            else
            {
                break; // 거울이 아닌 다른 물체에 닿으면 빛이 멈춤
            }
        }

        // 최종 빛의 끝점을 설정
        if (lineRenderer.positionCount == 1)
        {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(1, origin + direction * lightRange);
        }
    }

    public void Interact(GameObject player)
    {
        if (!canMount) return;

        this.player = player;

        if (!isMounted)
        {
            MountTurret();
        }
        else
        {
            DismountTurret();
        }
    }

    private void MountTurret()
    {
        player.transform.SetParent(transform);
        player.transform.position = turretPosition.position;
        player.transform.rotation = turretPosition.rotation;

        NavMeshAgent playerAgent = player.GetComponent<NavMeshAgent>();
        if (playerAgent != null)
        {
            playerAgent.enabled = false;
        }

        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            playerRb.isKinematic = true;
            playerRb.useGravity = false;
        }

        isMounted = true;
    }

    private void DismountTurret()
    {
        player.transform.SetParent(null);

        NavMeshAgent playerAgent = player.GetComponent<NavMeshAgent>();
        if (playerAgent != null)
        {
            playerAgent.enabled = true;
        }

        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            playerRb.isKinematic = false;
            playerRb.useGravity = true;
        }

        isMounted = false;
        StartCoroutine(DismountCooldown());
    }

    private IEnumerator DismountCooldown()
    {
        canMount = false;
        yield return new WaitForSeconds(dismountCooldown);
        canMount = true;
    }

    private void ControlLightDirection()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Began)
            {
                Ray ray = mainCamera.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 direction = hit.point - lightOrigin.position;
                    direction.y = 0; // 수평 방향만 고려

                    Quaternion rotation = Quaternion.LookRotation(direction);
                    lightOrigin.rotation = Quaternion.Slerp(lightOrigin.rotation, rotation, Time.deltaTime * 10f);
                }
            }
        }
    }
}
