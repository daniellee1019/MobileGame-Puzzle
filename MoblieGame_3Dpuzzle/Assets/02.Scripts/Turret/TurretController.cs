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


    private void Awake()
    {
        // 터렛 초기화 코드
        ObjectManager.Instance.RegisterTurret(this);
    }

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

        Mirror lastHitMirror = null;  // 이전에 충돌한 거울을 저장

        while (true)
        {
            // Ray가 충돌하는지 확인
            if (Physics.Raycast(ray, out hit, lightRange))
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);

                // Mirror 클래스나 자식 클래스를 확인
                if (hit.collider.TryGetComponent<Mirror>(out Mirror mirror))
                {
                    // 이전에 충돌한 거울과 같으면 루프 종료
                    if (mirror == lastHitMirror)
                    {
                        break;
                    }

                    // 현재 충돌한 거울을 기록
                    lastHitMirror = mirror;

                    // 현재 충돌한 지점에서 약간의 오프셋 적용
                    ray.origin = hit.point + hit.normal * 0.01f;

                    // Mirror의 자식 클래스에 따라 반사, 굴절, 증폭을 처리
                    if (mirror is ReflectingMirror reflectingMirror)
                    {
                        reflectingMirror.ReflectLight(ray, out ray);
                    }
                    else if (mirror is RefractingMirror refractingMirror)
                    {
                        refractingMirror.ReflectLight(ray, out ray);
                    }
                    else if (mirror is AmplifyingMirror amplifyingMirror)
                    {
                        amplifyingMirror.ReflectLight(ray, out ray);
                    }
                    else
                    {
                        break; // 처리되지 않은 경우 종료
                    }
                }
                else
                {
                    break; // 거울이 아닌 다른 물체에 닿으면 빛이 멈춤
                }
            }
            else
            {
                // Ray가 아무것도 맞지 않을 경우 끝점을 설정
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, ray.origin + ray.direction * lightRange);
                break;
            }
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
