using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    public TurretController turretPrefab;  // ��ž ������
    public PlayerMovement playerPrefab;  // �÷��̾� ������

    public Transform turretSpawnPoint;  // ��ž ���� ��ġ
    public Transform playerSpawnPoint;  // �÷��̾� ���� ��ġ

    void Start()
    {
        SpawnTurret();
        SpawnPlayer();
    }

    // ��ž ���� �� ���
    private void SpawnTurret()
    {
        if (turretPrefab != null && turretSpawnPoint != null)
        {
            TurretController turret = Instantiate(turretPrefab, turretSpawnPoint.position, turretSpawnPoint.rotation);
            ObjectManager.Instance.RegisterTurret(turret);  // ObjectManager�� ��ž ���
            Debug.Log("Turret spawned and registered: " + turret.name);
        }
        else
        {
            Debug.LogError("Turret prefab or spawn point not set in SpawnerManager.");
        }
    }

    // �÷��̾� ���� �� ���
    private void SpawnPlayer()
    {
        if (playerPrefab != null && playerSpawnPoint != null)
        {
            PlayerMovement player = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
            ObjectManager.Instance.RegisterPlayer(player);  // ObjectManager�� �÷��̾� ���
            Debug.Log("Player spawned and registered: " + player.name);
        }
        else
        {
            Debug.LogError("Player prefab or spawn point not set in SpawnerManager.");
        }
    }
}
