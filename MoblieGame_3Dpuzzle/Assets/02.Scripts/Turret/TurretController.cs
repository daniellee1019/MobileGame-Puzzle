using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TurretController : MonoBehaviour, IInteractable
{
    public Transform lightOrigin;
    public float lightRange = 100f; // ���� �ִ� ������ �ø��ϴ�.
    public LineRenderer lineRenderer;
    public Transform turretPosition; // �÷��̾ ��ž�� Ż �� ��ġ�� �ڸ�
    public float dismountCooldown = 3f;

    private bool isMounted = false;
    private bool canMount = true;
    private GameObject player;
    private Camera mainCamera;


    private void Awake()
    {
        // �ͷ� �ʱ�ȭ �ڵ�
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

        Mirror lastHitMirror = null;  // ������ �浹�� �ſ��� ����

        while (true)
        {
            // Ray�� �浹�ϴ��� Ȯ��
            if (Physics.Raycast(ray, out hit, lightRange))
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);

                // Mirror Ŭ������ �ڽ� Ŭ������ Ȯ��
                if (hit.collider.TryGetComponent<Mirror>(out Mirror mirror))
                {
                    // ������ �浹�� �ſ�� ������ ���� ����
                    if (mirror == lastHitMirror)
                    {
                        break;
                    }

                    // ���� �浹�� �ſ��� ���
                    lastHitMirror = mirror;

                    // ���� �浹�� �������� �ణ�� ������ ����
                    ray.origin = hit.point + hit.normal * 0.01f;

                    // Mirror�� �ڽ� Ŭ������ ���� �ݻ�, ����, ������ ó��
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
                        break; // ó������ ���� ��� ����
                    }
                }
                else
                {
                    break; // �ſ��� �ƴ� �ٸ� ��ü�� ������ ���� ����
                }
            }
            else
            {
                // Ray�� �ƹ��͵� ���� ���� ��� ������ ����
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
                    direction.y = 0; // ���� ���⸸ ���

                    Quaternion rotation = Quaternion.LookRotation(direction);
                    lightOrigin.rotation = Quaternion.Slerp(lightOrigin.rotation, rotation, Time.deltaTime * 10f);
                }
            }
        }
    }
}
