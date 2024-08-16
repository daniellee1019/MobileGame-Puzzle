using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Instance { get; private set; }

    public PlayerMovement player;  // �÷��̾� ����
    public TurretController turret;  // �ͷ� ����
    public EnemyAI[] enemies;  // �� ���� �迭

    private void Awake()
    {
        // �̱��� �������� GameManager �ν��Ͻ� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // �� ���� �� �ı����� �ʵ��� ����
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
        // ���� �迭�� �߰�
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
