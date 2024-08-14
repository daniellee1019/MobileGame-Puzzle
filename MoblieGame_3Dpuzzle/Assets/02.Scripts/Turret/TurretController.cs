using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TurretController : MonoBehaviour, IInteractable
{
    public Transform lightOrigin;
    public float lightRange = 10f;
    public LineRenderer lineRenderer;
    public Transform turretPosition; // 플레이어가 포탑에 탈 때 위치할 자리
    public float dismountCooldown = 3f;

    private bool isMounted = false;
    private bool canMount = true;
    private GameObject player;

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
    }

    void Update()
    {
        ShootLight();
    }

    private void ShootLight()
    {
        lineRenderer.SetPosition(0, lightOrigin.position);
        RaycastHit hit;

        if (Physics.Raycast(lightOrigin.position, lightOrigin.forward, out hit, lightRange))
        {
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            lineRenderer.SetPosition(1, lightOrigin.position + lightOrigin.forward * lightRange);
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
}
