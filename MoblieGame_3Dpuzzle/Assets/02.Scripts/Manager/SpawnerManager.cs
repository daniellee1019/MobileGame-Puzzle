using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    public TurretController turretPrefab;  // 포탑 프리팹
    public JoystickHandler joystickPrefab;
    public PlayerMovement playerPrefab;  // 플레이어 프리팹

    public Transform turretSpawnPoint;  // 포탑 스폰 위치
    public Transform playerSpawnPoint;  // 플레이어 스폰 위치
    public Canvas joystickSpawnPoint;  // 플레이어 스폰 위치

    void Start()
    {
        SpawnJoystick();
        SpawnTurret();
        SpawnPlayer();
    }

    // 포탑 스폰 및 등록
    private void SpawnTurret()
    {
        if (turretPrefab != null && turretSpawnPoint != null)
        {
            TurretController turret = Instantiate(turretPrefab, turretSpawnPoint.position, turretSpawnPoint.rotation);
            ObjectManager.Instance.RegisterTurret(turret);  // ObjectManager에 포탑 등록
            Debug.Log("Turret spawned and registered: " + turret.name);
        }
        else
        {
            Debug.LogError("Turret prefab or spawn point not set in SpawnerManager.");
        }
    }

    // 플레이어 스폰 및 등록
    private void SpawnPlayer()
    {
        if (playerPrefab != null && playerSpawnPoint != null)
        {
            PlayerMovement player = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
            ObjectManager.Instance.RegisterPlayer(player);  // ObjectManager에 플레이어 등록
            Debug.Log("Player spawned and registered: " + player.name);
        }
        else
        {
            Debug.LogError("Player prefab or spawn point not set in SpawnerManager.");
        }
    }

    // 플레이어 스폰 및 등록
    private void SpawnJoystick()
    {
        if (joystickPrefab != null)
        {
            if (joystickSpawnPoint != null)
            {
                // 조이스틱 인스턴스화 및 캔버스의 자식으로 설정
                JoystickHandler joystick = Instantiate(joystickPrefab, joystickSpawnPoint.transform);
                ObjectManager.Instance.RegisterJoystick(joystick); // ObjectManager에 등록
                Debug.Log("Joystick spawned and registered: " + joystick.name);
            }
            else
            {
                Debug.LogError("Canvas not found in scene. Joystick cannot be spawned.");
            }
        }
    }
}
