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

    // JSON을 통해 로드할 데미지 데이터
    public TurretDamageData turretDamageData;

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

        mainCamera = Camera.main;

        // JSON 파일 로드 및 파싱 (Resources 폴더 내의 파일 이름은 확장자 없이 사용)
        TextAsset jsonText = Resources.Load<TextAsset>("Data/turretDamageData");
        if (jsonText != null)
        {
            turretDamageData = JsonUtility.FromJson<TurretDamageData>(jsonText.text);
            Debug.Log("데미지 데이터 로드 성공");
        }
        else
        {
            Debug.LogError("turretDamageData.json 파일을 찾을 수 없습니다. Resources 폴더에 파일이 있는지 확인하세요.");
        }
    }

    void Update()
    {
        if (isMounted)
        {
            ControlLightDirection();
        }

        ShootLight();
    }

    // 데미지 계산 예시 메서드
    private float CalculateDamage()
    {
        // 기본 공격력부터 시작
        float damage = turretDamageData.basicDamage * Mathf.Pow(1f + turretDamageData.upgradeMultiplier, turretDamageData.currentLevel);

        return damage;
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

        // 초기 데미지를 계산 (예: 기본 공격력 + 치명타 판정 등)
        float currentDamage = CalculateDamage();

        while (true)
        {
            // Raycast로 물체와 충돌하는지 확인
            if (Physics.Raycast(ray, out hit, lightRange))
            {
                // 라인 렌더러에 현재 충돌 지점을 추가
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);

                // 먼저, 충돌한 물체가 거울(Mirror)인지 확인
                if (hit.collider.TryGetComponent<Mirror>(out Mirror mirror))
                {
                    // 동일한 거울에 계속 충돌하는 무한 루프 방지
                    if (mirror == lastHitMirror)
                    {
                        break;
                    }
                    lastHitMirror = mirror;

                    // 충돌 지점에서 약간의 오프셋 적용 (다음 Raycast의 시작점)
                    ray.origin = hit.point + hit.normal * 0.01f;

                    // 거울의 종류에 따라 Ray의 방향과 데미지 변화를 처리
                    if (mirror is ReflectingMirror reflectingMirror)
                    {
                        // 반사 거울은 데미지 변화 없이 반사
                        reflectingMirror.ReflectLight(ray, out ray);
                    }
                    else if (mirror is RefractingMirror refractingMirror)
                    {
                        // 굴절 거울은 데미지를 50%로 감소
                        currentDamage *= 0.5f;
                        refractingMirror.ReflectLight(ray, out ray);
                    }
                    else if (mirror is AmplifyingMirror amplifyingMirror)
                    {
                        // 증폭 거울은 데미지를 증가 (예시로 2배)
                        currentDamage *= 2.0f;
                        amplifyingMirror.ReflectLight(ray, out ray);
                    }
                    else
                    {
                        // 처리되지 않은 거울 타입이면 종료
                        break;
                    }
                }
                // 거울이 아니라면 적(EnemyAI) 체크
                else if (hit.collider.TryGetComponent<EnemyAI>(out EnemyAI enemy))
                {
                    float damage = currentDamage; // turret에서 계산된 데미지
                    float armorPenetration = turretDamageData.armorPenetration; // JSON에서 로드한 관통력 값
                    bool isCrit = Random.value < turretDamageData.critChance; // 치명타 확률에 따라 치명타 여부 결정

                    // 치명타일 경우 데미지 배율 적용
                    if (isCrit)
                    {
                        damage *= turretDamageData.critMultiplier;
                    }

                    enemy.TakeDamage(damage, armorPenetration, isCrit);
                    break;
                }
                else
                {
                    // 거울도 적도 아닌 다른 오브젝트에 충돌하면 데미지 적용 없이 종료
                    break;
                }
            }
            else
            {
                // Ray가 아무것도 맞지 않으면, 끝점을 라인 렌더러에 추가 후 종료
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
