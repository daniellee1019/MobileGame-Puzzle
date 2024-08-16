using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Instance { get; private set; }

    public PlayerMovement player;  // 플레이어 참조
    public TurretController turret;  // 터렛 참조
    public EnemyAI[] enemies;  // 적 참조 배열

    private void Awake()
    {
        // 싱글톤 패턴으로 GameManager 인스턴스 관리
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 씬 변경 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterPlayer(PlayerMovement player)
    {
        this.player = player;
    }

    public void RegisterTurret(TurretController turret)
    {
        this.turret = turret;
    }

    public void RegisterEnemy(EnemyAI enemy)
    {
        // 적을 배열에 추가
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] == null)
            {
                enemies[i] = enemy;
                break;
            }
        }
    }
}
